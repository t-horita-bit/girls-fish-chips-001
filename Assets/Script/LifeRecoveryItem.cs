using UnityEngine;

public class LifeRecoveryItem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // "Player" �̓v���C���[�I�u�W�F�N�g�̃^�O���w�肵�܂��B
        // �����قȂ�^�O���g���Ă���ꍇ�́A"Player" �̕�����K�؂ȃ^�O�ɕύX���Ă��������B
        if (other.CompareTag("Player"))
        {
            // ���C�t����
            GameManager.Instance.IncreaseLife();

            // �A�C�e�����A�N�e�B�u��
            gameObject.SetActive(false);
        }
    }
}
