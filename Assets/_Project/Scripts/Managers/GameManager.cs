using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private RoomLoader roomLoader;
    [SerializeField] private PlayerSpawner playerSpawner;

    [Header("Rooms")]
    [SerializeField] private GameObject hubRoomPrefab;
    [SerializeField] private GameObject combatRoomPrefab;

    private void Start()
    {
        LoadHubRoom();
    }

    public void LoadHubRoom()
    {
        RoomController room = roomLoader.LoadRoom(hubRoomPrefab);
        playerSpawner.SpawnPlayer(room.PlayerSpawnPoint);
    }

    public void LoadCombatRoom()
    {
        RoomController room = roomLoader.LoadRoom(combatRoomPrefab);
        playerSpawner.SpawnPlayer(room.PlayerSpawnPoint);
    }

    public void GameClear()
    {
        Debug.Log("게임 클리어!");
        LoadHubRoom();
    }
}