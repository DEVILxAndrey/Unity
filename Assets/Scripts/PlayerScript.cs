using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections; // Обязательно для IEnumerator (Корутин)
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class PlayerScript : MonoBehaviour
{
    [Header("Настройки физики")]
    public float jumpForce = 15f;
    public float pushForce = 20f;

    [Header("Настройки гравитации")]
    public float fallMultiplier = 4f;
    public float upMultiplier = 3f;

    [Header("Настройки Raycast")]
    public float groundDistance = 1.4f;
    public LayerMask groundMask;

    [Header("Настройки стрельбы (Лазер)")]
    public Transform cameraTransform; // Сама камера (откуда целимся)
    public Transform weaponPoint;     // Откуда рисуется лазер (плечо/оружие)
    public LineRenderer playerLaser;  // Сам лазер
    public float shootRange = 100f;   // Дальность выстрела
    public float laserDuration = 0.1f; // Время вспышки лазера

    [Header("Здоровье")]
    public int maxHealth = 100;
    private int currentHealth;
    public static event Action<int> OnHealthChanged;

    [Header("Управление")]
    public InputActionReference jumpAction;
    public InputActionReference pushRightAction;
    public InputActionReference pushLeftAction;
    public InputActionReference shootAction;

    private Rigidbody rb;
    private List<Collider> enemiesInTrigger = new List<Collider>();

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);
    }

    private void OnEnable()
    {
        jumpAction.action.performed += DoJump;
        pushRightAction.action.performed += DoPushRight;
        pushLeftAction.action.performed += DoPushLeft;
        shootAction.action.performed += DoShoot;

        jumpAction.action.Enable();
        pushRightAction.action.Enable();
        pushLeftAction.action.Enable();
        shootAction.action.Enable();
    }

    private void OnDisable()
    {
        jumpAction.action.performed -= DoJump;
        pushRightAction.action.performed -= DoPushRight;
        pushLeftAction.action.performed -= DoPushLeft;
        shootAction.action.performed -= DoShoot;

        jumpAction.action.Disable();
        pushRightAction.action.Disable();
        pushLeftAction.action.Disable();
        shootAction.action.Disable();
    }

    void FixedUpdate()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (upMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundDistance, groundMask);
    }

    private void DoJump(InputAction.CallbackContext context)
    {
        if (IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyBlue") || other.CompareTag("GreenEnemy"))
        {
            if (!enemiesInTrigger.Contains(other)) enemiesInTrigger.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (enemiesInTrigger.Contains(other)) enemiesInTrigger.Remove(other);
    }

    private void DoPushRight(InputAction.CallbackContext context)
    {
        enemiesInTrigger.RemoveAll(item => item == null);

        foreach (var hitCollider in enemiesInTrigger)
        {
            if (hitCollider.CompareTag("EnemyBlue"))
            {
                Rigidbody enemyRb = hitCollider.GetComponent<Rigidbody>();
                if (enemyRb != null)
                {
                    Vector3 pushDirection = transform.right + (Vector3.up * 0.3f);
                    enemyRb.AddForce(pushDirection.normalized * pushForce, ForceMode.Impulse);

                    EnemyBlue chase = hitCollider.GetComponent<EnemyBlue>();
                    if (chase != null) chase.enabled = false;

                    Animator enemyAnim = hitCollider.GetComponentInChildren<Animator>();
                    if (enemyAnim != null)
                    {
                        enemyAnim.SetTrigger("doFall");
                    }

                    Destroy(hitCollider.gameObject, 2f);
                }
            }
        }
    }

    private void DoPushLeft(InputAction.CallbackContext context)
    {
        enemiesInTrigger.RemoveAll(item => item == null);

        foreach (var hitCollider in enemiesInTrigger)
        {
            if (hitCollider.CompareTag("GreenEnemy"))
            {
                Rigidbody enemyRb = hitCollider.GetComponent<Rigidbody>();
                if (enemyRb != null)
                {
                    Vector3 pushDirection = -transform.right + (Vector3.up * 0.3f);
                    enemyRb.AddForce(pushDirection.normalized * pushForce, ForceMode.Impulse);

                    GreenEnemy chase = hitCollider.GetComponent<GreenEnemy>();
                    if (chase != null) chase.enabled = false;

                    Animator enemyAnim = hitCollider.GetComponentInChildren<Animator>();
                    if (enemyAnim != null)
                    {
                        enemyAnim.SetTrigger("doFall");
                    }

                    Destroy(hitCollider.gameObject, 2f);
                }
            }
        }
    }

    private void DoShoot(InputAction.CallbackContext context)
    {
        if (playerLaser != null && cameraTransform != null)
        {
            StartCoroutine(ShootCoroutine());
        }
    }

    private IEnumerator ShootCoroutine()
    {
        playerLaser.enabled = true;
        playerLaser.SetPosition(0, weaponPoint.position);

        RaycastHit hit;

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, shootRange))
        {
            playerLaser.SetPosition(1, hit.point);
            Debug.Log("Игрок выстрелил и попал в: " + hit.transform.name);

            BlackEnemy blackEnemy = hit.transform.GetComponent<BlackEnemy>();

            if (blackEnemy != null)
            {
                Debug.Log("Снайпер снят!");
                Destroy(hit.transform.gameObject);
            }
            else
            {
                Debug.Log("Броня не пробита / Это не снайпер!");
            }
        }
        else
        {
            playerLaser.SetPosition(1, cameraTransform.position + cameraTransform.forward * shootRange);
        }

        yield return new WaitForSeconds(laserDuration);
        playerLaser.enabled = false;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundDistance);
    }
}