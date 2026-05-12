using UnityEngine;

public class PlayerRespawn : MonoBehaviour {
    [SerializeField] private Transform spawnPoint;
    private Rigidbody2D rb;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Respawn() {
        rb.linearVelocity = Vector2.zero;
        transform.position = spawnPoint.position;
    }
}