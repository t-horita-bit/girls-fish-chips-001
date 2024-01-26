using UnityEngine;

public class TrapCoin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // ���C�t������
            GameManager.Instance.DecreaseLife();

            // �R�C����j��
            gameObject.SetActive(false);
        }
    }
}
