using UnityEngine;

public class WoodStick : MonoBehaviour
{
    [Header("Настройки блока")]
    public float health = 100f;
    public int pointsForDestroy = 50; // Сколько очков даем за блок
    public Sprite damagedSprite;      // Сюда перетащи картинку с трещинами (в Инспекторе)

    private bool isReady = false;
    private float startHealth;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startHealth = health;

        Invoke("SetReady", 2f);
    }

    void SetReady()
    {
        isReady = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isReady == false) return;

        float impactDamage = collision.relativeVelocity.magnitude * 10f;

        if (impactDamage > 15f)
        {
            health -= impactDamage;

            // --- МАГИЯ ТРЕЩИН ЗДЕСЬ ---
            // Если осталось меньше половины ХП и есть картинка с трещиной
            if (health <= startHealth / 2f && damagedSprite != null)
            {
                spriteRenderer.sprite = damagedSprite;
            }

            if (health <= 0)
            {
                Die();
            }
        }
    }

    void Die()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.woodBreakSound);
            AudioManager.instance.PlaySound(AudioManager.instance.glassBreakSound);
        }
        // Добавляем очки за разрушение блока
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddScore(pointsForDestroy);
        }

        Destroy(gameObject);
    }
}