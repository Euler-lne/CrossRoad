using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreRecord : MonoBehaviour
{
    public Text scoreText;
    public int index;
    private int score;
    public bool isOnline;
    private void OnEnable()
    {
        if (isOnline)
        {
            score = PlayfabManager.Instance.GetOnlineScoreAtIndex(index);
        }
        else
        {
            score = GameManager.Instance.GetScoreAtIndex(index);
        }
        Debug.Log(score);
        if (score == -1)
        {
            gameObject.SetActive(false);
        }
        else
        {
            scoreText.text = score.ToString();
        }
    }

}
