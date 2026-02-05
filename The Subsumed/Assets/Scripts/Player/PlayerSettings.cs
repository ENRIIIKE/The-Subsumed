using UnityEngine;

public class PlayerSettings : MonoBehaviour
{
    [SerializeField, ReadOnly] private bool _gamePaused = false;
    public PlayerRigidbody playerRigidbody;
    public PlayerInteraction playerInteraction;
    public PlayerHealth playerHealth;


    // This singleton will be used to access this script throughout the
    // entire scene.
    public static PlayerSettings instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this);

        //Lock cursor in the middle and make it invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Unpause game
            if (_gamePaused)
            {
                UnPauseGame();
            }
            // Pause game
            else
            {
                PauseGame();
            }
        }

        // Health test
        // Damage
        if (Input.GetKeyDown(KeyCode.H))
        {
            playerHealth.TriggerDamage(1);
        }
        // Heal
        else if (Input.GetKeyDown(KeyCode.J))
        {
            playerHealth.TriggerHeal(1);
        }
    }

    public void UnPauseGame()
    {
        if (!_gamePaused)
        {
            PauseGame();
        }

        _gamePaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerInteraction.ToggleInteract(true);
        playerRigidbody.ToggleMovement(true);

        Debug.LogWarning("Paused game has been resumed.");
    }

    public void PauseGame()
    {
        if (_gamePaused)
        {
            UnPauseGame();
        }

        _gamePaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        playerInteraction.ToggleInteract(false);
        playerRigidbody.ToggleMovement(false);

        Debug.LogWarning("Game has been paused.");
    }

    public void ToggleLinks(bool toggle)
    {
        playerInteraction.ToggleInteract(toggle);
        playerRigidbody.ToggleMovement(toggle);
    }
    public void TakeDamage(int damage)
    {
        playerHealth.TriggerDamage(damage);
    }
}
