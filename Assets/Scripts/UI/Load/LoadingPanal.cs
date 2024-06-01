using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanal : MonoBehaviour
{
    public Button button;
    private void OnEnable()
    {
        button.onClick.AddListener(OnClickCancel);
    }
    private void OnDisable()
    {
        button.onClick.RemoveListener(OnClickCancel);
    }

    private void OnClickCancel()
    {
        gameObject.SetActive(false);
    }
}
