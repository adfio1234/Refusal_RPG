using UnityEngine;

public class RoomPortal : MonoBehaviour
{
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private GameManager gameManager;
    private bool playerInside;

    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    private void Update()
    {
        if (playerInside && Input.GetKeyDown(interactKey))
        {
            if (gameManager != null)
            {
                gameManager.LoadCombatRoom();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            Debug.Log("포탈 근처: E키를 누르세요.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            Debug.Log("포탈에서 벗어남.");
        }
    }
}