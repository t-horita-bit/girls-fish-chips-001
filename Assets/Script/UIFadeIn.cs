using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIFadeIn : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button[] myButtons;  // UIボタンの配列を参照

    private void Start()
    {
        canvasGroup.alpha = 0;  // 初期透明度を0に設定

        // ボタンを無効にする
        foreach (var button in myButtons)
        {
            button.interactable = false;
        }

        Invoke("FadeInUI", 1f);
    }

    private void FadeInUI()
    {
        canvasGroup.DOFade(1, 1f)
            .SetDelay(1f)
            .SetEase(Ease.Linear)  // 一定の速度でフェードを行うように設定
            .OnComplete(() =>
            {
                // フェードイン完了後にボタンを有効にする
                foreach (var button in myButtons)
                {
                    button.interactable = true;
                }
            });
    }
}
