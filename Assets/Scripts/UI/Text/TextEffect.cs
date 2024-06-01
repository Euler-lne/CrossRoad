using UnityEngine;
using DG.Tweening;
using TMPro;

public class TextEffect : MonoBehaviour
{
    public TextMeshProUGUI tmp;

    void Start()
    {
        string text = tmp.text;
        var t = DOTween.To(() => string.Empty, value => tmp.text = value, text, 3f).SetEase(Ease.Linear);

        //富文本
        t.SetOptions(true);
    }
}
