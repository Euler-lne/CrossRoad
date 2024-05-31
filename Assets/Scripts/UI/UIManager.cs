using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text text;
    public GameObject gameOverBG;
    private void Start()
    {
        text.text = "00";
    }
    private void OnEnable()
    {
        EventHandler.FrogJumpSucceed += OnFrogJumpSucceed;
        EventHandler.FrogDead += OnFrogDead;
    }
    private void OnDisable()
    {
        EventHandler.FrogJumpSucceed -= OnFrogJumpSucceed;
        EventHandler.FrogDead -= OnFrogDead;
    }


    private void OnFrogDead()
    {
        gameOverBG.SetActive(true);
    }

    private void OnFrogJumpSucceed(int score)
    {
        text.text = score.ToString();
    }
}
