using UnityEngine;
using Firebase.Database;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using DG.Tweening;
using UnityEngine.Audio;


public class NameInputManager : MonoBehaviour
{
    public InputField nameInputField;
    private DatabaseReference databaseReference;
    public GameObject inputPanel;
    public CanvasGroup nameCanvasGroup;
    public Button startButton;
    private string deviceID;
    public SceneTransition sceneTransition;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip okSound;
    [SerializeField] private AudioClip cancelSound;
    [SerializeField] private AudioClip gameStartSound;
    [Header("Audio Effects")]
    public AudioMixer audioMixer;
    public Text errorMessageText;
    private Tween errorMessageTween;

    private void Start()
    {
        // FirebaseManager�̃C���X�^���X����f�[�^�x�[�X�̃��t�@�����X���擾

        SetLowPassEffect(false);

        deviceID = SystemInfo.deviceUniqueIdentifier;
        LoadPlayerNameToInputField();
    }

    public void OnStartButtonClicked()
    {
        audioSource.PlayOneShot(okSound);
        Invoke("activatePanel", 0.3f);
        SetLowPassEffect(true);
    }

    private void activatePanel()
    {
        inputPanel.SetActive(true);
    }

    public void OnGoButtonClicked()
    {
        StartGame();
    }

    public void StartGame()
    {
        databaseReference = FirebaseManager.Instance.GetDatabaseReference();
        string inputName = nameInputField.text.Trim();

        if (string.IsNullOrEmpty(inputName))
        {
            // �󔒂̏ꍇ�A�����_���Ȗ��O��ݒ肵�ăQ�[�����J�n
            SetRandomName();
            inputName = nameInputField.text.Trim();
        }

        databaseReference.Child("scores").GetValueAsync().ContinueWith(task =>
        {
            // ���C���X���b�h�Ŏ��s���邽�߂̏�����ǉ�
            MainThreadDispatcher.Enqueue(() => {
                if (task.IsFaulted)
                {
                    Debug.LogError("Failed to retrieve names: " + task.Exception);
                    return;
                }

                DataSnapshot snapshot = task.Result;
                bool nameExistsWithDifferentDeviceID = false;

                foreach (var child in snapshot.Children)
                {
                    string existingDeviceID = child.Child("deviceID").Value.ToString();
                    string existingPlayerName = child.Child("playerName").Value.ToString();

                    if (existingDeviceID == deviceID)
                    {
                        // �����f�o�C�XID�Ȃ疼�O�̃`�F�b�N���X�L�b�v
                        continue;
                    }

                    // ���O�����݂��A�f�o�C�XID���قȂ�ꍇ
                    if (existingPlayerName == inputName)
                    {
                        nameExistsWithDifferentDeviceID = true;
                        break;
                    }
                }

                if (nameExistsWithDifferentDeviceID)
                {
                    audioSource.PlayOneShot(cancelSound); // �G���[�����Đ�
                    ShowErrorMessage("���łɂق��̃��[�U�[���g�p���Ă��閼�O�ł��B"); // �G���[���b�Z�[�W��\��
                    return;
                }
                else
                {
                    audioSource.PlayOneShot(gameStartSound); // �����ő����ɃQ�[���J�n�����Đ�
                    Invoke("ExecuteStartGame", 0.3f);
                }
            });
        });

    }

    private void ExecuteStartGame()
    {
        string inputName = nameInputField.text.Trim();
        SetPlayerName(inputName);
        SceneTransition.Instance.ChangeSceneWithFade("GamePlay");
    }

    public void OnCloseNamePanelClicked()
    {
        SetLowPassEffect(false);
        audioSource.PlayOneShot(cancelSound);
        Invoke("FadeOutNamePanel", 0.3f);
    }

    private void FadeOutNamePanel()
    {
        nameCanvasGroup.DOFade(0, 0.3f)  // 0.3�b�ŃA���t�@��0�ɂ���
            .OnComplete(() => {
                inputPanel.SetActive(false);
                nameCanvasGroup.alpha = 1;  // �A���t�@�����ɖ߂�
            });
    }

    private void SetRandomName()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        string randomName = new string(Enumerable.Repeat(chars, 5)
          .Select(s => s[Random.Range(0, s.Length)]).ToArray());

        nameInputField.text = randomName;
    }


    private void SetPlayerName(string name)
    {
        // PlayerPrefs�⑼�̃X�g���[�W���@���g���āA�Q�[�����Ńv���C���[����ێ��ł��܂��B
        PlayerPrefs.SetString("PlayerName", name);
    }

    void LoadPlayerNameToInputField()
    {
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            string savedName = PlayerPrefs.GetString("PlayerName");
            nameInputField.text = savedName;
        }
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

    private void ShowErrorMessage(string message)
    {
        if (errorMessageTween != null && errorMessageTween.IsActive())
        {
            errorMessageTween.Kill();  // �O���Tween���L�����Z��
        }

        errorMessageText.text = message;
        Color initialColor = errorMessageText.color;
        initialColor.a = 1;
        errorMessageText.color = initialColor;

        errorMessageTween = DOTween.To(() => errorMessageText.color, x => errorMessageText.color = x, new Color(initialColor.r, initialColor.g, initialColor.b, 0), 3f);
    }

}
