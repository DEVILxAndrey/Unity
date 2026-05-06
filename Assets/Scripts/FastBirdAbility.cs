using UnityEngine;

public class FastBirdAbility : MonoBehaviour
{
    [Header("Настройки способности")]
    public float boostMultiplier = 2f;
    public GameObject boostEffectPrefab;

    private Rigidbody2D rb;
    private BirdDrag dragScript;
    private bool hasBoosted = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        dragScript = GetComponent<BirdDrag>();
    }

    void Update()
    {
        // Проверяем, что выстрел произошел и рывок еще не делали
        if (dragScript != null && dragScript.wasLaunched == true && hasBoosted == false)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Делаем рывок
                rb.linearVelocity = rb.linearVelocity * boostMultiplier;
                hasBoosted = true;

                // Эффект ускорения
                if (boostEffectPrefab != null)
                {
                    GameObject effect = Instantiate(boostEffectPrefab, transform.position, Quaternion.identity);
                    Destroy(effect, 1.5f);
                }
            }
        }
    }
}