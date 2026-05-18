using UnityEngine;
using UnityEngine.UI;

public class MapSelectNode : MonoBehaviour
{
    [SerializeField] private Image nodeImage;

    private MapNodeInfo nodeInfo;

    public MapNodeInfo NodeInfo => nodeInfo;

    public void Setup(MapNodeInfo info, bool selectable, bool completed)
    {
        nodeInfo = info;

        RectTransform rect = GetComponent<RectTransform>();
        rect.anchoredPosition = info.anchoredPosition;

        if (nodeImage == null)
            nodeImage = GetComponent<Image>();

        if (nodeImage != null)
        {
            if (completed)
            {
                nodeImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
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
}