using UnityEngine;

public class RedEnemy : MonoBehaviour
{
    public Transform target;
    public float walkSpeed = 3f;
    public float dashSpeed = 15f;
    public float dashDistance = 18f;

    [Header("Настройки атаки")]
    public int damage = 20;

    [Header("Настройки подката (Collider)")]
    public float slideColliderHeight = 0.5f;
    public Vector3 slideColliderCenter = new Vector3(0, -0.75f, 0);

    private bool isDashing = false;
    private Animator anim;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();

        if (target == null)
        {
            GameObject playerObj = GameObject.Find("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;

                Collider[] enemyColliders = GetComponents<Collider>();
                Collider[] playerColliders = playerObj.GetComponents<Collider>();

                foreach (Collider ec in enemyColliders)
                {
                    foreach (Collider pc in playerColliders)
                    {
                        Physics.IgnoreCollision(ec, pc);
                    }
                }
            }
        }
    }

    void Update()
    {
        if (target == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        Vector3 targetLookPos = new Vector3(target.position.x, transform.position.y, target.position.z);

        if (!isDashing)
        {
            transform.LookAt(targetLookPos);
            transform.Translate(Vector3.forward * walkSpeed * Time.deltaTime);

            if (distanceToPlayer < dashDistance)
            {
                isDashing = true;

                if (anim != null) anim.SetTrigger("doSlide");

                CapsuleCollider cap = GetComponent<CapsuleCollider>();
                if (cap != null)
                {
                    cap.height = slideColliderHeight;
                    cap.center = slideColliderCenter;
                }

                Destroy(gameObject, 1.5f);
            }
        }
        else
        {
            transform.Translate(Vector3.forward * dashSpeed * Time.deltaTime);
        }

        if (transform.position.y < -5f) Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            PlayerScript player = other.GetComponent<PlayerScript>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}