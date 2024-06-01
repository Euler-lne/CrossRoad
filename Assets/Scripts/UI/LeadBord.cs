using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeadBord : MonoBehaviour
{
    public GameObject bord;
    public bool isOnline;
    public GameObject loadingObj;
    public Text rankText;
    public bool isOpen;
    private Button button;
    private bool isActive;
    private void Awake()
    {
        button = GetComponent<Button>();
    }
    private void OnEnable()
    {
        button.onClick.AddListener(ClickLeadBord);
    }
    private void OnDisable()
    {
        button.onClick.RemoveListener(ClickLeadBord);
    }

    private async void ClickLeadBord()
    {
        bool isSuccess = !isOnline;
        if (isOnline)
        {
            loadingObj.SetActive(true);
            try
            {
                await PlayfabManager.Instance.GetLeaderboardDataAsync();
                int rank = await PlayfabManager.Instance.GetUserRankAsync();
                SetRankText(rank);
                isSuccess = true;
            }
            catch
            {
                StartCoroutine(WaitForTextDisplay());
            }
            if (isSuccess)
                loadingObj.SetActive(false);
        }
        if (isSuccess)
        {
            isActive = isOpen;
            bord.SetActive(isActive);
        }
    }

    private IEnumerator WaitForTextDisplay()
    {
        EventHandler.CallNetworkError();
        yield return new WaitForSeconds(Settings.NetworkInfoDuration);
        loadingObj.SetActive(false);
    }

    void SetRankText(int rank)
    {
        if (rank == -1)
        {
            rankText.text = "你没有登录";
        }
        string temp = "你是第" + rank.ToString() + "名";
    }
}
