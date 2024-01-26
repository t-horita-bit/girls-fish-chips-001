using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.UI;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public int currentScore = 0;
    private DatabaseReference databaseReference;
    private string deviceID;
    [SerializeField] private Text scoreText;
    [SerializeField] public Text rankText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            FirebaseApp.Create();
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
            deviceID = SystemInfo.deviceUniqueIdentifier;
            //deviceID = "default_000000";
            SetupScoreChangeListener();
            CheckAndUpdatePlayerName();
        });
    }

    private void Initialize()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RetrieveRankingFromFirebase(10, snapshot => {
                foreach (var child in snapshot.Children)
                {
                    UserScore userScore = JsonUtility.FromJson<UserScore>(child.GetRawJsonValue());
                    UnityEngine.Debug.Log($"Device ID: {userScore.deviceID}, Player Name: {userScore.playerName}, Score: {userScore.score}");
                }
            });
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveScoreToFirebase();
            UnityEngine.Debug.Log("Score saved to Firebase.");
        }
    }

    public void AddScore(int score)
    {
        currentScore += score;
        UpdateScoreDisplay();
        UpdateRanking();
        UnityEngine.Debug.Log(currentScore);
    }

    private void UpdateRanking()
    {
        databaseReference.Child("scores").OrderByChild("score").GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                UnityEngine.Debug.LogError("Failed to retrieve data: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                List<UserScore> scores = new List<UserScore>();

                foreach (var child in snapshot.Children)
                {
                    UserScore userScore = JsonUtility.FromJson<UserScore>(child.GetRawJsonValue());
                    scores.Add(userScore);
                }

                scores.Sort((a, b) => b.score.CompareTo(a.score));

                int rank = 1;
                bool foundRank = false;

                foreach (var score in scores)
                {
                    if (currentScore >= score.score)
                    {
                        foundRank = true;
                        break;
                    }
                    rank++;
                }

                if (!foundRank)
                {
                    rank = scores.Count + 1;
                }

                UpdateRankDisplay(rank);
            }
        });
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {currentScore}";
    }

    private void UpdateRankDisplay(int rank)
    {
        rankText.text = $"Your Rank: {rank}";
    }

    public void SaveScoreToFirebase()
    {
        string localDeviceID = SystemInfo.deviceUniqueIdentifier;
        string localPlayerName = GetPlayerName();

        databaseReference.Child("scores").Child(localDeviceID).GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                UnityEngine.Debug.LogError("Failed to retrieve data: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot.Exists)
                {
                    UserScore existingScore = JsonUtility.FromJson<UserScore>(snapshot.GetRawJsonValue());

                    if (currentScore > existingScore.score)
                    {
                        UpdateScoreInFirebase(localDeviceID, localPlayerName);
                    }
                }
                else
                {
                    UpdateScoreInFirebase(localDeviceID, localPlayerName);
                }
            }
        });
    }

    private void UpdateScoreInFirebase(string localDeviceID, string localPlayerName)
    {
        UserScore userScore = new UserScore(localDeviceID, localPlayerName, currentScore);
        string json = JsonUtility.ToJson(userScore);
        databaseReference.Child("scores").Child(localDeviceID).SetRawJsonValueAsync(json);
    }

    public void RetrieveRankingFromFirebase(int topN, System.Action<DataSnapshot> callback)
    {
        databaseReference.Child("scores").OrderByChild("score").LimitToLast(topN).GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                callback(task.Result);
            }
        });
    }

    private string GetPlayerName()
    {
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            return PlayerPrefs.GetString("PlayerName");
        }
        return "Unknown";
    }

    private void CheckAndUpdatePlayerName()
    {
        string localPlayerName = GetPlayerNameFromPrefs();

        databaseReference.Child("scores").Child(deviceID).GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                UnityEngine.Debug.LogError("Failed to retrieve data: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot.Exists)
                {
                    UserScore existingScore = JsonUtility.FromJson<UserScore>(snapshot.GetRawJsonValue());

                    if (existingScore.playerName != localPlayerName)
                    {
                        existingScore.playerName = localPlayerName;
                        string json = JsonUtility.ToJson(existingScore);
                        databaseReference.Child("scores").Child(deviceID).SetRawJsonValueAsync(json);
                    }
                }
            }
        });
    }

    private string GetPlayerNameFromPrefs()
    {
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            return PlayerPrefs.GetString("PlayerName");
        }
        return "Unknown";
    }

    private void SetupScoreChangeListener()
    {
        databaseReference.Child("scores").ValueChanged += OnScoresChanged;
    }

    private void OnScoresChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            UnityEngine.Debug.LogError(args.DatabaseError.Message);
            return;
        }

        UpdateRanking();  // Firebaseのスコアデータが変わったらランキングを更新
    }

    public void RetrieveTopScoresAroundCurrentScore(int currentGameScore, System.Action<List<UserScore>, int> callback)
    {
        databaseReference.Child("scores").OrderByChild("score").GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                UnityEngine.Debug.LogError("Failed to retrieve data: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                List<UserScore> scores = new List<UserScore>();
                foreach (var child in snapshot.Children)
                {
                    UserScore userScore = JsonUtility.FromJson<UserScore>(child.GetRawJsonValue());
                    scores.Add(userScore);
                }

                // 降順にソート
                scores.Sort((a, b) => b.score.CompareTo(a.score));

                // 現在のゲームスコアの位置を特定
                int currentIndex = scores.FindIndex(s => s.score <= currentGameScore);

                // その位置にplayerのスコアを挿入
                scores.Insert(currentIndex, new UserScore(SystemInfo.deviceUniqueIdentifier, "あなた", currentGameScore));

                // DB上での順位を取得
                int dbRank = currentIndex + 1; // +1するのは、0-based indexから1-based rankへの変換のため

                List<UserScore> surroundingScores = new List<UserScore>();

                if (currentIndex > 0)
                {
                    surroundingScores.Add(scores[currentIndex - 1]);  // 前のスコア
                }

                surroundingScores.Add(scores[currentIndex]);  // playerのスコア

                if (currentIndex < scores.Count - 1)
                {
                    surroundingScores.Add(scores[currentIndex + 1]);  // 次のスコア
                }

                callback(surroundingScores, dbRank);
            }
        });
    }




}

[System.Serializable]
public class UserScore
{
    public string deviceID;
    public string playerName;
    public int score;

    public UserScore(string _deviceID, string _playerName, int _score)
    {
        deviceID = _deviceID;
        playerName = _playerName;
        score = _score;
    }
}
