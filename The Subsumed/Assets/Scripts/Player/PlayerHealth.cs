using TMPro;
using UnityEngine;

public class PlayerHealth : HealthSystem
{
    [SerializeField] private TextMeshProUGUI _healthText; // Reference to the UI text element for displaying health

    #region Event OnHealthChanged
    private void OnEnable()
    {
        UpdateHealthUI(CurrentHealth); // Ensure the UI is updated with the initial health value

        // Subscribe to OnHealthChanged event
        OnHealthDamage += Damage;

        // Subsbribe to the OnHealthHeal event
        OnHealthHeal += Heal; // Call the Heal method when health is healed
    }

    // Method to update the health UI when OnHealthChanged is invoked
    private void UpdateHealthUI(int currentHealth)
    {
        if (_healthText != null)
        {
            _healthText.text = currentHealth.ToString(); // Update the health text with the current health value
        }
    }
    private void OnDisable()
    {
        // Unsubscribe from the event to prevent memory leaks
        OnHealthDamage -= Damage;

        // Unsubscribe from the OnHealthHeal event
        OnHealthHeal -= Heal; // Unsubscribe from the Heal method
    }
    #endregion

    public override void Die()
    {
        // Additional player-specific death logic can be added here, such as showing a game over screen or respawning.
        Debug.Log("<color=red>Player has died. Game Over!</color>");

        GetComponent<PlayerInteraction>().ToggleInteract(false);
        GetComponent<PlayerRigidbody>().ToggleMovement(false);

        // Die animation or effects can be triggered here
    }
    public override void SlowDown()
    {
        GetComponent<PlayerRigidbody>().SlowDown();
    }
    public override void TriggerDamage(int damage)
    {
        base.Damage(damage); // Call the base class method to handle damage logic
        UpdateHealthUI(CurrentHealth); // Update the health UI after taking damage
    }
}
