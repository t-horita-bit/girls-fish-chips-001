using UnityEngine;
using DG.Tweening;

public class logoController : MonoBehaviour
{
    RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector3(-152, 300, 0);
        Invoke("ShowWindow", 1f);
    }

    void ShowWindow()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(rectTransform.DOLocalMoveY(144f, 1f).SetEase(Ease.OutBounce))
                .Append(rectTransform.DOLocalMoveX(0f, 1f)); // 0.5秒はX軸の移動時間。適宜調整してください。

        sequence.Play();
    }
}
