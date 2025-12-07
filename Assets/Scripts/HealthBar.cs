using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Target")]
    public Fighter target;

    [Header("Sliders")]
    [SerializeField] private Slider mainHealthSlider;    // Barre verte immédiate
    [SerializeField] private Slider damageTrailSlider;   // Barre orange traînante (dégâts)

    [Header("Animation Settings")]
    [SerializeField] private float damageFallSpeed = 50f;  // Vitesse de descente de l'orange
    [SerializeField] private float healRiseSpeed = 50f;   // Vitesse de montée (dégâts + soins)

    private float currentMainValue;
    private float currentTrailValue;
    private bool isDamaged = false; // Garde en mémoire si on est en animation de dégâts

    void Start()
    {
        Reset();
    }

    public void Reset()
    {
        // Auto-find sliders if not assigned
        if (mainHealthSlider == null)
            mainHealthSlider = GetComponent<Slider>();

        // Initialize values
        if (target != null)
        {
            float maxHP = target.stats.GetMaxHP();
            float currentHP = target.stats.GetCurrentHP();

            if (mainHealthSlider != null)
            {
                mainHealthSlider.maxValue = maxHP;
                mainHealthSlider.value = currentHP;
            }

            if (damageTrailSlider != null)
            {
                damageTrailSlider.maxValue = maxHP;
                damageTrailSlider.value = currentHP;
            }

            currentMainValue = currentHP;
            currentTrailValue = currentHP;
        }
    }

    void Update()
    {
        if (target == null || mainHealthSlider == null) return;

        float targetHP = target.stats.GetCurrentHP();
        float maxHP = target.stats.GetMaxHP();

        mainHealthSlider.maxValue = maxHP;
        if (damageTrailSlider != null)
            damageTrailSlider.maxValue = maxHP;

        if (targetHP < currentMainValue)
        {
            isDamaged = true;
        }
        else if (targetHP > currentMainValue)
        {
            isDamaged = false;
        }

        if (isDamaged)
        {
            currentMainValue = targetHP;
            if (damageTrailSlider != null)
                currentTrailValue = Mathf.MoveTowards(currentTrailValue, targetHP, damageFallSpeed * Time.deltaTime);
            
            if (Mathf.Approximately(currentTrailValue, targetHP))
                isDamaged = false;
        }
        else
        {
            currentMainValue = Mathf.MoveTowards(currentMainValue, targetHP, healRiseSpeed * Time.deltaTime);
            if (damageTrailSlider != null)
                currentTrailValue = Mathf.MoveTowards(currentTrailValue, targetHP, healRiseSpeed * Time.deltaTime);
        }

        mainHealthSlider.value = currentMainValue;
        if (damageTrailSlider != null)
            damageTrailSlider.value = currentTrailValue;
    }
}
