using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class GameManager : Singleton<GameManager>
{
    public bool isOffLine;
    private List<int> scoreList;
    private int score;
    private string dataPath;

    protected override void Awake()
    {
        base.Awake();
        dataPath = Application.persistentDataPath + "/leaderboard.json";
        scoreList = GetScoreList();
        DontDestroyOnLoad(this);
    }

    private void OnEnable()
    {
        EventHandler.FrogDead += OnFrogDead;
        EventHandler.FrogJumpSucceed += OnFrogJumpSucceed;
    }

    private void OnDisable()
    {
        EventHandler.FrogDead -= OnFrogDead;
        EventHandler.FrogJumpSucceed -= OnFrogJumpSucceed;
    }

    private void OnFrogJumpSucceed(int _score)
    {
        score = _score;
    }

    public int GetScoreAtIndex(int index)
    {
        if (index > scoreList.Count - 1)
            return -1;
        if (scoreList[index] == 0) return -1;
        return scoreList[index];
    }

    private void OnFrogDead()
    {
        if (!scoreList.Contains(score))
        {
            scoreList.Add(score);
        }
        scoreList.Sort();
        scoreList.Reverse();
        if (scoreList.Count > 10)
        {
            scoreList = scoreList.GetRange(0, 10);
        }

        SaveScoreList();

        int maxScore = scoreList.Count > 0 ? scoreList[0] : score;
        if (maxScore > 0)
            PlayfabManager.Instance.SendLeaderboard(maxScore);
    }

    private List<int> GetScoreList()
    {
#if UNITY_WEBGL
        string jsonData = PlayerPrefs.GetString("leaderboard", string.Empty);
        if (!string.IsNullOrEmpty(jsonData))
        {
            List<int> list = JsonConvert.DeserializeObject<List<int>>(jsonData);
            if (list.Count > 10)
            {
                list = list.GetRange(0, 10);
            }
            return list;
        }
#else
        if (File.Exists(dataPath))
        {
            string jsonData = File.ReadAllText(dataPath);
            List<int> list = JsonConvert.DeserializeObject<List<int>>(jsonData);
            if (list.Count > 10)
            {
                list = list.GetRange(0, 10);
            }
            return list;
        }
#endif
        return new List<int>();
    }

    private void SaveScoreList()
    {
#if UNITY_WEBGL
        PlayerPrefs.SetString("leaderboard", JsonConvert.SerializeObject(scoreList));
        PlayerPrefs.Save();
#else
        File.WriteAllText(dataPath, JsonConvert.SerializeObject(scoreList));
#endif
    }
}
