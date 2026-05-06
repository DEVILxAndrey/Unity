using UnityEngine;
using UnityEngine.EventSystems; // Обязательно для защиты от кликов сквозь UI (кнопки)

public class CameraFollow : MonoBehaviour
{
    [Header("Настройки слежения")]
    public float smoothSpeed = 5f;

    [Header("Абсолютные границы уровня (Края фона)")]
    public float minX = -10f;
    public float maxX = 20f;
    public float minY = -2f;
    public float maxY = 15f;

    [Header("Настройки масштаба (Зум)")]
    public float zoomSpeedMouse = 2f;    // Скорость зума для ПК
    public float zoomSpeedTouch = 0.02f; // Чувствительность щипка на телефоне
    public float minZoom = 3f;
    public float maxZoom = 8f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private Rigidbody2D activeBird;
    private Camera cam;

    private Vector3 dragOrigin;

    // Флаг "памяти", чтобы знать, когда возвращаться
    private bool wasFollowingBird = false;
    // Флаг, разрешающий сдвиг камеры на телефоне
    private bool canPanMobile = false;

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos;
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        // 1. Ищем летящую птицу
        if (activeBird == null)
        {
            FindActiveBird();

            // МАГИЯ ВОЗВРАТА: Если мы следили за птицей, но она пропала (уничтожилась)
            if (wasFollowingBird && activeBird == null)
            {
                ResetCameraPosition(); // Переводим прицел обратно на рогатку
                wasFollowingBird = false; // Забываем про старую птицу
            }
        }
        else
        {
            // Если птица есть, запоминаем, что мы за ней следим
            wasFollowingBird = true;
        }

        // 2. УПРАВЛЕНИЕ (только если птица не летит)
        if (activeBird == null)
        {
            HandlePCInput();
            HandleMobileInput();
        }
    }

    // --- УПРАВЛЕНИЕ ДЛЯ ПК (Мышь) ---
    void HandlePCInput()
    {
        // Зум колесиком мыши
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            cam.orthographicSize -= scroll * zoomSpeedMouse;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }

        // Панорама правой кнопкой мыши
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButton(1))
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
            targetPos += difference;
        }
    }

    // --- УПРАВЛЕНИЕ ДЛЯ ТЕЛЕФОНА (Сенсор) ---
    void HandleMobileInput()
    {
        if (Input.touchCount == 0) return;

        // ЗУМ (Щипок двумя пальцами)
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            cam.orthographicSize += deltaMagnitudeDiff * zoomSpeedTouch;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
        // ПАНОРАМА (Свайп одним пальцем)
        else if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // Защита №1: Если нажали на кнопку меню, отменяем сдвиг камеры
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    canPanMobile = false;
                    return;
                }

                // Защита №2: Если попали в Птицу, отменяем сдвиг камеры
                Vector2 worldPoint = cam.ScreenToWorldPoint(touch.position);
                RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
                if (hit.collider != null && hit.collider.GetComponent<BirdDrag>() != null)
                {
                    canPanMobile = false;
                    return;
                }

                canPanMobile = true;
                dragOrigin = cam.ScreenToWorldPoint(touch.position);
            }
            else if (touch.phase == TouchPhase.Moved && canPanMobile)
            {
                Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(touch.position);
                targetPos += difference;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                canPanMobile = false;
            }
        }
    }

    void LateUpdate()
    {
        // Следование за летящей птицей
        if (activeBird != null)
        {
            targetPos = new Vector3(activeBird.transform.position.x, activeBird.transform.position.y, transform.position.z);
        }

        float camHeight = cam.orthographicSize;
        float camWidth = cam.orthographicSize * cam.aspect;

        float clampMinX = minX + camWidth;
        float clampMaxX = maxX - camWidth;
        float clampMinY = minY + camHeight;
        float clampMaxY = maxY - camHeight;

        if (clampMinX > clampMaxX) clampMinX = clampMaxX = (minX + maxX) / 2f;
        if (clampMinY > clampMaxY) clampMinY = clampMaxY = (minY + maxY) / 2f;

        targetPos.x = Mathf.Clamp(targetPos.x, clampMinX, clampMaxX);
        targetPos.y = Mathf.Clamp(targetPos.y, clampMinY, clampMaxY);
        targetPos.z = startPos.z;

        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
    }

    void FindActiveBird()
    {
        BirdDrag[] allBirds = Object.FindObjectsByType<BirdDrag>(FindObjectsSortMode.None);

        foreach (BirdDrag bird in allBirds)
        {
            if (bird != null && bird.wasLaunched == true)
            {
                activeBird = bird.GetComponent<Rigidbody2D>();
                return;
            }
        }
    }

    public void ResetCameraPosition()
    {
        targetPos = startPos;
    }
}