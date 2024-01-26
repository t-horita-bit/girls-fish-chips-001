using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonController : MonoBehaviour
{
    void Awake()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        transform.DOScale(0.96f, 0.5f).SetEase(Ease.OutElastic)
            .OnComplete(() => transform.DOScale(1f, 0.5f)); // �A�j���[�V����������A���̃T�C�Y�ɖ߂铮����ǉ�
    }

}