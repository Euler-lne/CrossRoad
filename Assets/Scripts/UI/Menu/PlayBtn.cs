using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayBtn : MonoBehaviour
{
    private Button button;
    public Button confirm;
    public Button cancel;
    public GameObject nameInputPanel;
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
        button.onClick.AddListener(StartGame);
        confirm.onClick.AddListener(OnConfirm);
        cancel.onClick.AddListener(OnCancel);
    }
    private void OnDisable()
    {
        button.onClick.RemoveListener(StartGame);
        confirm.onClick.RemoveListener(OnConfirm);
        cancel.onClick.RemoveListener(OnCancel);
    }

    private async void OnConfirm()
    {
        if (inputFiled.text == string.Empty) return;
        loading.SetActive(true);
        try
        {
            Settings.LoginReturnType type = await PlayfabManager.Instance.SubmitNameAsync(inputFiled.text);
            GameManager.Instance.isOffLine = false;
            if (type == Settings.LoginReturnType.Success)
            {
                nameInputPanel.SetActive(false);
                TransitionManager.Instance.Transition("Gameplay");
            }
            else
            {
                if (type == Settings.LoginReturnType.SameName)
                {
                    info = "你输入的名字和之前相同";
                }
                else if (type == Settings.LoginReturnType.NameRepeat)
                {
                    info = "名字已经被占用";
                }
                StartCoroutine(SetNetworkInfo());
            }

        }
        catch
        {
            StartCoroutine(WaitForTextDisplay());
        }
    }

    private void OnCancel()
    {
        nameInputPanel.SetActive(false);
        GameManager.Instance.isOffLine = true;
        TransitionManager.Instance.Transition("Gameplay");
    }

    private void StartGame()
    {
        if (!GameManager.Instance.isOffLine && !PlayfabManager.Instance.IsUserNameInit())
        {
            nameInputPanel.SetActive(true);
        }
        else
        {
            nameInputPanel.SetActive(false);
            TransitionManager.Instance.Transition("Gameplay");
        }
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
