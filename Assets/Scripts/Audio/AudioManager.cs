using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Mixer")]
    public AudioMixer mixer;
    [Header("Audio Clips")]
    public AudioClip bgmClip;
    public AudioClip jumpClip;
    public AudioClip longJumpClip;
    public AudioClip deadClip;
    [Header("Audio Source")]
    public AudioSource bgmMusic;
    public AudioSource fx;
    public bool isPlaying;
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        isPlaying = true;
    }
    private void OnEnable()
    {
        EventHandler.FrogDead += OnFrogDead;
    }
    private void OnDisable()
    {
        EventHandler.FrogDead -= OnFrogDead;
    }
    private void Start()
    {
        bgmMusic.clip = bgmClip;
        PlayMusic();
    }

    private void OnFrogDead()
    {
        fx.clip = deadClip;
        fx.Play();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type">0小跳，1大跳</param>
    public void SetJumpClip(int type)
    {
        fx.clip = type == 0 ? jumpClip : longJumpClip;
    }


    public void PlayJumpFx()
    {
        fx.Play();
    }

    public void PlayMusic()
    {
        if (!bgmMusic.isPlaying)
        {
            bgmMusic.Play();
        }
    }

}
