using UnityEngine;
using UnityEngine.UI;

public class HPBarUI : MonoBehaviour
{
    [SerializeField] private Image hpFill;

    public void SetHP(float currentHP, float maxHP)
    {
        if (hpFill == null)
            return;

        hpFill.fillAmount = currentHP / maxHP;
        //³ªÁß¿¡ hpFill.fillAmount(currentHO/maxHP);
    }
}