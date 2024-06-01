using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoginBtn : MonoBehaviour
{
    private Button button;
    public Button confirm;
    public Button cancel;
    public GameObject nameInputPanel;
    public Text infoText;
    public Text inputFiled;
    public GameObject loading;
    public Text networkFianlInfo;
    private string info;
    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(OnLoginBtn);
        confirm.onClick.AddListener(OnConfirm);
        cancel.onClick.AddListener(OnCancel);
    }
    private void OnDisable()
    {
        button.onClick.RemoveListener(OnLoginBtn);
        confirm.onClick.RemoveListener(OnConfirm);
        cancel.onClick.RemoveListener(OnCancel);
    }

    private async void OnConfirm()
    {
        if (inputFiled.text == string.Empty) return;
        loading.SetActive(true);
        try
        {
            await PlayfabManager.Instance.SubmitNameAsync(inputFiled.text);
            StartCoroutine(SetNetworkInfo());
            GameManager.Instance.isOffLine = false;
        }
        catch
        {
            StartCoroutine(WaitForTextDisplay());
        }
    }

    private void OnCancel()
    {
        nameInputPanel.SetActive(false);
    }

    private void OnLoginBtn()
    {
        if (PlayfabManager.Instance.IsLogin())
        {
            infoText.text = "已登录 输入可更名";
            info = "更改名字成功";
        }
        else
        {
            infoText.text = "输入你的名字";
            info = "登录成功";
        }
        nameInputPanel.SetActive(true);
    }

    private IEnumerator WaitForTextDisplay()
    {
        EventHandler.CallNetworkError();
        yield return new WaitForSeconds(Settings.NetworkInfoDuration);
        loading.SetActive(false);
        nameInputPanel.SetActive(false);
    }

    private IEnumerator SetNetworkInfo()
    {
        networkFianlInfo.GetComponent<LoadingText>().ChangeFianlInfo(info);
        EventHandler.CallNetworkError();
        yield return new WaitForSeconds(Settings.NetworkInfoDuration);
        loading.SetActive(false);
        nameInputPanel.SetActive(false);
        networkFianlInfo.GetComponent<LoadingText>().ResetFainlInfo();
    }
}
