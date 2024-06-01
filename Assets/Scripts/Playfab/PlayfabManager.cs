using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab;
using System;
using System.Threading.Tasks;

public class PlayfabManager : Singleton<PlayfabManager>
{
    private List<int> scoreList;
    private List<string> nameList;
    private bool isLogin;
    public bool IsLogin() { return isLogin; }
    private string userName = string.Empty;
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
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
            // 发送数据混合请求
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true,
            }
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnError);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login Success");
        if (result.InfoResultPayload.PlayerProfile != null)
        {
            userName = result.InfoResultPayload.PlayerProfile.DisplayName;
        }
        isLogin = true;
    }
    private async Task<LoginResult> LoginAsync()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };

        var t = new TaskCompletionSource<LoginResult>();

        // 发起异步请求获取排行榜数据
        PlayFabClientAPI.LoginWithCustomID(request, result =>
        {
            Debug.Log("Login Success");
            isLogin = true;
        }, error =>
        {
            // 处理错误情况
            OnError(error);
            // 设置 TaskCompletionSource 的异常为当前错误
            t.SetException(new Exception(error.GenerateErrorReport()));
        });

        try
        {
            return await t.Task;
        }
        catch (Exception e)
        {
            Debug.LogError("Login Error: " + e.Message);
            throw; // rethrow the exception
        }
    }

    public void SendLeaderboard(int score)
    {
        if (!isLogin || !IsUserNameInit()) return;
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
        if (scoreList == null || index >= scoreList.Count)
        {
            return -1;
        }
        else
        {
            return scoreList[index];
        }
    }
    public string GetOnlineNameAtIndex(int index)
    {
        if (nameList == null || index >= nameList.Count)
        {
            return string.Empty;
        }
        else
        {
            return nameList[index];
        }
    }
    public bool IsUserNameInit() { return userName != null && userName != string.Empty; }
    public bool IsCurUserName(int index)
    {
        if (nameList == null || index >= nameList.Count)
        {
            return false;
        }
        else
        {
            return nameList[index] == userName;
        }
    }

    private void OnUpdatePlayerStatistics(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Leaderboard Updated!");
        GetLeaderboardData();
    }
    /// <summary>
    /// 不会等待网络的返回
    /// </summary>
    private void GetLeaderboardData()
    {
        var request = new GetLeaderboardRequest();
        request.StatisticName = "HighScores";
        request.StartPosition = 0;
        request.MaxResultsCount = 10;
        PlayFabClientAPI.GetLeaderboard(request, OnGetLeaderboard, OnError);
    }
    /// <summary>
    /// 处理异步数据，让其等待网络响应
    /// </summary>
    /// <returns>Task对象</returns>
    public async Task<GetLeaderboardResult> GetLeaderboardDataAsync()
    {
        if (!isLogin)
        {
            try
            {
                var result = await LoginAsync();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw new Exception("Failed to login"); // Throw a new exception if login fails
            }
        }
        var request = new GetLeaderboardRequest
        {
            StatisticName = "HighScores",
            StartPosition = 0,
            MaxResultsCount = 10
        };

        var t = new TaskCompletionSource<GetLeaderboardResult>();

        // 发起异步请求获取排行榜数据
        PlayFabClientAPI.GetLeaderboard(request, result =>
        {
            // 处理获取到的排行榜数据
            OnGetLeaderboard(result);
            // 设置 TaskCompletionSource 的结果为获取到的排行榜数据
            t.SetResult(result);
        }, error =>
        {
            // 处理错误情况
            OnError(error);
            // 设置 TaskCompletionSource 的异常为当前错误
            t.SetException(new Exception(error.GenerateErrorReport()));
        });
        return await t.Task;
    }

    private void OnGetLeaderboard(GetLeaderboardResult result)
    {
        scoreList = new List<int>();
        nameList = new List<string>();
        foreach (var item in result.Leaderboard)
        {
            scoreList.Add(item.StatValue);
            nameList.Add(item.DisplayName);
        }
    }

    private void OnError(PlayFabError error)
    {
        Debug.Log("Login Errror");
        Debug.Log(error.GenerateErrorReport());
    }

    #region 设置用户名字
    public async Task<UpdateUserTitleDisplayNameResult> SubmitNameAsync(string name)
    {
        if (!isLogin)
        {
            try
            {
                var result = await LoginAsync();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw new Exception("Failed to login"); // Throw a new exception if login fails
            }
        }
        var currentName = await GetCurrentDisplayNameAsync();

        if (currentName != name)
        {
            var request = new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = name,
            };

            var t = new TaskCompletionSource<UpdateUserTitleDisplayNameResult>();

            PlayFabClientAPI.UpdateUserTitleDisplayName(request, result =>
            {
                OnDisplayNameUpdate(result);
                t.SetResult(result);
            }, error =>
            {
                OnError(error);
                t.SetException(new Exception(error.GenerateErrorReport()));
            });

            return await t.Task;
        }
        else
        {
            Debug.Log("The display name is already the same as the remote name.");
            return null; // Or handle it in another way if necessary
        }

    }
    private async Task<string> GetCurrentDisplayNameAsync()
    {
        var request = new GetAccountInfoRequest();

        var t = new TaskCompletionSource<GetAccountInfoResult>();

        PlayFabClientAPI.GetAccountInfo(request, result =>
        {
            t.SetResult(result);
        }, error =>
        {
            t.SetException(new Exception(error.GenerateErrorReport()));
        });

        var accountInfoResult = await t.Task;

        return accountInfoResult.AccountInfo.TitleInfo.DisplayName;
    }

    // private void SubmitName(string name)
    // {
    //     var request = new UpdateUserTitleDisplayNameRequest
    //     {
    //         DisplayName = name,
    //     };

    //     PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, OnError);
    // }

    private void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result)
    {
        userName = result.DisplayName;
        Debug.Log("DisplayName Updated");
    }
    #endregion


    public async Task<int> GetUserRankAsync()
    {
        if (!isLogin)
        {
            try
            {
                await LoginAsync();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw new Exception("Failed to login");
            }
        }

        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = "HighScores",
            MaxResultsCount = 1
        };

        var t = new TaskCompletionSource<GetLeaderboardAroundPlayerResult>();

        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, result =>
        {
            t.SetResult(result);
        }, error =>
        {
            OnError(error);
            t.SetException(new Exception(error.GenerateErrorReport()));
        });

        var leaderboardResult = await t.Task;
        // 检查返回的排行榜列表是否为空
        if (leaderboardResult.Leaderboard == null || leaderboardResult.Leaderboard.Count == 0)
        {
            Debug.LogWarning("Player does not have a rank in the leaderboard.");
            return -1; // 或者返回一个适当的默认值，例如-1
        }
        var userRank = leaderboardResult.Leaderboard[0].Position;

        return userRank + 1;
    }
}
