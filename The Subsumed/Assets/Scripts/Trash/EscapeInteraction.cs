using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeInteraction : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        SceneManager.LoadScene("Main Menu");
        SceneManager.UnloadSceneAsync("Forest");
    }
    public void DeInteract()
    {
        // Doesn't work without this bullshit
    }
}
