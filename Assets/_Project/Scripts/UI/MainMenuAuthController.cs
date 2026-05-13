using UnityEngine;

public class MainMenuAuthController : MonoBehaviour
{
    private const string LoginKey = "IsLoggedIn";

    [Header("UI Groups")]
    [SerializeField] private GameObject loggedOutGroup;
    [SerializeField] private GameObject loggedInGroup;

    private void Start()
    {
        RefreshUI();
    }

    public void Login()
    {
        PlayerPrefs.SetInt(LoginKey, 1);
        PlayerPrefs.Save();

        Debug.Log("로그인 완료");
        RefreshUI();
    }

    public void SignUp()
    {
        // DB 연동 전까지는 회원가입 버튼도 로그인처럼 처리
        PlayerPrefs.SetInt(LoginKey, 1);
        PlayerPrefs.Save();

        Debug.Log("회원가입 완료 - 임시 로그인 처리");
        RefreshUI();
    }

    public void Logout()
    {
        PlayerPrefs.SetInt(LoginKey, 0);
        PlayerPrefs.Save();

        Debug.Log("로그아웃 완료");
        RefreshUI();
    }

    private bool IsLoggedIn()
    {
        return PlayerPrefs.GetInt(LoginKey, 0) == 1;
    }

    private void RefreshUI()
    {
        bool isLoggedIn = IsLoggedIn();

        if (loggedOutGroup != null)
        {
            loggedOutGroup.SetActive(!isLoggedIn);
        }

        if (loggedInGroup != null)
        {
            loggedInGroup.SetActive(isLoggedIn);
        }
    }
}