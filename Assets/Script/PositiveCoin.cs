using UnityEngine;

public class PositiveCoin : MonoBehaviour
{
    public int scoreValue = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �v���C���[�^�O�̃I�u�W�F�N�g�Ƃ̏Փ˂����o���܂��B"Player"�̕����͎��ۂ̃v���C���[�I�u�W�F�N�g�̃^�O�ɍ��킹�ĕύX���Ă��������B
        if (collision.CompareTag("Player"))
        {
            // �X�R�A�����Z
            ScoreManager.Instance.AddScore(scoreValue);

            // �R�C����j��
            gameObject.SetActive(false);
        }
    }
}
