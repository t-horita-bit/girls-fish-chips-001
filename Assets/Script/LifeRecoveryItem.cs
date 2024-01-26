using UnityEngine;

public class LifeRecoveryItem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // "Player" はプレイヤーオブジェクトのタグを指定します。
        // もし異なるタグを使っている場合は、"Player" の部分を適切なタグに変更してください。
        if (other.CompareTag("Player"))
        {
            // ライフを回復
            GameManager.Instance.IncreaseLife();

            // アイテムを非アクティブ化
            gameObject.SetActive(false);
        }
    }
}
