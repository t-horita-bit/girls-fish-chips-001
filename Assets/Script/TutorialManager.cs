using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Audio;

public class TutorialManager : MonoBehaviour
{
    public GameObject[] pages; // 3ページ分のGameObjectをInspectorから関連付け
    public Button prevButton;
    public Button nextButton;
    public Text pageNumberText; // ページ数表示用のテキスト
    private int currentPage = 0;
    public GameObject tutorialPanel;
    public CanvasGroup tutorialCanvasGroup;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip okSound;
    [SerializeField] private AudioClip cancelSound;
    [SerializeField] private AudioClip flipSound;
    [Header("Audio Effects")]
    public AudioMixer audioMixer;

    private void Start()
    {
        UpdatePages();
        SetLowPassEffect(false);
    }

    public void NextPage()
    {
        if (currentPage < pages.Length - 1)
        {

            audioSource.PlayOneShot(flipSound);
            currentPage++;
            UpdatePages();
        }
    }

    public void PrevPage()
    {
        if (currentPage > 0)
        {

            audioSource.PlayOneShot(flipSound);
            currentPage--;
            UpdatePages();
        }
    }

    void UpdatePages()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(i == currentPage);
        }

        prevButton.interactable = currentPage != 0;
        nextButton.interactable = currentPage != pages.Length - 1;

        // ページ数の更新
        pageNumberText.text = (currentPage + 1) + "/" + pages.Length;
    }

    public void OnCloseTutorialPanelClicked()
    {
        audioSource.PlayOneShot(cancelSound);
        SetLowPassEffect(false);
        Invoke("FadeOutTutorialPanel", 0.3f);
    }

    private void FadeOutTutorialPanel()
    {
        tutorialCanvasGroup.DOFade(0, 0.3f)  // 0.3秒でアルファを0にする
            .OnComplete(() => {
                currentPage = 0;               // これを追加
                UpdatePages();
                tutorialPanel.SetActive(false);
                tutorialCanvasGroup.alpha = 1;  // アルファを元に戻す
            });
    }

    public void ShowTutorial()
    {
        //currentPage = 0;
        SetLowPassEffect(true);
        audioSource.PlayOneShot(okSound);
        Invoke("OpenPanelTutorial", 0.3f);
    }

    public void OpenPanelTutorial()
    {
        tutorialPanel.SetActive(true);
    }

    private float currentCutoffFrequency = 22000f;  // 初期カットオフ周波数を格納
    private float currentMakeupGain = 0f;


    private void SetLowPassEffect(bool apply)
    {
        float targetFrequency = apply ? 1000f : 22000f;  // 目標とするカットオフ周波数を設定
        float targetMakeupGain = apply ? 5f : 0f;      // Makeup Gainの目標値を設定

        // DOTweenを使用してカットオフ周波数を徐々に変更
        DOTween.To(
            () => currentCutoffFrequency,
            x => {
                currentCutoffFrequency = x;
                audioMixer.SetFloat("BGMGroup_LowPass_Cutoff", x);
            },
            targetFrequency,
            1f
        );

        // DOTweenを使用してCompressorのMakeup Gainを徐々に変更
        DOTween.To(
            () => currentMakeupGain,        // 現在のMakeup Gainを取得するための新しい変数
            x => {
                currentMakeupGain = x;      // 現在のMakeup Gainを更新
                audioMixer.SetFloat("BGMGroup_Compressor_MakeupGain", x);  // Makeup GainをAudioMixerに設定
            },
            targetMakeupGain,
            1f
        );
    }


}
