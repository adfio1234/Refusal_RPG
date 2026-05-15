using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private Transform player;

    public void SpawnPlayer(Transform spawnPoint)
    {
        if (player == null || spawnPoint == null)
            return;

        player.position = spawnPoint.position;
    }
}