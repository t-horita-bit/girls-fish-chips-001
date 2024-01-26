using UnityEngine;

public class PositiveCoin : MonoBehaviour
{
    public int scoreValue = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // プレイヤータグのオブジェクトとの衝突を検出します。"Player"の部分は実際のプレイヤーオブジェクトのタグに合わせて変更してください。
        if (collision.CompareTag("Player"))
        {
            // スコアを加算
            ScoreManager.Instance.AddScore(scoreValue);

            // コインを破壊
            gameObject.SetActive(false);
        }
    }
}
