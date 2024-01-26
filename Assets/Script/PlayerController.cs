using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    public AudioClip coinCollectSound;  // �R�C���擾���̃T�E���h
    public AudioClip trapCollectSound;
    public AudioClip lifeCollectSound;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))  // "Coin"�̓R�C���̃I�u�W�F�N�g�̃^�O���ł��B���ۂ̃Q�[���ɍ��킹�ĕύX���Ă��������B
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
