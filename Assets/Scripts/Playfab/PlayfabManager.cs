using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab;
using System;

public class PlayfabManager : Singleton<PlayfabManager>
{
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

    private void OnUpdatePlayerStatistics(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Liaderboard Updated!");
    }
    private void OnError(PlayFabError error)
    {
        Debug.Log("Login Errror");
        Debug.Log(error.GenerateErrorReport());
    }
}
