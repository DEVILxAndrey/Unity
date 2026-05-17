using UnityEngine;

public class MoverScript : SampleScript
{
    public float moveSpeed = 5f;
    public Vector3 targetPosition;
    private bool isMoving = false;

    public override void Use()
    {
        isMoving = true; // Перемещение
    }

    void Update()
    {
        if (isMoving)
        {
            // Умножаем на Time.deltaTime
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Если достигли цели- остановка
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                isMoving = false;
            }
        }
    }
}