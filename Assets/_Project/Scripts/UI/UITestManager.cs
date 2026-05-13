using TMPro;
using UnityEngine;

public class UITestManager : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private float healAmount = 10f;

    [Header("UI")]
    [SerializeField] private HPBarUI hpBarUI;
    [SerializeField] private TextMeshProUGUI hpText;

    private float currentHP;

    private void Start()
    {
        currentHP = maxHP;
        RefreshHPUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TakeDamage();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Heal();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetHP();
        }
    }

    public void TakeDamage()
    {
        currentHP -= damageAmount;
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP);

        RefreshHPUI();
        Debug.Log($"피 감소: {currentHP}/{maxHP}");
    }

    public void Heal()
    {
        currentHP += healAmount;
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP);

        RefreshHPUI();
        Debug.Log($"피 회복: {currentHP}/{maxHP}");
    }

    public void ResetHP()
    {
        currentHP = maxHP;

        RefreshHPUI();
        Debug.Log($"피 초기화: {currentHP}/{maxHP}");
    }

    private void RefreshHPUI()
    {
        if (hpBarUI != null)
        {
            hpBarUI.SetHP(currentHP, maxHP);
        }

        if (hpText != null)
        {
            hpText.text = $"{currentHP:0} / {maxHP:0}";
        }
    }
}