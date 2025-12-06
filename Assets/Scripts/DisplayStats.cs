using UnityEngine;
using UnityEngine.UI;

public class DisplayStats : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;

    [SerializeField] private Text healthText;
    [SerializeField] private Text attackText;
    [SerializeField] private Text rerollingText;
    [SerializeField] private Text defenseText;
    [SerializeField] private Text critChanceText;
    [SerializeField] private Text critDamageText;

    void Awake()
    {
        UpdateStatsDisplay();
    }

    public void UpdateStatsDisplay()
    {
        Stats stats = playerStats.GetFinalStats();
        healthText.text = stats.HP.ToString();
        attackText.text = stats.ATK.ToString();
        rerollingText.text = stats.SPD.ToString();
        defenseText.text = stats.DEF.ToString();
        critChanceText.text = stats.CC.ToString();
        critDamageText.text = stats.CD.ToString();
    }
}
