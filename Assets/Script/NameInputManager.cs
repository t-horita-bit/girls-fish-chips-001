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
        // FirebaseManagerのインスタンスからデータベースのリファレンスを取得

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
            // 空白の場合、ランダムな名前を設定してゲームを開始
            SetRandomName();
            inputName = nameInputField.text.Trim();
        }

        databaseReference.Child("scores").GetValueAsync().ContinueWith(task =>
        {
            // メインスレッドで実行するための処理を追加
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
                        // 同じデバイスIDなら名前のチェックをスキップ
                        continue;
                    }

                    // 名前が存在し、デバイスIDが異なる場合
                    if (existingPlayerName == inputName)
                    {
                        nameExistsWithDifferentDeviceID = true;
                        break;
                    }
                }

                if (nameExistsWithDifferentDeviceID)
                {
                    audioSource.PlayOneShot(cancelSound); // エラー音を再生
                    ShowErrorMessage("すでにほかのユーザーが使用している名前です。"); // エラーメッセージを表示
                    return;
                }
                else
                {
                    audioSource.PlayOneShot(gameStartSound); // ここで即座にゲーム開始音を再生
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
        nameCanvasGroup.DOFade(0, 0.3f)  // 0.3秒でアルファを0にする
            .OnComplete(() => {
                inputPanel.SetActive(false);
                nameCanvasGroup.alpha = 1;  // アルファを元に戻す
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
        // PlayerPrefsや他のストレージ方法を使って、ゲーム内でプレイヤー名を保持できます。
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

    private void ShowErrorMessage(string message)
    {
        if (errorMessageTween != null && errorMessageTween.IsActive())
        {
            errorMessageTween.Kill();  // 前回のTweenをキャンセル
        }

        errorMessageText.text = message;
        Color initialColor = errorMessageText.color;
        initialColor.a = 1;
        errorMessageText.color = initialColor;

        errorMessageTween = DOTween.To(() => errorMessageText.color, x => errorMessageText.color = x, new Color(initialColor.r, initialColor.g, initialColor.b, 0), 3f);
    }

}
