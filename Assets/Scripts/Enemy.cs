using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Настройки кота")]
    public float health = 100f;

    private bool isReady = false;

    void Start()
    {
        // Ждем пару секунд при старте, чтобы кот не умер от падения при спавне
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
            AudioManager.instance.PlaySound(AudioManager.instance.catDeathSound);
        }

        // 1. Начисляем очки за кота
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddScore(500);
        }

        // 2. Говорим менеджеру, что кот убит (ИСПРАВЛЕНО ДЛЯ UNITY 6)
        LevelManager manager = FindFirstObjectByType<LevelManager>();
        if (manager != null)
        {
            manager.CatDestroyed();
        }

        // 3. Уничтожаем кота
        Destroy(gameObject);
    }
}