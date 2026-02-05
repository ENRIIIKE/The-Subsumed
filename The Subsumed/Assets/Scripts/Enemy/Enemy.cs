using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody), typeof(NavMeshAgent), typeof(CapsuleCollider))]
public class Enemy : MonoBehaviour
{
    private enum EnemyState { Patroling, Chasing, IdleAction, AttackAction}

    #region General Settings
    [Header("General Settings")]
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Animator _animator;
    [SerializeField, ReadOnly] private EnemyState _state;
    [SerializeField, Range(1f, 10f)] private float _walkSpeed = 2.5f;
    [SerializeField, Range(1f, 6f)] private float _sprintFactor = 1f;
    [SerializeField] private float _angularSpeed = 500f;
    [SerializeField] private float _acceleration = 12f;
    #endregion

    #region Patrol Settings
    [Header("Patrol settings")]
    [Tooltip("Enable this to see informational output in console.")]
    [SerializeField] private bool _showPatrolLog = false;

    [Tooltip("Cooldown before next patrol point.")]
    [SerializeField] private float _cooldownTime = 5f;

    [Tooltip("Distance to patrol point or player.")]
    [SerializeField, ReadOnly] private float _distanceToTarget = 0f;

    [Tooltip("Show if cooldown for next patrol points is ongoing.")]
    [SerializeField, ReadOnly] private bool _cooldownOnGoing = false;

    [Tooltip("Assign this to a parent object that has a child objects that are 'patrol points'.")]
    [SerializeField] private Transform _patrolParent;

    [Tooltip("List of patrol points that will be assigned to this list on game start.")]
    [SerializeField, ReadOnly] private List<PatrolPointData> _patrolPointsData = new List<PatrolPointData>();

    [SerializeField, ReadOnly] private bool _onIdleAction = false;

    [SerializeField] private float _idleActionCooldown = 1f;

    [SerializeField] private bool _idleActionOnGoing = false;

    private Vector3 _nextDestination;
    private PatrolPointData _nextPatrolPointData;
    private int _indexOfLastDestination = 0;
    private bool _findingNewPoint = false;
    #endregion

    #region Sight Cone Settings
    // Sight Cone
    [Header("Sight cone settings")]
    [Tooltip("Sight distance, visually represtend by yellow cone, that is not visible to player camera.")]
    [SerializeField] private float _coneScale;

    [Tooltip("Reference for cone object. That object is seperate! It is a child of this gameobject.")]
    [SerializeField] private GameObject _coneObject;

    // Player Detection
    [Header("Player detection settings")]
    [Tooltip("Enable this to see informational output in console.")]
    [SerializeField] private bool _showDetectionLog;

    [Tooltip("When enemy spots player, how long it is going to take for him to react (in seconds).")]
    [SerializeField] private float _reactTime;

    //[Tooltip("If player is in the sight, this will be true.")]
    //[SerializeField, ReadOnly] private bool _playerInSight;

    [Tooltip("Player reference.")]
    [SerializeField] private GameObject _player;

    [Tooltip("For player layer only.")]
    [SerializeField] private LayerMask _playerMask;

    [Tooltip("Execute chase, sound effects and visual effects.")]
    [SerializeField] private UnityEvent _playerChase;   // Start Chase, SFX, VFX

    [Tooltip("Time in seconds for when player is out of enemy's range, this is how long it is going to take to stop chase.")]
    [SerializeField] private float _loseTimer = 5f;

    [Tooltip("If player is out of range, the cooldown starts and this boolean is enabled.")]
    [SerializeField, ReadOnly] private bool _loseTimerOngoing = false;

    [Tooltip("Reference for the cone that represents the sight of an enemey.")]
    [SerializeField] private SightCone _sightCone;

    [Tooltip("Prefab that will be visible only for editor, shows the waypoints to get to the patrol point.")]
    [SerializeField] private GameObject _prefabForLastPos;
    #endregion

    #region Attack
    [Header("Attack settings")]
    [SerializeField] private bool _showAttackLog = false;

    [Tooltip("If this is true, enemy will attack player when he is close enough.")]
    [SerializeField, ReadOnly] private bool _canAttackPlayer = true;

    [Tooltip("Enemy attack range.")]
    [SerializeField] private float _attackRange = 2f;

    [Tooltip("This cooldown time will be executed at the end of the attack animation.")]
    [SerializeField] private float _attackCooldown = 1f;

    [Tooltip("Enemy damage to player.")]
    [SerializeField] private int _attackDamage = 1;

    [SerializeField, ReadOnly] private bool _isStopped = false;
    #endregion

    #region Debug Settings
    [SerializeField] private Image _chaseShow;
    [SerializeField] private TextMeshProUGUI _textState;
    [SerializeField] private TextMeshProUGUI _textDistanceTarget;
    [SerializeField] private TextMeshProUGUI _textTarget;

    #endregion

    void Start()
    {
        _agent.angularSpeed = _angularSpeed;
        _agent.acceleration = _acceleration;
        _agent.speed = _walkSpeed;

        _state = EnemyState.Patroling;

        _patrolPointsData.Clear();
        foreach (Transform point in _patrolParent)
        {
            PatrolPoint patrolPoint = point.GetComponent<PatrolPoint>();
            _patrolPointsData.Add(patrolPoint.patrolPointData);
        }

        StartCoroutine(PatrolCooldown(0.5f));
    }
    private void Update()
    {
        _isStopped = _agent.isStopped;
        if (_agent.pathStatus == NavMeshPathStatus.PathPartial ||
            _agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            Debug.LogError("Agent didn't found a way to the point.");
        }

        // Get the distance to the target
        _distanceToTarget = (transform.position - _nextDestination).magnitude;
        UpdateDebug();

        // Detection for player needs to be checked every frame to ensure that enemy reacts to player entering or leaving the sight cone.
        LookingForPlayer();

        // If player is in sight, chase him, otherwise patrol
        switch (_state)
        {
            case EnemyState.Patroling:

                if (_distanceToTarget < 0.6f && _cooldownOnGoing == false)
                {
                    if (_nextPatrolPointData.PointType == PatrolPointType.ActionPoint && !_findingNewPoint)
                    {
                        if (_showPatrolLog) Debug.Log("Going to Idle");
                        _state = EnemyState.IdleAction;
                    }
                    else
                    {
                        // If this patrol point is Waypoint, then just continue patrolling
                        StartCoroutine(PatrolCooldown(_cooldownTime));
                    }
                }

                // For debug purposes, show the chase status in UI
                _chaseShow.color = Color.green;
                _textState.text = "Patroling";
                break;
            case EnemyState.Chasing:
                // Update destination target
                _agent.SetDestination(_player.transform.position);
                _nextDestination = _player.transform.position;
                _agent.isStopped = false;

                if (!_sightCone.PlayerInCone && !_loseTimerOngoing)
                {
                    StartCoroutine(LoseTimer());
                }
                else
                {
                    StopCoroutine(LoseTimer());
                }
                // Attack player if he is close enough
                if (_distanceToTarget < _attackRange)
                {
                    _state = EnemyState.AttackAction;
                }

                // For debug purposes, show the chase status in UI
                _chaseShow.color = Color.red;
                _textState.text = "Chasing";
                break;
            case EnemyState.AttackAction:
                // If enemy is close enough to player, stop moving and attack him
                if (_showAttackLog) Debug.Log("Enemy Action");
                _nextDestination = _player.transform.position;
                _agent.isStopped = true;

                if (_canAttackPlayer)
                {
                    AttackPlayer();
                }

                if (_distanceToTarget > _attackRange)
                {
                    // If player is out of range, start cooldown and go back to patroling
                    _state = EnemyState.Chasing;
                }

                _textState.text = "Attacking";
                break;
            case EnemyState.IdleAction:
                // If enemy is at the Action Point, then do the action
                if (_nextPatrolPointData.ActionDirection != null && !_onIdleAction)
                {
                    Vector3 position = _nextPatrolPointData.ActionDirection.position;
                    position.y = transform.position.y; // Keep the y position the same

                    transform.DOLookAt(position, 0.5f);
                    if (_showPatrolLog) Debug.Log("Idle Action");
                }
                if (_animator != null && !_onIdleAction)
                {
                    _animator.SetInteger("ActionIndex", _nextPatrolPointData.IndexOfAnimation);
                }
                else if (!_idleActionOnGoing)
                {
                    StartCoroutine(IdleActionCooldown());
                }

                _onIdleAction = true;
                _textState.text = "Idle Action";
                break;
        }
    }


    #region Looking For Player
    /// <summary>
    /// Determines whether the player is within the sight cone and initiates appropriate actions if detected.
    /// </summary>
    /// <remarks>This method checks if the player is within the sight cone and performs a raycast to confirm
    /// visibility. If the player is detected, it updates the state, logs detection (if enabled), starts a reaction
    /// timer, and invokes the player chase event. If the player is not detected, the sight state is reset.</remarks>
    private void LookingForPlayer()
    {
        if (_sightCone.PlayerInCone && _state == EnemyState.Patroling)
        {
            Vector3 directionToPlayer = (_player.transform.position - transform.position).normalized;
            RaycastHit hit; // Not used yet, but needed for Raycast to work properly 

            bool playerHit = Physics.Raycast(
                _coneObject.transform.position,
                directionToPlayer,
                out hit,
                _coneScale,
                _playerMask
            );

            if (playerHit)
            {
                //_playerInSight = true;

                if (_showDetectionLog)
                    Debug.Log("Player in Sight");

                StartCoroutine(ReactTimer());
                _playerChase.Invoke();
            }
        }
        else
        {
            //_playerInSight = false;
        }
    }
    #endregion

    #region Patrol

    /// <summary>
    /// Initiates a cooldown period before assigning a new patrol destination to the agent.
    /// </summary>
    /// <remarks>During the cooldown, a random patrol point is selected from the available list, ensuring it
    /// is not the same as the last destination. Once the cooldown period ends, the agent is directed to the new
    /// destination. This method also logs the cooldown start, end, and the next destination if logging is
    /// enabled.</remarks>
    /// <param name="timer">The duration of the cooldown period, in seconds.</param>
    /// <returns>An enumerator that can be used to control the timing of the cooldown process.</returns>
    private IEnumerator PatrolCooldown(float timer)
    {
        if (_showPatrolLog) Debug.Log("<color=green>Cooldown Start</color>");
        _cooldownOnGoing = true;

        NewPatrolPoint();

        yield return new WaitForSeconds(timer);

        // Make agent move to the destination and enable rotation 
        _agent.SetDestination(_nextDestination);

        if (_showPatrolLog) Debug.Log("<color=red>Cooldown End</color>");
        _cooldownOnGoing = false;

        // Draw the waypoints for the last path
        //StartCoroutine(DrawWaypoints());
    }
    private void NewPatrolPoint()
    {
        // Get the random point from the list of patrol points
        _findingNewPoint = true;

        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, _patrolPointsData.Count);
        } while (randomIndex == _indexOfLastDestination);

        // Assign to last point to avoid repeating the same point
        _indexOfLastDestination = randomIndex;

        // Assign to the next destination and log it
        _nextDestination = _patrolPointsData[randomIndex].PointTransform.position;
        _nextPatrolPointData = _patrolPointsData[randomIndex];
        if (_showPatrolLog) Debug.Log($"<color=yellow>Next Destination: {_patrolPointsData[randomIndex].Name}</color>");

        // Get the distance of new destination
        _distanceToTarget = (transform.position - _nextDestination).magnitude;
        _textTarget.text = _patrolPointsData[randomIndex].Name;

        _findingNewPoint = false;
    }
    private void OnValidate()
    {
        _coneObject.transform.localScale = new Vector3(_coneScale, _coneScale, _coneScale);
    }
    #endregion

    #region Chase

    /// <summary>
    /// This method is called by unity event when player is detected by enemy.
    /// </summary>
    public void ChaseAfterPlayer()
    {
        _agent.speed = _walkSpeed * _sprintFactor;
        _textTarget.text = "Player";
    }

    /// <summary>
    /// Waits for a specified reaction time before transitioning the enemy's state to chasing.
    /// </summary>
    /// <remarks>This method pauses execution for the duration of the reaction time and then sets the enemy's
    /// state  to <see cref="EnemyState.Chasing"/>. It also stops any active patrol cooldown coroutine.</remarks>
    /// <returns></returns>
    private IEnumerator ReactTimer()
    {
        yield return new WaitForSeconds(_reactTime);
        StopCoroutine(PatrolCooldown(0f));
        _state = EnemyState.Chasing;
    }

    /// <summary>
    /// Initiates a countdown timer that triggers specific actions upon completion.
    /// </summary>
    /// <remarks>This method starts a timer during which the enemy state is temporarily altered.  Once the
    /// timer elapses, the player's last position is visualized, and the enemy  transitions back to a patrolling state
    /// with adjusted movement speed.</remarks>
    /// <returns>A coroutine that manages the timer and subsequent actions.</returns>
    private IEnumerator LoseTimer()
    {
        _loseTimerOngoing = true;
        if (_showDetectionLog) Debug.Log("Player is out of range, starting lose timer.");
        yield return new WaitForSeconds(_loseTimer);
        _loseTimerOngoing = false;
        Vector3 lastPos = _player.transform.position;
        GameObject visualization = Instantiate(_prefabForLastPos, lastPos, Quaternion.identity);
        Destroy(visualization, 4f);
        _state = EnemyState.Patroling;
        _agent.speed = _walkSpeed;
    }
    #endregion

    #region Attack
    public IEnumerator AttackTimer()
    {
        yield return new WaitForSeconds(_attackCooldown);
        _canAttackPlayer = true;
    }
    private void AttackPlayer()
    {
        // If player is close enough, attack him

        // If animator is not assigned, then wait for the timer to finish
        if (_animator != null)
        {
            _animator.SetTrigger("Attack");
        }
        else
        {
            StartCoroutine(AttackTimer());
        }

         _canAttackPlayer = false;

        // Damage player
        if (_showAttackLog) Debug.Log("Player has been attacked!");
        PlayerSettings.instance.TakeDamage(_attackDamage);
    }

    #endregion

    #region Idle Action
    private void EndIdleAction()
    {
        _state = EnemyState.Patroling;
        _onIdleAction = false;

        NewPatrolPoint();
        _agent.SetDestination(_nextDestination);
    }
    private IEnumerator IdleActionCooldown()
    {
        _idleActionOnGoing = true;
        yield return new WaitForSeconds(_idleActionCooldown);
        _idleActionOnGoing = false;
        EndIdleAction();
    }
    #endregion

    /// <summary>
    /// Draws the waypoints of the last calculated path for the agent.
    /// </summary>
    /// <remarks>This method instantiates a prefab at each waypoint along the agent's path, excluding the
    /// final corner. The method introduces a delay of 0.2 seconds before starting the instantiation process.</remarks>
    /// <returns>A coroutine that performs the waypoint drawing operation after a short delay.</returns>
    private IEnumerator DrawWaypoints()     // Draws the waypoints of the last path
    {
        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < _agent.path.corners.Length - 1; i++)
        {
            Instantiate(_prefabForLastPos, _agent.path.corners[i], Quaternion.identity);
        }
    }
    private void UpdateDebug()
    {
        _textDistanceTarget.text = _distanceToTarget.ToString("F2");
    }
}