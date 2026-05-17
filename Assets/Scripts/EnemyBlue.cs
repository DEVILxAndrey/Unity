using UnityEngine;

public class EnemyBlue : MonoBehaviour
{
    public Transform target;
    public float speed = 2f;

    [Header("Настройки атаки")]
    public int damage = 15; 

    void Start()
    {
        if (target == null)
        {
            GameObject playerObj = GameObject.Find("Player");
            if (playerObj != null) target = playerObj.transform;
        }
    }

    void Update()
    {
        if (target != null && this.enabled)
        {
            Vector3 targetLookPos = new Vector3(target.position.x, transform.position.y, target.position.z);
            transform.LookAt(targetLookPos);
            transform.position = Vector3.MoveTowards(transform.position, targetLookPos, speed * Time.deltaTime);
        }

        if (transform.position.y < -5f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Player")
        {
            PlayerScript player = collision.gameObject.GetComponent<PlayerScript>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
            Destroy(gameObject); 
        }
    }
}