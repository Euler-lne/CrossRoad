using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreRecord : MonoBehaviour
{
    public Text scoreText;
    public int index;
    private int score;
    private void OnEnable()
    {
        score = GameManager.Instance.GetScoreAtIndex(index);
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
