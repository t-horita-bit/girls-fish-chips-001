using UnityEngine;
using DG.Tweening;

public class PanelController : MonoBehaviour
{
    void OnEnable()
    {
        transform.localScale = Vector3.zero;
        ShowWindow();
    }
    /*
    void Awake()
    {
        transform.localScale = Vector3.zero;
        ShowWindow();
    }*/

    void ShowWindow()
    {
        transform.DOScale(1f, 1f).SetEase(Ease.OutBounce);
    }
}