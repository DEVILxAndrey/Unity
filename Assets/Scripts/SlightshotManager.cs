using UnityEngine;
using System.Collections.Generic;

public class SlingshotManager : MonoBehaviour
{
    [Header("Настройки")]
    public Transform spawnPoint;
    public List<GameObject> birdsQueue;

    private int currentBirdIndex = 0;

    void Start()
    {
        LoadNextBird();
    }

    public void LoadNextBird()
    {
        if (currentBirdIndex < birdsQueue.Count)
        {
            GameObject nextBird = birdsQueue[currentBirdIndex];

            nextBird.transform.position = spawnPoint.position;

            nextBird.GetComponent<BirdDrag>().enabled = true;

            // --- ИСПРАВЛЕНО ДЛЯ UNITY 6 ---
            nextBird.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            nextBird.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

            currentBirdIndex++;
        }
        else
        {
            Debug.Log("Птицы закончились!");
            // --- ИСПРАВЛЕНО ДЛЯ UNITY 6 ---
            FindFirstObjectByType<LevelManager>().CheckDefeat();
        }
    }
}