using UnityEngine;
using UnityEngine.UI; 

public class HealthDisplay : MonoBehaviour
{
    public Slider healthSlider;

    private void OnEnable()
    {
        PlayerScript.OnHealthChanged += UpdateDisplay;
    }

    private void OnDisable()
    {
        PlayerScript.OnHealthChanged -= UpdateDisplay;
    }

    void UpdateDisplay(int hp)
    {
        if (healthSlider != null)
        {
            healthSlider.value = hp; 
        }
    }
}