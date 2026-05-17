using UnityEngine;
using System.Collections;

public class BlackEnemy : MonoBehaviour
{
    [Header("Настройки прицеливания")]
    public Transform player;
    public Transform firePoint;
    public Transform leftEye;
    public Transform rightEye;

    public LineRenderer leftLaser;
    public LineRenderer rightLaser;

    [Header("Визуал и Анимация")]
    public Animator enemyAnim;

    [Header("Настройки оружия")]
    public int damage = 10;
    public float range = 50f;
    public float fireRate = 3f;
    public float laserDuration = 0.1f;

    [Header("Настройки 'Зарядки' перед выстрелом")]
    public float chargeTime = 1.0f; 

    private Light leftEyeLight;
    private Light rightEyeLight;

    private float nextFireTime = 0f;
    private bool isCharging = false;

    private Color idleColor = Color.white;
    private Color midColor = new Color(1f, 0.4f, 0f); 
    private Color finalColor = Color.red;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        if (enemyAnim == null) enemyAnim = GetComponent<Animator>();

        if (leftEye != null) leftEyeLight = leftEye.GetComponent<Light>();
        if (rightEye != null) rightEyeLight = rightEye.GetComponent<Light>();

        ResetEyesColor();
    }

    void Update()
    {
        if (player == null) return;

        Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(targetPosition);

        if (firePoint != null)
        {
            firePoint.LookAt(player.position);
        }

        if (Time.time >= nextFireTime && !isCharging)
        {
            StartCoroutine(ChargeAndShootRoutine());
        }
    }

    IEnumerator ChargeAndShootRoutine()
    {
        isCharging = true;
        float elapsed = 0f;

        while (elapsed < chargeTime)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / chargeTime; 

            Color currentFrameColor;

            if (progress < 0.5f)
            {
                currentFrameColor = Color.Lerp(idleColor, midColor, progress * 2f);
            }
            else
            {

                currentFrameColor = Color.Lerp(midColor, finalColor, (progress - 0.5f) * 2f);
            }
            if (leftEyeLight != null) leftEyeLight.color = currentFrameColor;
            if (rightEyeLight != null) rightEyeLight.color = currentFrameColor;

            yield return null; 
        }

        SetEyesColor(finalColor);

        Shoot();

        nextFireTime = Time.time + fireRate;
        isCharging = false;
    }

    void Shoot()
    {
        if (enemyAnim != null)
        {
            enemyAnim.SetTrigger("Recoil");
        }

        RaycastHit hit;

        leftLaser.enabled = true;
        rightLaser.enabled = true;

        leftLaser.SetPosition(0, leftEye.position);
        rightLaser.SetPosition(0, rightEye.position);

        if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, range, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            leftLaser.SetPosition(1, hit.point);
            rightLaser.SetPosition(1, hit.point);

            if (hit.transform.CompareTag("Player"))
            {
                PlayerScript playerScript = hit.transform.GetComponent<PlayerScript>();
                if (playerScript != null)
                {
                    playerScript.TakeDamage(damage);
                }
            }
        }
        else
        {
            Vector3 endPos = firePoint.position + firePoint.forward * range;
            leftLaser.SetPosition(1, endPos);
            rightLaser.SetPosition(1, endPos);
        }

        StartCoroutine(TurnOffVisuals());
    }

    IEnumerator TurnOffVisuals()
    {
        yield return new WaitForSeconds(laserDuration);

        leftLaser.enabled = false;
        rightLaser.enabled = false;

        ResetEyesColor();
    }

    private void ResetEyesColor()
    {
        if (leftEyeLight != null) { leftEyeLight.enabled = true; leftEyeLight.color = idleColor; }
        if (rightEyeLight != null) { rightEyeLight.enabled = true; rightEyeLight.color = idleColor; }
    }

    private void SetEyesColor(Color color)
    {
        if (leftEyeLight != null) leftEyeLight.color = color;
        if (rightEyeLight != null) rightEyeLight.color = color;
    }
}