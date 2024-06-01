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
    public Text playerName;
    private void OnEnable()
    {
        if (isOnline)
        {
            score = PlayfabManager.Instance.GetOnlineScoreAtIndex(index);
            playerName.text = PlayfabManager.Instance.GetOnlineNameAtIndex(index);
            if (PlayfabManager.Instance.IsCurUserName(index))
            {
                playerName.color = Color.blue;
            }
            else
            {
                playerName.color = Color.black;
            }

        }
        else
        {
            score = GameManager.Instance.GetScoreAtIndex(index);
        }
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
