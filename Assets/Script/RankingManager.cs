using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.Audio;

public class RankingManager : MonoBehaviour
{
    [SerializeField] private GameObject rankingEntryPrefab;
    [SerializeField] private Transform rankingContent;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private GameObject rankingPanel;
    [SerializeField] private Text currentPageText;
    [SerializeField] private CanvasGroup rankingCanvasGroup;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip okSound;
    [SerializeField] private AudioClip cancelSound;
    [SerializeField] private AudioClip flipSound;
    public AudioMixer audioMixer;

    [Header("Loading UI")]
    [SerializeField] private GameObject loadingPanel;

    private DatabaseReference databaseReference;
    private int currentPage = 0;
    private const int entriesPerPage = 5;
    private List<UserScoreRanking> allScores = new List<UserScoreRanking>();


    private void Start()
    {
        SetLowPassEffect(false);
        Debug.Log("Start method in RankingManager");
        
        if (databaseReference == null)
        {
            //Debug.LogError("databaseReference is null in Start of RankingManager");
        }
    }

    public void OnRankingButtonClicked()
    {
        audioSource.PlayOneShot(okSound);
        Invoke("ActivateRankingPanel", 0.3f);
        SetLowPassEffect(true);
    }

    public void ActivateRankingPanel()
    {
        currentPage = 0;
        rankingPanel.SetActive(true);
        loadingPanel.SetActive(true);
        LoadRanking();
    }

    private void LoadRanking()
    {
        databaseReference = FirebaseManager.Instance.GetDatabaseReference();
        if (databaseReference == null)
        {
            Debug.LogError("databaseReference is null in LoadRanking");
            return;
        }
        databaseReference.Child("scores").OrderByChild("score").GetValueAsync().ContinueWith(task =>
        {
            MainThreadDispatcher.Enqueue(() => {
                if (task.IsFaulted)
                {
                    Debug.LogError("Failed to retrieve data: " + task.Exception);
                    loadingPanel.SetActive(false);
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;

                    allScores.Clear();
                    foreach (var child in snapshot.Children)
                    {
                        UserScoreRanking userScoreRanking = JsonUtility.FromJson<UserScoreRanking>(child.GetRawJsonValue());
                        allScores.Add(userScoreRanking);
                    }

                    allScores.Sort((a, b) => b.score.CompareTo(a.score));
                    DisplayRankingPage(0);
                    loadingPanel.SetActive(false);
                }
            });
        });
    }

    private void DisplayRankingPage(int pageNumber)
    {
        // Remove old entries
        foreach (Transform child in rankingContent)
        {
            Destroy(child.gameObject);
        }

        int startIndex = pageNumber * entriesPerPage;
        int endIndex = Mathf.Min(startIndex + entriesPerPage, allScores.Count);

        for (int i = startIndex; i < endIndex; i++)
        {
            GameObject entryObj = Instantiate(rankingEntryPrefab, rankingContent);
            entryObj.transform.Find("Rank").GetComponent<Text>().text = GetOrdinalSuffix(i + 1);
            entryObj.transform.Find("Username").GetComponent<Text>().text = allScores[i].playerName;
            entryObj.transform.Find("Score").GetComponent<Text>().text = allScores[i].score.ToString()+" pt";
        }

        prevButton.interactable = pageNumber > 0;
        nextButton.interactable = pageNumber < Mathf.CeilToInt((float)allScores.Count / entriesPerPage) - 1;
        currentPageText.text = (pageNumber + 1) + " / " + Mathf.CeilToInt((float)allScores.Count / entriesPerPage).ToString();

    }

    public void OnPrevButtonClicked()
    {
        audioSource.PlayOneShot(flipSound);
        currentPage--;
        DisplayRankingPage(currentPage);
    }

    public void OnNextButtonClicked()
    {
        audioSource.PlayOneShot(flipSound);
        currentPage++;
        DisplayRankingPage(currentPage);
    }

    public void OnCloseRankingPanelClicked()
    {
        audioSource.PlayOneShot(cancelSound);
        Invoke("FadeOutRankingPanel", 0.3f);
        SetLowPassEffect(false);
    }

    private void FadeOutRankingPanel()
    {
        rankingCanvasGroup.DOFade(0, 0.3f)  // 0.3秒でアルファを0にする
            .OnComplete(() => {
                rankingPanel.SetActive(false);
                rankingCanvasGroup.alpha = 1;  // アルファを元に戻す
            });
    }


    private string GetOrdinalSuffix(int number)
    {
        if (number <= 0) return number.ToString();

        switch (number % 100)
        {
            case 11:
            case 12:
            case 13:
                return number + "th";
        }

        switch (number % 10)
        {
            case 1:
                return number + "st";
            case 2:
                return number + "nd";
            case 3:
                return number + "rd";
            default:
                return number + "th";
        }
    }

    private float currentCutoffFrequency = 22000f;  // 初期カットオフ周波数を格納
    private float currentMakeupGain = 0f;


    private void SetLowPassEffect(bool apply)
    {
        float targetFrequency = apply ? 1000f : 22000f;  // 目標とするカットオフ周波数を設定
        float targetMakeupGain = apply ? 5f : 0f;      // Makeup Gainの目標値を設定

        // DOTweenを使用してカットオフ周波数を徐々に変更
        DOTween.To(
            () => currentCutoffFrequency,
            x => {
                currentCutoffFrequency = x;
                audioMixer.SetFloat("BGMGroup_LowPass_Cutoff", x);
            },
            targetFrequency,
            1f
        );

        // DOTweenを使用してCompressorのMakeup Gainを徐々に変更
        DOTween.To(
            () => currentMakeupGain,        // 現在のMakeup Gainを取得するための新しい変数
            x => {
                currentMakeupGain = x;      // 現在のMakeup Gainを更新
                audioMixer.SetFloat("BGMGroup_Compressor_MakeupGain", x);  // Makeup GainをAudioMixerに設定
            },
            targetMakeupGain,
            1f
        );
    }

}

[System.Serializable]
public class UserScoreRanking
{
    public string deviceID;
    public string playerName;
    public int score;
}
