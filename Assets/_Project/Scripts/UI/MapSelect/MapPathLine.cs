using UnityEngine;
using UnityEngine.UI;

public class MapPathLine : MonoBehaviour
{
    [SerializeField] private Image lineImage;

    public void Setup(RectTransform fromNode, RectTransform toNode, float thickness, bool activeLine)
    {
        RectTransform lineRect = GetComponent<RectTransform>();

        Vector3 fromPosition = fromNode.position;
        Vector3 toPosition = toNode.position;

        Vector3 direction = toPosition - fromPosition;
        float distance = direction.magnitude;

        lineRect.position = fromPosition + direction * 0.5f;
        lineRect.sizeDelta = new Vector2(distance, thickness);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        lineRect.rotation = Quaternion.Euler(0f, 0f, angle);

        if (lineImage == null)
        {
            lineImage = GetComponent<Image>();
        }

        if (lineImage != null)
        {
            lineImage.color = activeLine
                ? new Color(1f, 0.8f, 0.25f, 1f)
                : new Color(0.25f, 0.25f, 0.25f, 0.65f);
        }
    }
}