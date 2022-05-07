using DG.Tweening;
using System.Collections;
using UnityEngine;

public class EnemyStateManager : MonoBehaviour
{
    EnemyBaseState _currentState;
    EnemyStateFactory _states;

    #region Components
    CharacterController _characterController;
    Animator _animator;
    PlayerDetection _playerDetection;
    Grid _grid;
    #endregion

    #region PatrollingVariables
    [SerializeField] Transform[] _patrolPoints;
    int _currentPatrolPointsIndex = 0;
    #endregion

    #region Movement variables
    const float c_MovementSpeed = 3.0f;
    const float c_RotationSpeed = 10.0f;
    Vector3 _movementDirection = Vector3.zero;
    #endregion

    #region Pathfinding variables
    Vector3[] _path;
    Transform _currentTarget;
    bool _followingPath = false;
    const float c_PathUpdateMoveThreshold = 1.0f;
    const float c_MinPathUpdateTime = 0.2f;
    #endregion

    #region Enemy State
    GameObject _hitbox;
    const int c_HitboxIndex = 2;

    private float _health;

    bool _canGetHit = true;
    bool _isGettingHit = false;
    bool _canAttack = false;
    bool _canPunch = true;
    bool _isPlayerIsOutOfView = true;
    #endregion

    #region Player Reference
    [SerializeField] GameObject _player;
    GameObject _playerGameObject = null;
    Transform _playerTransform;
    #endregion

    #region Timer variables
    float _timer = 0;
    const float c_TimeLimitToGoPatrol = 4.0f;
    #endregion

    #region Getters and Setters
    public EnemyBaseState CurrentState { get => _currentState; set => _currentState = value; }

    public Animator Animator { get => _animator; set => _animator = value; }

    public GameObject PlayerGameObject { get => _playerGameObject; set => _playerGameObject = value; }
    public Transform PlayerTransform { get => _playerTransform; set => _playerTransform = value; }
    public Transform CurrentTarget { get => _currentTarget; set => _currentTarget = value; }
    public Transform[] PatrolPoints { get => _patrolPoints; set => _patrolPoints = value; }

    public int CurrentPatrolPointsIndex { get => _currentPatrolPointsIndex; set => _currentPatrolPointsIndex = value; }

    public float Health { get => _health; set => _health = value; }

    public bool CanPunch { get => _canPunch; set => _canPunch = value; }
    public bool IsPlayerOutOfView { get => _isPlayerIsOutOfView; set => _isPlayerIsOutOfView = value; }
    public bool CanAttack { get => _canAttack; set => _canAttack = value; }
    public bool CanGetHit { get => _canGetHit; set => _canGetHit = value; }
    public bool IsGettingHit { get => _isGettingHit; set => _isGettingHit = value; }
    #endregion

    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _characterController.isTrigger = false;

        _animator = GetComponent<Animator>();
        _playerDetection = GetComponent<PlayerDetection>();
        _grid = FindObjectOfType<Grid>().GetComponent<Grid>();

        _hitbox = gameObject.transform.GetChild(c_HitboxIndex).gameObject;
    }
    void Start()
    {
        _currentTarget = _patrolPoints[_currentPatrolPointsIndex];

        InitState();

        _health = 50;
    }
    void InitState()
    {
        _states = new EnemyStateFactory(this);
        if (_currentState == null)
            _currentState = _states.Patrol();
        _currentState.EnterState();
    }
    void Update()
    {
        _playerGameObject = _playerDetection.Player; 
        if (_playerGameObject != null)
            _playerTransform = _playerGameObject.transform;

        if (!_isGettingHit)
        {
            _currentState.UpdateState();
            HandleRotation();
        }
        else
        {
            if (_playerTransform != null)
                transform.DOLookAt(_playerTransform.position, 0.15f);
        }
    }
    public Transform GetPatrolPoint()
    {
        return _patrolPoints[_currentPatrolPointsIndex];
    }
    public void GetPath(Transform target)
    {
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }
    void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            _path = newPath;
            _followingPath = true;

            if (_currentState == _states.States[EnemyStates.chase]) {
                if (CanChase())
                {
                    _animator.SetBool("IsWalking", true);
                    StopCoroutine("Move");
                    StartCoroutine("Move");
                }
            }
            else
            {
                _animator.SetBool("IsWalking", true);
                StopCoroutine("Move");
                StartCoroutine("Move");
            }
        }
    }
    IEnumerator Move()
    {
        int targetIndex = 0;
        Vector3 targetPosition = transform.forward;

        if (_path.Length > 0)
        {
            Vector3 currentWaypoint = _path[0];

            while (true)
            {
                if (_followingPath)
                {
                    float distanceToCurrentPoint = (currentWaypoint - transform.position).sqrMagnitude;
                    if (distanceToCurrentPoint < 0.1f)
                    {
                        targetIndex++;
                        if (targetIndex >= _path.Length)
                        {
                            _followingPath = false;
                        }
                        else
                        {
                            currentWaypoint = _path[targetIndex];
                        }
                    }
                    targetPosition = currentWaypoint;
                }
                else
                {
                    targetPosition = _currentTarget.position;
                }
                _movementDirection = targetPosition - transform.position;
                _movementDirection.Normalize();
                _movementDirection.y = 0;

                _animator.SetBool("IsWalking", true);

                _characterController.Move(_movementDirection * c_MovementSpeed * Time.deltaTime);

                yield return null;
            }
        }
    }
    public bool CanChase()
    {
        if (_playerTransform != null)
        {
            float distanceToTarget = Vector3.Distance(_playerTransform.position, transform.position);

            if (distanceToTarget <= 1.7f)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }
    
    public IEnumerator UpdatePath()
    {
        PathRequestManager.RequestPath(transform.position, _player.transform.position, OnPathFound);

        float sqrMoveThreshold = c_PathUpdateMoveThreshold * c_PathUpdateMoveThreshold;
        Vector3 targetPosOld = _player.transform.position;

        while (true)
        {
            yield return new WaitForSeconds(c_MinPathUpdateTime);
            if ((_player.transform.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
            {
                PathRequestManager.RequestPath(transform.position, _player.transform.position, OnPathFound);
                targetPosOld = _player.transform.position;
            }
        }
    }
    public void CheckIfCanChase()
    {
        if (_playerGameObject != null)
        {
            _isPlayerIsOutOfView = false;
            _timer = 0;
        }
    }
    public void CheckIfCanAttack()
    {
        float distanceToTarget = Vector3.Distance(_playerTransform.position, transform.position);

        if (_playerGameObject != null && distanceToTarget <= 1.7f)
        {
            _canAttack = true;   
        }
        else
        {
            _canAttack = false;
        }
    }
    public void PlayerOutOfView()
    {
        _timer += Time.deltaTime;
        if (_timer >= c_TimeLimitToGoPatrol)
        {
            _isPlayerIsOutOfView = true;
        }
    }
    void HandleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = _movementDirection.x;
        positionToLookAt.y = 0;
        positionToLookAt.z = _movementDirection.z;

        Quaternion currentRotation = transform.rotation;

        if (positionToLookAt != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, c_RotationSpeed * Time.deltaTime);
        }
    }
    public void DisableGameObject()
    {
        this.gameObject.SetActive(false);
    }
    public void UpdateAnimations()
    {
        _isGettingHit = false;
        StopCoroutine("RechargePunchCoroutine");
        StartCoroutine("RechargePunchCoroutine");
    }
    IEnumerator RechargePunchCoroutine()
    {
        yield return new WaitForSeconds(2.0f);
        _canPunch = true;
    }
}
