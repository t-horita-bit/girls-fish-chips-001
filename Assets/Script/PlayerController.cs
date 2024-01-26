using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    public AudioClip coinCollectSound;  // コイン取得時のサウンド
    public AudioClip trapCollectSound;
    public AudioClip lifeCollectSound;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))  // "Coin"はコインのオブジェクトのタグ名です。実際のゲームに合わせて変更してください。
        {
            audioSource.PlayOneShot(coinCollectSound);
        }
        else if (other.CompareTag("Trap"))
        {
            audioSource.PlayOneShot(trapCollectSound);
        }
        else if (other.CompareTag("Life"))
        {
            audioSource.PlayOneShot(lifeCollectSound);
        }
    }
}
