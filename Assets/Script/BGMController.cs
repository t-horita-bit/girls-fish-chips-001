using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMController : MonoBehaviour
{
    // BGMの配列
    public AudioClip[] bgmClips;
    private AudioSource bgmAudioSource;

    private void Start()
    {
        bgmAudioSource = GetComponent<AudioSource>();
        PlayRandomBGM();
    }

    // ランダムなBGMを再生するメソッド
    public void PlayRandomBGM()
    {
        if (bgmClips.Length > 0)
        {
            int randomIndex = Random.Range(0, bgmClips.Length);
            bgmAudioSource.clip = bgmClips[randomIndex];
            bgmAudioSource.Play();
        }
    }
}
