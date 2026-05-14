using System.Collections;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    [SerializeField] private Health targetHealth;
    [SerializeField] private SpriteRenderer targetSpriteRenderer;
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;

    private Color originalColor;
    private int lastHealth;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        if (targetSpriteRenderer != null)
        {
            originalColor = targetSpriteRenderer.color;
        }

        if (targetHealth != null)
        {
            lastHealth = targetHealth.CurrentHealth;
        }
    }

    private void OnEnable()
    {
        if (targetHealth != null)
        {
            targetHealth.OnHealthChanged += HandleHealthChanged;
            lastHealth = targetHealth.CurrentHealth;
        }
    }

    private void OnDisable()
    {
        if (targetHealth != null)
        {
            targetHealth.OnHealthChanged -= HandleHealthChanged;
        }
    }

    private void HandleHealthChanged(int currentHealth, int maxHealth)
    {
        if (currentHealth < lastHealth)
        {
            PlayFlash();
        }

        lastHealth = currentHealth;
    }

    private void PlayFlash()
    {
        if (targetSpriteRenderer == null) return;

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        targetSpriteRenderer.color = flashColor;

        yield return new WaitForSeconds(flashDuration);

        targetSpriteRenderer.color = originalColor;
        flashCoroutine = null;
    }
}