using UnityEngine;

public class RoomController:MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] private Transform playerSpawnPoint;

    public Transform PlayerSpawnPoint => playerSpawnPoint;
}
