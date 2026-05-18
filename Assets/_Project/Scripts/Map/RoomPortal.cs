using UnityEngine;
using UnityEngine.SceneManagement;

public enum PortalDestination
{
    CombatRoom,
    HubRoom,
    GameClear,
    MapSelect,
    CompleteRoom
}

public class RoomPortal : MonoBehaviour
{
    [Header("Portal")]
    [SerializeField] private PortalDestination destination;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Scene")]
    [SerializeField] private string mapSelectSceneName = "MapSelect";

    private GameManager gameManager;
    private bool playerInside;

    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
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
        switch (destination)
        {
            case PortalDestination.CombatRoom:
                if (gameManager != null)
                    gameManager.LoadCombatRoom();
                break;

            case PortalDestination.HubRoom:
                if (gameManager != null)
                    gameManager.LoadHubRoom();
                break;

            case PortalDestination.GameClear:
                if (gameManager != null)
                    gameManager.GameClear();
                break;

            case PortalDestination.MapSelect:
                SceneManager.LoadScene(mapSelectSceneName);
                break;

            case PortalDestination.CompleteRoom:
                if (gameManager != null)
                    gameManager.CompleteNormalRoom();
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