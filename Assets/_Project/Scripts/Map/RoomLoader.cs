using UnityEngine;

public class RoomLoader : MonoBehaviour
{
    [SerializeField] private Transform currentRoomParent;

    private RoomController currentRoom;

    public RoomController LoadRoom(GameObject roomPrefab)
    {
        if (currentRoom != null)
        {
            Destroy(currentRoom.gameObject);
        }

        GameObject roomObject = Instantiate(
            roomPrefab,
            Vector3.zero,
            Quaternion.identity,
            currentRoomParent
        );

        currentRoom = roomObject.GetComponent<RoomController>();

        if (currentRoom == null)
        {
            Debug.LogError("불러온 방에 RoomController가 없습니다.");
        }

        return currentRoom;
    }
}