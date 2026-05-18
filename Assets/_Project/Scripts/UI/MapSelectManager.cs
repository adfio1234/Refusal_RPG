using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSelectManager : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Game";

    public void SelectCombatRoom()
    {
        SelectedRoomData.SelectRoom(RoomType.Combat);
        SceneManager.LoadScene(gameSceneName);
    }

    public void SelectShopRoom()
    {
        SelectedRoomData.SelectRoom(RoomType.Shop);
        SceneManager.LoadScene(gameSceneName);
    }

    public void SelectTreasureRoom()
    {
        SelectedRoomData.SelectRoom(RoomType.Treasure);
        SceneManager.LoadScene(gameSceneName);
    }

    public void SelectBossRoom()
    {
        SelectedRoomData.SelectRoom(RoomType.Boss);
        SceneManager.LoadScene(gameSceneName);
    }

    public void BackToHub()
    {
        SelectedRoomData.ResetRun();
        SceneManager.LoadScene(gameSceneName);
    }
}