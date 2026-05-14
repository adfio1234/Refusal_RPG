using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance { get; private set; }

    public FirebaseAuth Auth { get; private set; }
    public FirebaseFirestore DB { get; private set; }

    public bool IsReady { get; private set; }

    private FirebaseApp app;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            DependencyStatus dependencyStatus = task.Result;

            if (dependencyStatus == DependencyStatus.Available)
            {
                app = FirebaseApp.DefaultInstance;

                Auth = FirebaseAuth.DefaultInstance;
                DB = FirebaseFirestore.DefaultInstance;

                IsReady = true;

                Debug.Log("Firebase 초기화 성공");
            }
            else
            {
                IsReady = false;

                Debug.LogError("Firebase 초기화 실패: " + dependencyStatus);
            }
        });
    }
}