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
    public bool IsHaveName() { return userName != string.Empty; }
    private string userName = string.Empty;
    private int maxScore;
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        Login();
    }

    private void Login()
    {
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
        maxScore = score;
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
    private async Task<UpdatePlayerStatisticsResult> SenLeaderboardAsync()
    {
        var request = new UpdatePlayerStatisticsRequest();
        request.Statistics = new List<StatisticUpdate>
        {
            new StatisticUpdate{
                StatisticName = "HighScores",
                Value = maxScore
            },
        };
        var t = new TaskCompletionSource<UpdatePlayerStatisticsResult>();

        // 发起异步请求获取排行榜数据
        PlayFabClientAPI.UpdatePlayerStatistics(request, result =>
        {
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
                await SenLeaderboardAsync();
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
    public async Task<Settings.LoginReturnType> SubmitNameAsync(string name)
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

            var t = new TaskCompletionSource<Settings.LoginReturnType>();

            PlayFabClientAPI.UpdateUserTitleDisplayName(request, result =>
            {
                OnDisplayNameUpdate(result);
                t.SetResult(Settings.LoginReturnType.Success);
            }, error =>
            {
                // Check if the error is related to the display name being the same
                if (error.Error == PlayFabErrorCode.NameNotAvailable) // Replace with the actual error code for name not available
                {
                    t.SetResult(Settings.LoginReturnType.NameRepeat);
                }
                else
                {
                    OnError(error);
                    t.SetException(new Exception(error.GenerateErrorReport()));
                }
            });

            return await t.Task;
        }
        else
        {
            Debug.Log("The display name is already the same as the remote name.");
            return Settings.LoginReturnType.SameName; // Or handle it in another way if necessary
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

    public int GetUserRank()
    {
        if (nameList == null || !IsHaveName()) return -1;
        for (int i = 0; i < nameList.Count; i++)
        {
            if (userName == nameList[i])
                return i + 1;
        }
        return -1;
    }
}
