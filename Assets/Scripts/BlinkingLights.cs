using UnityEngine;
using System.Collections;

public class BlinkingLight : MonoBehaviour
{
    private Light pointLight;

    [Header("Настройки мигания")]
    public float blinkInterval = 1.5f; // Скорость мигания (в секундах)

    void Start()
    {
        pointLight = GetComponent<Light>();
        StartCoroutine(BlinkRoutine());
    }

    // Корутина — идеальный инструмент для таймеров в Unity
    IEnumerator BlinkRoutine()
    {
        while (true) // Бесконечный цикл мигания
        {
            if (pointLight != null)
            {
                pointLight.enabled = !pointLight.enabled; // Включаем/Выключаем
            }
            yield return new WaitForSeconds(blinkInterval); // Ждем заданное время
        }
    }
}