using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CapsuleCollider))]
public class Trap : Interactable
{
    [Header("Trap settings")]
    [SerializeField] private bool _trapArmed = true;
    [SerializeField] private bool _disableMovement;

    [Space]
    [SerializeField] private bool _progressDisarm;
    [SerializeField, Range(1, 30)] private int _progressAdd;
    [SerializeField, Range(5, 50)] private float _progressSubtract;
    private float _currentDisarmProgress;

    [Space]
    [SerializeField] private UnityEvent triggerEvents;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _trapArmed)
        {
            if (_showLog) Debug.Log("Trap triggered", gameObject);

            triggerEvents.Invoke();

            /* Inflict damage to player.
             * Trigger sound, if enemy is nearby, he will start running towards 
             * the sound.
             * Disable movement when trapped.
             */
        }
    }
    private void Update()
    {
        if (!_trapArmed) return;

        // Reduce progress in time by the amount set in the inspector.
        if (_progressDisarm && _currentDisarmProgress > 0)
        {
            _currentDisarmProgress = Mathf.Clamp(_currentDisarmProgress, 0, 100);
            _currentDisarmProgress -= _progressSubtract * Time.deltaTime;
        }
    }

    public void DisarmTrap()
    {
        if (!_trapArmed) return;

        if (_progressDisarm)
        {
            _currentDisarmProgress += _progressAdd;

            if (_currentDisarmProgress < 99) return;
        }
        if (_showLog) Debug.Log("Trap disarmed", gameObject);

        triggerEvents.Invoke();
        _trapArmed = false;

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y + 
            0.8f, transform.position.z),new Vector3(1f, 1.5f, 1f));
    }
}