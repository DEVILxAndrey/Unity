using UnityEngine;

public class SlingshotBands : MonoBehaviour
{
    [Header("Линии резинки")]
    public LineRenderer leftBand;
    public LineRenderer rightBand;

    [Header("Точки крепления")]
    public Transform leftFork;
    public Transform rightFork;

    // Скрываем переменную, теперь скрипт сам её заполняет
    private Transform currentBird;

    void Update()
    {
        // 1. ЕСЛИ ПТИЦЫ СЕЙЧАС НЕТ - ИЩЕМ ЕЁ
        if (currentBird == null)
        {
            // Находим все скрипты BirdDrag на сцене
            BirdDrag[] allBirds = FindObjectsByType<BirdDrag>(FindObjectsSortMode.None);
            foreach (BirdDrag b in allBirds)
            {
                // Если птица находится близко к рогатке (меньше 1.5 метров)
                if (Vector2.Distance(b.transform.position, leftFork.position) < 1.5f)
                {
                    currentBird = b.transform; // Хватаем её!
                    break;
                }
            }
        }

        // 2. ЕСЛИ ПТИЦА НАЙДЕНА - РАБОТАЕМ С НЕЙ
        if (currentBird != null)
        {
            // Проверяем, не улетела ли она уже далеко (например, выстрел произошел)
            if (Vector2.Distance(currentBird.position, leftFork.position) > 3f)
            {
                currentBird = null; // Отпускаем резинку
                leftBand.enabled = false;
                rightBand.enabled = false;
            }
            // ... (верхняя часть кода остается такой же) ...
            else
            {
                // Включаем видимость
                leftBand.enabled = true;
                rightBand.enabled = true;

                // --- НОВАЯ МАГИЯ ЗДЕСЬ ---
                // 1. Находим математический центр рогатки (между двумя рогами)
                Vector3 centerSling = (leftFork.position + rightFork.position) / 2f;

                // 2. Узнаем направление: куда мы оттянули птицу относительно рогатки?
                Vector3 dir = (currentBird.position - centerSling).normalized;

                // 3. Вычисляем точку на "спине" (сдвигаем центр птицы дальше по вектору натяжения)
                float birdRadius = 0.4f; // Радиус птицы. Если резинка висит в воздухе — уменьши, если втыкается — увеличь.
                Vector3 holdPoint = currentBird.position + dir * birdRadius;
                // -------------------------

                // Рисуем резинки, которые тянутся к новой точке!
                leftBand.SetPosition(0, leftFork.position);
                leftBand.SetPosition(1, holdPoint);

                rightBand.SetPosition(0, rightFork.position);
                rightBand.SetPosition(1, holdPoint);
            }
        }
        else
        {
            // Если птицы поблизости нет вообще - прячем резинки
            leftBand.enabled = false;
            rightBand.enabled = false;
        }
    }
}