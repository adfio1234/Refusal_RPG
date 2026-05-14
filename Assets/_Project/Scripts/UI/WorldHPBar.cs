using UnityEngine;

public class WorldHPBar : MonoBehaviour
{
    [SerializeField] private Health targetHealth;
    [SerializeField] private Transform fillTransform;

    private Vector3 originalFillScale;

    private void Awake()
    {
        if (fillTransform != null)
        {
            originalFillScale = fillTransform.localScale;
        }
    }

    private void OnEnable()
    {
        if (targetHealth != null)
        {
            targetHealth.OnHealthChanged += UpdateHPBar;
            UpdateHPBar(targetHealth.CurrentHealth, targetHealth.MaxHealth);
        }
    }

    private void OnDisable()
    {
        if (targetHealth != null)
        {
            targetHealth.OnHealthChanged -= UpdateHPBar;
        }
    }

    private void UpdateHPBar(int currentHealth, int maxHealth)
    {
        if (fillTransform == null) return;

        float ratio = (float)currentHealth / maxHealth;
        ratio = Mathf.Clamp01(ratio);

        fillTransform.localScale = new Vector3(
            originalFillScale.x * ratio,
            originalFillScale.y,
            originalFillScale.z
        );
    }
}