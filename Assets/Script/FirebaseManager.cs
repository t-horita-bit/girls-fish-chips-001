using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    private DatabaseReference databaseReference;

    // Singleton instance
    public static FirebaseManager Instance { get; private set; }

    private void Awake()
    {
        // Check if an instance already exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeFirebase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
                Debug.LogError("Failed to initialize Firebase with error: " + task.Exception);
                return;
            }

            if (task.Result != DependencyStatus.Available)
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
                return;
            }

            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
            if (databaseReference == null)
            {
                Debug.LogError("Firebase Database reference is null after initialization");
            }
            else
            {
                Debug.Log("Firebase Database reference initialized successfully");
            }
        });
    }

    public DatabaseReference GetDatabaseReference()
    {
        return databaseReference;
    }
}
