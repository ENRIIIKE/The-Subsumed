using UnityEngine;

public abstract class HealthSystem : MonoBehaviour, IHealth
{
    [Tooltip("Set maximum health.")]
    [SerializeField] private int _maxHealth = 10;
    [Tooltip("Current health of the entity. This value is read-only.")]
    [SerializeField, ReadOnly] private int _currentHealth;
    public int CurrentHealth
    {
        get { return _currentHealth; }
        private set { _currentHealth = value; }
    }
    [Tooltip("Indicates whether the entity can be slowed down.")]
    [SerializeField] private bool _canBeSlowed = false;

    public delegate void HealthDamage(int damage); 
    public event HealthDamage OnHealthDamage;

    public delegate void HealthHeal(int heal);
    public event HealthHeal OnHealthHeal;

    #region Damage
    /// <summary>
    /// Reduces the current health of the entity by the specified damage amount.
    /// </summary>
    /// <remarks>If the resulting health falls below zero, it is clamped to zero.  This method triggers the
    /// <see cref="OnHealthDamage"/> event with the updated health value  and performs additional actions such as
    /// checking the entity's health state and applying a slowdown effect.</remarks>
    /// <param name="damage">The amount of damage to apply. Must be a non-negative integer.</param>
    public void Damage(int damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth < 0)
            CurrentHealth = 0;

        CheckHealth();
        if (_canBeSlowed)
            SlowDown();
    }
    public virtual void TriggerDamage(int damage)
    {
        OnHealthDamage?.Invoke(damage);
    }
    #endregion

    #region Heal
    /// <summary>
    /// Restores the specified amount of health to the entity.
    /// </summary>
    /// <remarks>If the resulting health exceeds the maximum health, it will be capped at the maximum value.
    /// This method triggers the <see cref="OnHealthDamage"/> event after the health is updated.</remarks>
    /// <param name="amount">The amount of health to restore. Must be a non-negative value.</param>
    public void Heal(int amount)
    {
        CurrentHealth += amount; 
        if (CurrentHealth > _maxHealth)
            CurrentHealth = _maxHealth;

        OnHealthDamage?.Invoke(CurrentHealth);
    }
    public virtual void TriggerHeal(int heal)
    {
        OnHealthHeal?.Invoke(heal);
    }
    #endregion

    private void CheckHealth()
    {
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }
    public virtual void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        // Additional death logic can be added here, such as playing an animation or triggering an event.
    }
    public abstract void SlowDown();
}