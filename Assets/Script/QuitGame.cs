using UnityEngine;
using DG.Tweening;

public class QuitGame : MonoBehaviour
{
    public CanvasGroup canvasGroup1;
    public CanvasGroup canvasGroup2;
    public float fadeDuration = 1.0f; // フェードアウトにかかる時間（秒）
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip quitSound;
    [SerializeField] private AudioSource bgmAudioSource;

    public void ExitGame()
    {
        audioSource.PlayOneShot(quitSound);
        // 2つのCanvas Groupをフェードアウト
        Sequence sequence = DOTween.Sequence();
        sequence.Append(canvasGroup1.DOFade(0, fadeDuration));
        sequence.Join(canvasGroup2.DOFade(0, fadeDuration));
        sequence.Join(bgmAudioSource.DOFade(0, fadeDuration));
        sequence.OnComplete(() =>
        {
            // フェードアウト後の処理
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit(); // ゲームの終了
#endif
        });
    }

}
