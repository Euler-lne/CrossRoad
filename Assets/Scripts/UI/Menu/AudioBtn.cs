using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class AudioBtn : MonoBehaviour
{
    private Toggle toggle;
    private void Awake()
    {
        toggle = GetComponent<Toggle>();
    }
    private void Start()
    {
        toggle.isOn = AudioManager.Instance.isPlaying;
    }
    private void OnEnable()
    {
        toggle.onValueChanged.AddListener(ClickAudion);
    }

    private void OnDisable()
    {
        toggle.onValueChanged.RemoveListener(ClickAudion);
    }

    private void ClickAudion(bool isOn)
    {
        if (isOn)
        {
            AudioManager.Instance.isPlaying = true;
            AudioManager.Instance.mixer.SetFloat("masterVolume", 0);
        }
        else
        {
            AudioManager.Instance.isPlaying = false;
            AudioManager.Instance.mixer.SetFloat("masterVolume", -80);
            // -80是最小值
        }
    }
}
