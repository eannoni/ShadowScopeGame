using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;

    public float GetHealth()
    {
        return slider.value;
    }

    public void SetHealth(int health)
    {
        slider.value = health;
    }

    public void DecHealth()
    {
        slider.value--;
    }

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void ResetHealth()
    {
        slider.value = slider.maxValue;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
