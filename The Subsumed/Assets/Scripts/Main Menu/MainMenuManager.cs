using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void OnPlayButton()
    {
        SceneManager.LoadScene("Forest");
        SceneManager.UnloadSceneAsync("Main Menu");
    }
    public void OnExitButton()
    {
        Application.Quit();
    }
}
