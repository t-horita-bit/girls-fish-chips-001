using UnityEngine;
using System.Collections;

public class SoundController : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] AudioSource seSource;
    [SerializeField] AudioSource playerSource;
    [SerializeField] AudioSource jumpSource;
    [SerializeField] AudioSource ambientSource;
    public float pitchDecreaseSpeed = 0.5f; // pitchをどれだけ速く下げるか

    private const string BGM_VOLUME_KEY = "BGMVolume";
    private const string SE_VOLUME_KEY = "SEVolume";

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.volume = GetBgmVolume();
        float seVolume = GetSeVolume(); // SE音量を一度だけ取得して、複数の音源に適用する
        seSource.volume = seVolume;
        playerSource.volume = seVolume;
        jumpSource.volume = seVolume;
        ambientSource.volume = seVolume;
    }

    public void PlayMusic()
    {
        audioSource.Play();
    }

    public void GameOver()
    {
        StartCoroutine(DecreasePitch());
    }

    private IEnumerator DecreasePitch()
    {
        while (audioSource.pitch > 0f) // pitchは0より大きくないといけないため、0.1まで下げる
        {
            audioSource.pitch -= pitchDecreaseSpeed * Time.deltaTime;
            yield return null;
        }
    }

    private float GetBgmVolume()
    {
        if (PlayerPrefs.HasKey(BGM_VOLUME_KEY))
        {
            return PlayerPrefs.GetFloat(BGM_VOLUME_KEY);
        }
        else
        {
            return 1f; // デフォルトのボリューム値
        }
    }

    private float GetSeVolume()
    {
        if (PlayerPrefs.HasKey(SE_VOLUME_KEY))
        {
            return PlayerPrefs.GetFloat(SE_VOLUME_KEY);
        }
        else
        {
            return 1f; // デフォルトのボリューム値
        }
    }
}
