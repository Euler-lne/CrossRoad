using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeadBord : MonoBehaviour
{
    public GameObject bord;
    public bool isOnline;
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
    private void OnDestroy()
    {
        button.onClick.RemoveListener(ClickLeadBord);
    }

    private void ClickLeadBord()
    {
        if (isOnline) { PlayfabManager.Instance.GetLeaderboardData(); }
        isActive = isOpen;
        bord.SetActive(isActive);
    }
}
