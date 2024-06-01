using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab;
using System;

public class PlayfabManager : Singleton<PlayfabManager>
{
    private List<int> scoreList;
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        Login();
    }
    private void Login()
    {
        // 闭包方法
        // var request = new LoginWithCustomIDRequest
        // {
        //     CustomId = SystemInfo.deviceUniqueIdentifier,
        //     CreateAccount = true,
        // };
        var request = new LoginWithCustomIDRequest();
        request.CustomId = SystemInfo.deviceUniqueIdentifier;
        request.CreateAccount = true;

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnError);

    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login Success");
    }

    public void SendLeaderboard(int score)
    {
        var request = new UpdatePlayerStatisticsRequest();
        request.Statistics = new List<StatisticUpdate>
        {
            new StatisticUpdate{
                StatisticName = "HighScores",
                Value = score
            },
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnUpdatePlayerStatistics, OnError);
    }

    public int GetOnlineScoreAtIndex(int index)
    {
        Debug.Log(scoreList);
        if (scoreList == null || index >= scoreList.Count)
        {
            return -1;
        }
        else
        {
            return scoreList[index];
        }
    }

    private void OnUpdatePlayerStatistics(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Leaderboard Updated!");
        GetLeaderboardData();
    }
    // 创建一个协程方法来封装GetLeaderboard的异步处理
    public IEnumerator GetLeaderboardDataCoroutine(Action<GetLeaderboardResult> onSuccess, Action<PlayFabError> onError)
    {
        bool isDone = false;
        GetLeaderboardResult leaderboardResult = null;
        PlayFabError errorResult = null;

        // 发起请求
        PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest
        {
            StatisticName = "HighScores",
            StartPosition = 0,
            MaxResultsCount = 10
        }, result =>
        {
            leaderboardResult = result;
            isDone = true;
        }, error =>
        {
            errorResult = error;
            isDone = true;
        });

        // 等待请求完成
        yield return new WaitUntil(() => isDone);

        // 处理结果
        if (errorResult != null)
        {
            onError?.Invoke(errorResult);
        }
        else
        {
            onSuccess?.Invoke(leaderboardResult);
        }
    }
    public void GetLeaderboardData()
    {
        var request = new GetLeaderboardRequest();
        request.StatisticName = "HighScores";
        request.StartPosition = 0;
        request.MaxResultsCount = 10;
        PlayFabClientAPI.GetLeaderboard(request, OnGetLeaderboard, OnError);
    }
    private void OnGetLeaderboard(GetLeaderboardResult result)
    {
        scoreList = new List<int>();
        foreach (var item in result.Leaderboard)
        {
            Debug.Log(item.Position + " " + item.DisplayName + " " + item.StatValue);
            scoreList.Add(item.StatValue);
        }
    }


    private void OnError(PlayFabError error)
    {
        Debug.Log("Login Errror");
        Debug.Log(error.GenerateErrorReport());
    }
}
