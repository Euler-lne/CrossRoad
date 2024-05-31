using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameOverBg : MonoBehaviour
{
    public Button restartBnt;
    public Button backBnt;
    private void OnEnable()
    {
        restartBnt.onClick.AddListener(ClickRestart);
        backBnt.onClick.AddListener(ToMenu);
    }
    private void OnDisable()
    {
        restartBnt.onClick.RemoveListener(ClickRestart);
        backBnt.onClick.RemoveListener(ToMenu);
    }

    private void ToMenu()
    {
        TransitionManager.Instance.Transition("Menu");
    }

    private void ClickRestart()
    {
        // 重新加载就好了
        TransitionManager.Instance.Transition("Gameplay");
    }
}
