using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIFadeIn : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button[] myButtons;  // UI�{�^���̔z����Q��

    private void Start()
    {
        canvasGroup.alpha = 0;  // ���������x��0�ɐݒ�

        // �{�^���𖳌��ɂ���
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
            .SetEase(Ease.Linear)  // ���̑��x�Ńt�F�[�h���s���悤�ɐݒ�
            .OnComplete(() =>
            {
                // �t�F�[�h�C��������Ƀ{�^����L���ɂ���
                foreach (var button in myButtons)
                {
                    button.interactable = true;
                }
            });
    }
}
