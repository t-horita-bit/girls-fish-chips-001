using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject coinSpawner;

    [SerializeField]
    private Button startButton;

    [SerializeField]
    private Text countdownText; // Reference to the countdown Text

    public GameObject fullHeartPrefab;  // ♥ のプレハブ
    public GameObject emptyHeartPrefab; // ♡ のプレハブ
    public Transform lifeContainer;     // ライフのUIを表示する場所
    private int currentLives = 3;
    private GameObject[] lifeImages;
    public static GameManager Instance;
    [SerializeField]
    private PlayerMove playerMove;

    [SerializeField]
    private ScoreManager scoreManager;

    [SerializeField]
    private GameObject gameOverPanel;

    [SerializeField]
    private SoundController soundController;

    [SerializeField]
    private PlayerAnimation playerAnimation;

    [SerializeField]
    private Text finalScoreText;


    [SerializeField]
    private Text previousRankText;
    [SerializeField]
    private Text currentRankText;
    [SerializeField]
    private Text nextRankText;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip okSound;
    [SerializeField] private AudioClip cancelSound;
    [SerializeField] private AudioClip flipSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayerAnimation.OnDeathAnimationComplete += HandleDeathAnimationComplete;
        lifeImages = new GameObject[currentLives];
        for (int i = 0; i < currentLives; i++)
        {
            lifeImages[i] = Instantiate(fullHeartPrefab, lifeContainer);
        }
    }

    public void OnStartButtonPressed()
    {
        Invoke("StartingProcess", 0.3f);
        audioSource.PlayOneShot(okSound);
    }

    public void StartingProcess()
    {
        startButton.gameObject.SetActive(false); // Hide the button
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        countdownText.text = "3";
        yield return new WaitForSeconds(1f);
        countdownText.text = "2";
        yield return new WaitForSeconds(1f);
        countdownText.text = "1";
        yield return new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(false); // Hide the countdown
        coinSpawner.SetActive(true);
        soundController.PlayMusic();
    }

    public void IncreaseLife()
    {
        if (currentLives < lifeImages.Length)
        {
            currentLives++;
            UpdateLifeUI();

            // ライフが最大に達したら、全ての回復アイテムを非表示にする
            if (currentLives == lifeImages.Length)
            {
                foreach (var item in FindObjectOfType<CoinSpawner>().lifeRecoveryPool)
                {
                    if (item.activeInHierarchy)
                    {
                        item.SetActive(false);
                    }
                }
            }
        }
    }

    public void DecreaseLife()
    {
        if (currentLives > 0)
        {
            currentLives--;
            UpdateLifeUI();

            if (currentLives == 0)
            {
                playerMove.moveSpeed = 0;
                scoreManager.SaveScoreToFirebase();
                gameOverPanel.SetActive(true);
                soundController.GameOver();
                playerAnimation.PlayerDead(true);

                // スコアとランキングのテキストを更新
                finalScoreText.text = $"Your Score: {scoreManager.currentScore}";

                // 前後のランキング情報を更新
                scoreManager.RetrieveTopScoresAroundCurrentScore(scoreManager.currentScore, (surroundingScores, yourActualRank) =>
                {
                    UnityEngine.Debug.Log("Processing surrounding scores...");

                    if (surroundingScores != null && surroundingScores.Count > 0)
                    {
                        UnityEngine.Debug.Log($"Retrieved {surroundingScores.Count} scores from database.");

                        // あなたのスコアの実際の順位を使用
                        int currentIndex = surroundingScores.FindIndex(s => s.playerName == "あなた");

                        // 前のスコアが存在する場合
                        if (currentIndex > 0)
                        {
                            int previousRank = yourActualRank - 1;
                            previousRankText.text = $"{FormatRank(previousRank)} - {surroundingScores[currentIndex - 1].playerName} - {surroundingScores[currentIndex - 1].score}";
                        }

                        // あなたのスコア
                        if (currentIndex != -1)
                        {
                            currentRankText.text = $"{FormatRank(yourActualRank)} - {surroundingScores[currentIndex].playerName} - {surroundingScores[currentIndex].score}";
                        }

                        // 次のスコアが存在する場合
                        if (currentIndex + 1 < surroundingScores.Count)
                        {
                            int nextRank = yourActualRank + 1;
                            nextRankText.text = $"{FormatRank(nextRank)} - {surroundingScores[currentIndex + 1].playerName} - {surroundingScores[currentIndex + 1].score}";
                        }

                        UnityEngine.Debug.Log("Ranking display updated.");
                    }
                    else
                    {
                        UnityEngine.Debug.Log("Failed to retrieve scores from database.");
                    }
                });





            }
        }
    }

    private void UpdateLifeUI()
    {
        // 現在の全てのハートを破壊
        foreach (var heart in lifeImages)
        {
            Destroy(heart);
        }

        for (int i = 0; i < lifeImages.Length; i++)
        {
            if (i < currentLives)
            {
                // フルハートを生成
                lifeImages[i] = Instantiate(fullHeartPrefab, lifeContainer);
            }
            else
            {
                // 空のハートを生成
                lifeImages[i] = Instantiate(emptyHeartPrefab, lifeContainer);
            }
        }
    }






    public void OnRetryButtonPressed()
    {
        Invoke("ReloadThisSnece", 0.3f);
        audioSource.PlayOneShot(okSound);
    }

    public void ReloadThisSnece()
    {
        //gameOverPanel.SetActive(false);
        SceneTransition.Instance.ChangeSceneWithFade("GamePlay");
    }

    public void OnTitleButtonPressed()
    {
        Invoke("LoadTitleScene", 0.3f);
        audioSource.PlayOneShot(okSound);
    }

    public void LoadTitleScene()
    {
        SceneTransition.Instance.ChangeSceneWithFade("Title");
    }

    public int GetCurrentLives()
    {
        return currentLives;
    }

    public int GetMaxLives()
    {
        return lifeImages.Length;
    }

    private void OnDestroy()
    {
        // イベントの購読を解除
        PlayerAnimation.OnDeathAnimationComplete -= HandleDeathAnimationComplete;
    }

    private void HandleDeathAnimationComplete()
    {
        // アニメーションが終了したら、プレイヤーを非表示にする
        playerMove.gameObject.SetActive(false);
    }

    // 他の部分に、以下の関数を追加
    string FormatRank(int rank)
    {
        switch (rank)
        {
            case 1: return "1st";
            case 2: return "2nd";
            case 3: return "3rd";
            default: return $"{rank}th";
        }
    }
}
