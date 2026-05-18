using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private RoomLoader roomLoader;
    [SerializeField] private PlayerSpawner playerSpawner;

    [Header("Rooms")]
    [SerializeField] private GameObject hubRoomPrefab;
    [SerializeField] private GameObject combatRoomPrefab;
    [SerializeField] private GameObject bossRoomPrefab;

    [Header("Scenes")]
    [SerializeField] private string mapSelectSceneName = "MapSelect";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private void Start()
    {
        LoadSelectedRoom();
    }

    private void LoadSelectedRoom()
    {
        switch (SelectedRoomData.SelectedRoomType)
        {
            case RoomType.Hub:
                LoadHubRoom();
                break;

            case RoomType.Combat:
                LoadCombatRoom();
                break;

            case RoomType.Boss:
                LoadBossRoom();
                break;
        }
    }

    public void LoadHubRoom()
    {
        SelectedRoomData.SelectRoom(RoomType.Hub);

        RoomController room = roomLoader.LoadRoom(hubRoomPrefab);
        playerSpawner.SpawnPlayer(room.PlayerSpawnPoint);
    }

    public void LoadCombatRoom()
    {
        SelectedRoomData.SelectRoom(RoomType.Combat);

        RoomController room = roomLoader.LoadRoom(combatRoomPrefab);
        playerSpawner.SpawnPlayer(room.PlayerSpawnPoint);
    }

    public void LoadBossRoom()
    {
        SelectedRoomData.SelectRoom(RoomType.Boss);

        if (bossRoomPrefab == null)
        {
            Debug.LogWarning("BossRoomPrefab이 없습니다. CombatRoom으로 대체합니다.");
            LoadCombatRoom();
            return;
        }

        RoomController room = roomLoader.LoadRoom(bossRoomPrefab);
        playerSpawner.SpawnPlayer(room.PlayerSpawnPoint);
    }

    public void CompleteNormalRoom()
    {
        Debug.Log("일반 방 클리어. 지도 선택 화면으로 이동합니다.");
        SceneManager.LoadScene(mapSelectSceneName);
    }

    public void GameClear()
    {
        Debug.Log("게임 클리어!");

        SelectedRoomData.ResetRun();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void GameOver()
    {
        Debug.Log("게임 오버!");

        SelectedRoomData.ResetRun();
        SceneManager.LoadScene(mainMenuSceneName);
    }
}