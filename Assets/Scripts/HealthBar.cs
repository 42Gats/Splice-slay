using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Fighter target;
    public Slider slider;

    void Start()
    {
        Reset();
    }

    public void Reset()
    {
        if (slider == null) slider = GetComponent<Slider>();
        if (target != null)
        {
            Debug.Log("Initializing HealthBar for " + target.name);
            slider.maxValue = target.stats.GetMaxHP();
            slider.value = target.stats.GetCurrentHP();
            Debug.Log("HealthBar initialized: MaxHP = " + slider.maxValue + ", CurrentHP = " + slider.value);
        }
    }

    void Update()
    {
        if (target != null && slider != null)
            slider.value = target.stats.GetCurrentHP();
    }
}
