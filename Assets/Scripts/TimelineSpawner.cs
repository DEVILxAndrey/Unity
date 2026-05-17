using UnityEngine;
using System.Collections;

[System.Serializable]
public class SpawnEvent
{
    public float delay;
    public GameObject enemyPrefab;
}

public class TimelineSpawner : MonoBehaviour
{
    [Header("Сценарий появления врагов")]
    public SpawnEvent[] timeline;

    [Header("Настройки")]
    public Transform playerTransform;

    [Header("Контейнеры точек спавна")]
    [Tooltip("Точки на крышах/трубах для Черного")]
    public Transform sniperPointsContainer;

    [Tooltip("Точки на полу для Красного, Синего и Зеленого")]
    public Transform groundPointsContainer;

    void Start()
    {
        StartCoroutine(PlayTimeline());
    }

    IEnumerator PlayTimeline()
    {
        foreach (SpawnEvent currentEvent in timeline)
        {
            yield return new WaitForSeconds(currentEvent.delay);
            Spawn(currentEvent.enemyPrefab);
        }
        Debug.Log("Сценарий завершен!");
    }

    void Spawn(GameObject prefab)
    {
        if (prefab == null) return;

        Vector3 spawnPos = Vector3.zero;

        if (prefab.GetComponent<BlackEnemy>() != null)
        {
            spawnPos = GetRandomPointFromContainer(sniperPointsContainer);
        }
        else
        {
            spawnPos = GetRandomPointFromContainer(groundPointsContainer);
        }

        if (spawnPos == Vector3.zero) spawnPos = new Vector3(0, 1, 0);

        GameObject newEnemy = Instantiate(prefab, spawnPos, Quaternion.identity);

        var blue = newEnemy.GetComponent<EnemyBlue>();
        if (blue != null) blue.target = playerTransform;

        var red = newEnemy.GetComponent<RedEnemy>();
        if (red != null) red.target = playerTransform;

        var green = newEnemy.GetComponent<GreenEnemy>();
        if (green != null) green.target = playerTransform;
    }

    Vector3 GetRandomPointFromContainer(Transform container)
    {
        if (container == null || container.childCount == 0)
        {
            Debug.LogWarning("Контейнер точек спавна пуст или не назначен!");
            return Vector3.zero;
        }

        int randomIndex = Random.Range(0, container.childCount);
        return container.GetChild(randomIndex).position;
    }
}