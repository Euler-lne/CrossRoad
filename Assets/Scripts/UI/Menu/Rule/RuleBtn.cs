using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RuleBtn : MonoBehaviour
{
    public GameObject ruleObj;
    public bool isShow;
    private Button button;
    private void Awake()
    {
        button = GetComponent<Button>();
    }
    private void OnEnable()
    {
        button.onClick.AddListener(ClickRule);
    }
    private void OnDisable()
    {
        button.onClick.RemoveListener(ClickRule);
    }

    private void ClickRule()
    {
        ruleObj.SetActive(isShow);
    }
}
