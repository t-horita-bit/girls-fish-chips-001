using UnityEngine;
using DG.Tweening;

public class QuitGame : MonoBehaviour
{
    public CanvasGroup canvasGroup1;
    public CanvasGroup canvasGroup2;
    public float fadeDuration = 1.0f; // �t�F�[�h�A�E�g�ɂ����鎞�ԁi�b�j
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip quitSound;
    [SerializeField] private AudioSource bgmAudioSource;

    public void ExitGame()
    {
        audioSource.PlayOneShot(quitSound);
        // 2��Canvas Group���t�F�[�h�A�E�g
        Sequence sequence = DOTween.Sequence();
        sequence.Append(canvasGroup1.DOFade(0, fadeDuration));
        sequence.Join(canvasGroup2.DOFade(0, fadeDuration));
        sequence.Join(bgmAudioSource.DOFade(0, fadeDuration));
        sequence.OnComplete(() =>
        {
            // �t�F�[�h�A�E�g��̏���
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit(); // �Q�[���̏I��
#endif
        });
    }

}
