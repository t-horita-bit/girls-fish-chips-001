using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Audio;

public class TutorialManager : MonoBehaviour
{
    public GameObject[] pages; // 3�y�[�W����GameObject��Inspector����֘A�t��
    public Button prevButton;
    public Button nextButton;
    public Text pageNumberText; // �y�[�W���\���p�̃e�L�X�g
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

        // �y�[�W���̍X�V
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
        tutorialCanvasGroup.DOFade(0, 0.3f)  // 0.3�b�ŃA���t�@��0�ɂ���
            .OnComplete(() => {
                currentPage = 0;               // �����ǉ�
                UpdatePages();
                tutorialPanel.SetActive(false);
                tutorialCanvasGroup.alpha = 1;  // �A���t�@�����ɖ߂�
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

    private float currentCutoffFrequency = 22000f;  // �����J�b�g�I�t���g�����i�[
    private float currentMakeupGain = 0f;


    private void SetLowPassEffect(bool apply)
    {
        float targetFrequency = apply ? 1000f : 22000f;  // �ڕW�Ƃ���J�b�g�I�t���g����ݒ�
        float targetMakeupGain = apply ? 5f : 0f;      // Makeup Gain�̖ڕW�l��ݒ�

        // DOTween���g�p���ăJ�b�g�I�t���g�������X�ɕύX
        DOTween.To(
            () => currentCutoffFrequency,
            x => {
                currentCutoffFrequency = x;
                audioMixer.SetFloat("BGMGroup_LowPass_Cutoff", x);
            },
            targetFrequency,
            1f
        );

        // DOTween���g�p����Compressor��Makeup Gain�����X�ɕύX
        DOTween.To(
            () => currentMakeupGain,        // ���݂�Makeup Gain���擾���邽�߂̐V�����ϐ�
            x => {
                currentMakeupGain = x;      // ���݂�Makeup Gain���X�V
                audioMixer.SetFloat("BGMGroup_Compressor_MakeupGain", x);  // Makeup Gain��AudioMixer�ɐݒ�
            },
            targetMakeupGain,
            1f
        );
    }


}
