using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class TransitionManager : Singleton<TransitionManager>
{
    private CanvasGroup canvasGroup;
    public float scaler;

    protected override void Awake()
    {
        base.Awake();
        canvasGroup = GetComponent<CanvasGroup>();
        DontDestroyOnLoad(this);
    }
    public void Transition(string sceneName)
    {
        StartCoroutine(TransitionToScene(sceneName));
    }
    public void Transition(Action action)
    {
        StartCoroutine(TransitionSelf(action));
    }
    private IEnumerator TransitionSelf(Action restartFunction)
    {
        yield return Fade(1);
        restartFunction();
        yield return Fade(0);
    }

    private IEnumerator TransitionToScene(string sceneName)
    {
        yield return Fade(1);
        yield return SceneManager.LoadSceneAsync(sceneName);
        yield return Fade(0);
    }

    private IEnumerator Fade(int target)
    {
        canvasGroup.blocksRaycasts = true;
        while (canvasGroup.alpha != target)
        {
            switch (target)
            {
                case 0:
                    canvasGroup.alpha -= Time.deltaTime * scaler;
                    break;
                case 1:
                    canvasGroup.alpha += Time.deltaTime * scaler;
                    break;
            }
            yield return null;
        }
        canvasGroup.blocksRaycasts = false;

    }
}
