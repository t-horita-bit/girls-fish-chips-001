using UnityEngine;

public class TrapCoin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // ライフを減少
            GameManager.Instance.DecreaseLife();

            // コインを破壊
            gameObject.SetActive(false);
        }
    }
}
