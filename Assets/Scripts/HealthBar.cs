using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Fighter target;
    public Slider slider;

    void Start()
    {
        if (slider == null) slider = GetComponent<Slider>();
        if (target != null)
        {
            slider.maxValue = target.maxHP;
            slider.value = target.currentHP;
        }
    }

    void Update()
    {
        if (target != null && slider != null)
            slider.value = target.currentHP;
    }
}
