using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingText : MonoBehaviour
{
    private Text text;
    private Animator animator;
    private bool isSetText;
    private void Awake()
    {
        text = GetComponent<Text>();
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        text.text = "等待网络连接";
        animator.enabled = true;
        isSetText = false;
    }
    private void OnEnable()
    {
        EventHandler.NetworkError += OnNetworkError;
        text.text = "等待网络连接";
        text.color = new Color(1, 1, 1, 0);
        animator.enabled = true;
    }
    private void OnDisable()
    {
        EventHandler.NetworkError -= OnNetworkError;
    }

    private void OnNetworkError()
    {
        animator.enabled = false;
        text.color = Color.white;
        if (!isSetText)
        {
            text.text = "网络连接错误";
        }
    }
    public void ChangeFianlInfo(string fianlInfo)
    {
        text.text = fianlInfo;
        isSetText = true;
    }

    public void ResetFainlInfo() { text.text = "网络连接错误"; isSetText = false; }
}
