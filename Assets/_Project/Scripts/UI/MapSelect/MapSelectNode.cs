using UnityEngine;
using UnityEngine.UI;

public class MapSelectNode : MonoBehaviour
{
    [Header("Image")]
    [SerializeField] private Image nodeImage;

    [Header("Sprites")]
    [SerializeField] private Sprite startSprite;
    [SerializeField] private Sprite combatSprite;
    [SerializeField] private Sprite bossSprite;

    private MapNodeInfo nodeInfo;

    public MapNodeInfo NodeInfo => nodeInfo;

    public void Setup(MapNodeInfo info, bool selectable, bool completed)
    {
        nodeInfo = info;

        RectTransform rect = GetComponent<RectTransform>();
        rect.anchoredPosition = info.anchoredPosition;

        if (nodeImage == null)
        {
            nodeImage = GetComponent<Image>();
        }

        ApplyRoomSprite(info.roomType);
        ApplyStateColor(selectable, completed);
    }

    private void ApplyRoomSprite(RoomType roomType)
    {
        if (nodeImage == null)
            return;

        switch (roomType)
        {
            case RoomType.Hub:
                if (startSprite != null)
                    nodeImage.sprite = startSprite;
                break;

            case RoomType.Combat:
                if (combatSprite != null)
                    nodeImage.sprite = combatSprite;
                break;

            case RoomType.Boss:
                if (bossSprite != null)
                    nodeImage.sprite = bossSprite;
                break;
        }
    }

    private void ApplyStateColor(bool selectable, bool completed)
    {
        if (nodeImage == null)
            return;

        if (completed)
        {
            nodeImage.color = new Color(0.45f, 0.45f, 0.45f, 1f);
        }
        else if (selectable)
        {
            nodeImage.color = Color.white;
        }
        else
        {
            nodeImage.color = new Color(0.25f, 0.25f, 0.25f, 0.7f);
        }
    }
}