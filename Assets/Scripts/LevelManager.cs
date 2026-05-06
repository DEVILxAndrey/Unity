using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("UI Панели")]
    public GameObject winPanel;  
    public GameObject losePanel; 

    private int totalCats; 
    private bool gameEnded = false;

    void Start()
    {
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);


        if (AudioManager.instance != null)
        {
            AudioManager.instance.ResetMusic();
        }

        Enemy[] allCats = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        totalCats = allCats.Length;
    }

    public void CatDestroyed()
    {
        totalCats--; 

        if (totalCats <= 0 && gameEnded == false)
        {
            WinGame();
        }
    }

    public void CheckDefeat()
    {
        Invoke("ShowDefeat", 3.5f);
    }

    private void ShowDefeat()
    {
        if (totalCats > 0 && gameEnded == false)
        {
            gameEnded = true;
            losePanel.SetActive(true);

            if (AudioManager.instance != null)
            {
                AudioManager.instance.FadeMusicOut();
                AudioManager.instance.PlaySound(AudioManager.instance.loseSound);
            }
        }
    }

    private void WinGame()
    {
        gameEnded = true;
        winPanel.SetActive(true);

        if (AudioManager.instance != null)
        {
            AudioManager.instance.FadeMusicOut(); 
            AudioManager.instance.PlaySound(AudioManager.instance.winSound);
        }

        // --- БОНУС ЗА ПТИЦ ---
        BirdDrag[] remainingBirds = FindObjectsByType<BirdDrag>(FindObjectsSortMode.None);

        if (remainingBirds.Length > 0 && ScoreManager.instance != null)
        {
            ScoreManager.instance.AddScore(remainingBirds.Length * 1000);
        }
    }

    // --- Функции для кнопок в новых панелях ---
    public void NextLevel()
    {
        // Выводим сообщение в консоль при нажатии
        Debug.Log("КЛИК ПРОШЕЛ! Пытаюсь загрузить следующий уровень...");

        Time.timeScale = 1f;
        int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        UnityEngine.SceneManagement.SceneManager.LoadScene(currentSceneIndex + 1);
    }
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}