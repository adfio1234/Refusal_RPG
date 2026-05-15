using UnityEngine;

public enum PortalDestination
{
    CombatRoom,
    HubRoom,
    GameClear
}

public class RoomPortal : MonoBehaviour
{
    [Header("Portal")]
    [SerializeField] private PortalDestination destination;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private GameManager gameManager;
    private bool playerInside;

    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager == null)
        {
            Debug.LogError("GameManager를 찾지 못했습니다.");
        }
    }

    private void Update()
    {
        if (!playerInside)
            return;

        if (Input.GetKeyDown(interactKey))
        {
            MoveToDestination();
        }
    }

    private void MoveToDestination()
    {
        if (gameManager == null)
        {
            Debug.LogError("GameManager가 없어서 이동할 수 없습니다.");
            return;
        }

        switch (destination)
        {
            case PortalDestination.CombatRoom:
                gameManager.LoadCombatRoom();
                break;

            case PortalDestination.HubRoom:
                gameManager.LoadHubRoom();
                break;

            case PortalDestination.GameClear:
                gameManager.GameClear();
                break;
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
        }
    }
}