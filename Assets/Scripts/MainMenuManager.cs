using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Level_1");
    }
    public void ExitGame()
    {
        Debug.Log("Выход из игры!");
        Application.Quit();
    }
}