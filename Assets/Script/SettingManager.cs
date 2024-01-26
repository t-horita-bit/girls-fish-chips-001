using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Audio;

public class SettingManager : MonoBehaviour
{
    public AudioSource bgmSource;
    public AudioSource seSource;
    public Slider bgmSlider;
    public Slider seSlider;
    public GameObject settingsPanel;
    public CanvasGroup settingCanvasGroup;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip okSound;
    [SerializeField] private AudioClip cancelSound;
    [Header("Audio Effects")]
    public AudioMixer audioMixer;

    public static SettingManager Instance;

    private const string BGM_VOLUME_KEY = "BGMVolume";
    private const string SE_VOLUME_KEY = "SEVolume";

    private void Awake()
    {
        InitializeVolume();

        // Singleton implementation
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        bgmSlider.onValueChanged.AddListener(SetBgmVolume);
        seSlider.onValueChanged.AddListener(SetSeVolume);

        bgmSlider.value = bgmSource.volume;
        seSlider.value = seSource.volume;
    }

    private void InitializeVolume()
    {
        if (PlayerPrefs.HasKey(BGM_VOLUME_KEY))
        {
            bgmSource.volume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY);
        }
        else
        {
            PlayerPrefs.SetFloat(BGM_VOLUME_KEY, bgmSource.volume);
        }

        if (PlayerPrefs.HasKey(SE_VOLUME_KEY))
        {
            seSource.volume = PlayerPrefs.GetFloat(SE_VOLUME_KEY);
        }
        else
        {
            PlayerPrefs.SetFloat(SE_VOLUME_KEY, seSource.volume);
        }
    }

    public void SetBgmVolume(float volume)
    {
        bgmSource.volume = volume;
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, volume);
    }

    public void SetSeVolume(float volume)
    {
        seSource.volume = volume;
        PlayerPrefs.SetFloat(SE_VOLUME_KEY, volume);
    }

    public void ToggleSettings()
    {
        audioSource.PlayOneShot(okSound);
        Invoke("ActivateSettingPanel", 0.3f);
        SetLowPassEffect(true);
    }

    public void ActivateSettingPanel()
    {        
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    public void OnCloseSettingPanelClicked()
    {
        audioSource.PlayOneShot(cancelSound);
        Invoke("FadeOutSettingPanel", 0.3f);
    }

    private void FadeOutSettingPanel()
    {
        SetLowPassEffect(false);
        settingCanvasGroup.DOFade(0, 0.3f)  // 0.3秒でアルファを0にする
            .OnComplete(() => {
                settingsPanel.SetActive(false);
                settingCanvasGroup.alpha = 1;  // アルファを元に戻す
            });
    }

    private void SetLowPassEffect(bool apply)
    {
        if (apply)
        {
            // LowPassエフェクトのカットオフ周波数を300Hzに設定
            audioMixer.SetFloat("BGMGroup_LowPass_Cutoff", 22000f);
        }
        else
        {
            // LowPassエフェクトをバイパスするために、カットオフ周波数を元の値（例：22000Hz）に設定
            audioMixer.SetFloat("BGMGroup_LowPass_Cutoff", 22000f);
        }
    }

    public float GetBgmVolume()
    {
        if (PlayerPrefs.HasKey(BGM_VOLUME_KEY))
        {
            return PlayerPrefs.GetFloat(BGM_VOLUME_KEY);
        }
        else
        {
            return 1f; // またはデフォルトのボリューム値
        }
    }

    public float GetSeVolume()
    {
        if (PlayerPrefs.HasKey(SE_VOLUME_KEY))
        {
            return PlayerPrefs.GetFloat(SE_VOLUME_KEY);
        }
        else
        {
            return 1f; // またはデフォルトのボリューム値
        }
    }

}
