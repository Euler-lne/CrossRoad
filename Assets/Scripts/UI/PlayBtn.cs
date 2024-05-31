using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayBtn : MonoBehaviour
{
    private Button button;
    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(StartGame);
    }
    private void OnDisable()
    {
        button.onClick.RemoveListener(StartGame);
    }

    private void StartGame()
    {
        TransitionManager.Instance.Transition("Gameplay");
    }
}
