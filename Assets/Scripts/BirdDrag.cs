using UnityEngine;
using System.Collections.Generic;

public class BirdDrag : MonoBehaviour
{
    [Header("Настройки физики")]
    public float launchForce = 500f;
    public float maxDragDistance = 1.5f;
    public float minDragDistance = 1.0f;

    [Header("Траектория и след")]
    public GameObject trailDotPrefab;
    public LineRenderer trajectoryLine;
    public int trajectorySteps = 15;

    private Vector2 startPosition;
    private Rigidbody2D rb;
    private Collider2D col;
    private LineRenderer lr;

    private bool isDragging = false;
    public bool wasLaunched = false;

    // Новые переменные для фикса багов:
    private bool hasCollided = false;
    private float launchTime = 0f;

    private static List<GameObject> activeTrailDots = new List<GameObject>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        lr = GetComponent<LineRenderer>();

        if (lr != null) lr.enabled = false;
        if (trajectoryLine != null) trajectoryLine.enabled = false;
    }

    void Update()
    {
        if (wasLaunched)
        {
            // ПРЕДОХРАНИТЕЛЬ: Если птица улетела в пропасть (ниже Y: -15) или летит дольше 6 секунд (FastBird)
            if (transform.position.y < -15f || (Time.time - launchTime > 6f))
            {
                Destroy(gameObject);
                this.enabled = false;
                return;
            }

            // ПРОВЕРКА ОСТАНОВКИ: Уничтожаем ТОЛЬКО если она уже ударилась (hasCollided) И остановилась
            if (hasCollided && rb.linearVelocity.magnitude < 0.5f && rb.angularVelocity < 15f)
            {
                Destroy(gameObject, 2f);
                this.enabled = false;
            }
            return;
        }

        // 2. НАЧАЛО ТЯГИ
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (col.OverlapPoint(mousePos))
            {
                ClearOldTrail();
                isDragging = true;
                startPosition = transform.position;

                if (lr != null) lr.enabled = true;
                if (trajectoryLine != null) trajectoryLine.enabled = true;

                if (AudioManager.instance != null)
                {
                    AudioManager.instance.PlaySound(AudioManager.instance.stretchSound);
                }
            }
        }

        // 3. ПРОЦЕСС ТЯГИ
        if (isDragging && Input.GetMouseButton(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mousePos - startPosition;

            if (direction.magnitude > maxDragDistance)
            {
                direction = direction.normalized * maxDragDistance;
            }

            transform.position = startPosition + direction;

            if (lr != null)
            {
                lr.SetPosition(0, startPosition);
                lr.SetPosition(1, transform.position);
            }

            DrawTrajectory(startPosition - (Vector2)transform.position);
        }

        // 4. ВЫСТРЕЛ
        if (isDragging && Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            if (lr != null) lr.enabled = false;
            if (trajectoryLine != null) trajectoryLine.enabled = false;

            Vector2 throwVector = startPosition - (Vector2)transform.position;

            if (throwVector.magnitude < minDragDistance)
            {
                transform.position = startPosition;
                if (trajectoryLine != null) trajectoryLine.positionCount = 0;
                return;
            }

            wasLaunched = true;
            launchTime = Time.time; // Запоминаем время выстрела
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.AddForce(throwVector * launchForce);

            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlaySound(AudioManager.instance.shootSound);
            }

            if (trailDotPrefab != null)
            {
                InvokeRepeating("DropTrailDot", 0.05f, 0.1f);
            }

            SlingshotManager sling = Object.FindFirstObjectByType<SlingshotManager>();
            if (sling != null)
            {
                sling.Invoke("LoadNextBird", 2.5f);
            }
        }
    }

    // Метод вызовется автоматически, когда птица коснется чего угодно
    void OnCollisionEnter2D(Collision2D collision)
    {
        hasCollided = true;
    }

    void DropTrailDot()
    {
        if (rb == null || rb.linearVelocity.magnitude < 1f)
        {
            CancelInvoke("DropTrailDot");
            return;
        }

        GameObject newDot = Instantiate(trailDotPrefab, transform.position, Quaternion.identity);
        activeTrailDots.Add(newDot);
    }

    void ClearOldTrail()
    {
        foreach (GameObject dot in activeTrailDots)
        {
            if (dot != null)
            {
                Destroy(dot);
            }
        }
        activeTrailDots.Clear();
    }

    void DrawTrajectory(Vector2 forceVector)
    {
        if (trajectoryLine == null) return;

        Vector2 startVelocity = (forceVector * launchForce / rb.mass) * Time.fixedDeltaTime;
        Vector2 gravity = Physics2D.gravity * rb.gravityScale;

        trajectoryLine.positionCount = trajectorySteps;
        Vector2 currentPos = startPosition;

        for (int i = 0; i < trajectorySteps; i++)
        {
            float t = i * 0.1f;
            Vector2 pointPos = currentPos + startVelocity * t + 0.5f * gravity * t * t;
            trajectoryLine.SetPosition(i, pointPos);
        }
    }
}