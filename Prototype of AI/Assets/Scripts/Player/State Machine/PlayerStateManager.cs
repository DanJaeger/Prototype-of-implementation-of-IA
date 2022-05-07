using UnityEngine;
using DG.Tweening;

public class PlayerStateManager : MonoBehaviour
{
    float _health;

    #region Components
    private Animator _animator;

    PlayerBaseState _currentState;
    PlayerStateFactory _states;
    CharacterController _characterController;
    EnemyDetection _enemyDetection;
    #endregion

    [Header(header: "Movement Settings")]
    float movementVelocity = 0.0f;
    const float c_walkSpeed = 5.0f;
    const float c_idleSpeed = 0.0f;
    const float rotationSpeed = 10.0f;
    bool _isMovingRight = false;
    bool _isMovingLeft = false;
    Vector2 _currentMovementInput;
    Vector3 _currentMovement;
    Vector3 _appliedMovement;
    bool _isMovementPressed;
    bool _isFighting;
    bool _isInBorderArea = false;

    [Header(header: "JumpSettings")]
    float _gravity = -9.8f;

    float _initialJumpVelocity;
    const float _maxJumpHeight = 2.0f;
    const float _maxJumpTime = 1.0f;
    bool _isJumping = false;
    bool _isJumpPressed = false;

    int _isWalkingHash;
    int _isJumpingHash;
    bool _isGettingHit = false; 
    
    bool _lightPunch;

    int _lightAttackHash;
    bool _canIMove;
    bool _canIPunch;
    int _lightsPunchsCount;

    public PlayerBaseState CurrentState { get => _currentState; set => _currentState = value; }
    public PlayerStateFactory States { get => _states; set => _states = value; }
    public CharacterController CharacterController { get => _characterController; set => _characterController = value; }
    public Animator Animator { get => _animator; set => _animator = value; }
    public bool IsJumping { get => _isJumping; set => _isJumping = value; }
    public float CurrentMovementY { get => _currentMovement.y; set => _currentMovement.y = value; }
    public float InitialJumpVelocity { get => _initialJumpVelocity; set => _initialJumpVelocity = value; }
    public float AppliedMovementY { get => _appliedMovement.y; set => _appliedMovement.y = value; }
    public float Gravity { get => _gravity; set => _gravity = value; }
    public bool IsJumpPressed { get => _isJumpPressed; set => _isJumpPressed = value; }
    public bool IsMovementPressed { get => _isMovementPressed; set => _isMovementPressed = value; }
    public bool IsFighting { get => _isFighting; set => _isFighting = value; }
    public int IsWalkingHash { get => _isWalkingHash; set => _isWalkingHash = value; }
    public int IsJumpingHash { get => _isJumpingHash; set => _isJumpingHash = value; }
    public int LightAttackHash { get => _lightAttackHash; set => _lightAttackHash = value; }
    public bool IsMovingRight { get => _isMovingRight; set => _isMovingRight = value; }
    public bool IsMovingLeft { get => _isMovingLeft; set => _isMovingLeft = value; }
    public bool IsInBorderArea { get => _isInBorderArea; set => _isInBorderArea = value; }
    public bool IsGettingHit { get => _isGettingHit; set => _isGettingHit = value; }
    public Vector2 CurrentMovementInput { get => _currentMovementInput; set => _currentMovementInput = value; }
    public float Health { get => _health; set => _health = value; }
    public bool LightPunch { get => _lightPunch; set => _lightPunch = value; }
    public bool CanIMove { get => _canIMove; set => _canIMove = value; }
    public bool CanIPunch { get => _canIPunch; set => _canIPunch = value; }
    public int LightsPunchsCount { get => _lightsPunchsCount; set => _lightsPunchsCount = value; }
    public float MovementVelocity { get => movementVelocity; set => movementVelocity = value; }

    public static float WalkSpeed => c_walkSpeed;
    public static float IdleSpeed => c_idleSpeed;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _enemyDetection = GetComponent<EnemyDetection>();

        SetupAnimator();
        SetupJumpVariables();

        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();

        _health = 30;
        movementVelocity = c_walkSpeed;
    }
    void SetupAnimator()
    {
        _animator = GetComponentInChildren<Animator>();
        if (_animator == null)
        {
            Debug.Log("No model found");
        }
        _animator.applyRootMotion = false;

        _isWalkingHash = Animator.StringToHash("IsWalking");
        _isJumpingHash = Animator.StringToHash("IsJumping");
        _lightAttackHash = Animator.StringToHash("LightAttack");
    }
    void SetupJumpVariables()
    {
        float timeToApex = _maxJumpTime / 2;
        _gravity = (-2 * _maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        _initialJumpVelocity = (2 * _maxJumpHeight) / timeToApex;
    }

    void Start()
    {
        _characterController.Move(_appliedMovement * Time.deltaTime);
    }

    void Update()
    {
        _currentState.UpdateStates();
        MovePlayer();
    }

    void MovePlayer()
    {
        HandleMovement();
        HandleRotation();
    }

    void HandleMovement()
    {
        _currentMovementInput.Normalize();
        _currentMovement.x = _currentMovementInput.x;
        _currentMovement.z = _currentMovementInput.y;

        _isMovementPressed = (_currentMovementInput.x != 0 || _currentMovementInput.y != 0) ? true : false;

        _isMovingRight = (_currentMovement.x > 0.5f) ? true : false;
        _isMovingLeft = (_currentMovement.x < -0.5f) ? true : false;

        _appliedMovement.x = _currentMovement.x * movementVelocity;
        _appliedMovement.z = _currentMovement.z * movementVelocity;

        _characterController.Move(_appliedMovement * Time.deltaTime);
    }

    void HandleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = _currentMovement.x;
        positionToLookAt.y = 0;
        positionToLookAt.z = _currentMovement.z;

        Quaternion currentRotation = transform.rotation;

        if (CanRotateWhileFighting())
        {
            GameObject closestEnemy = _enemyDetection.GetClosestEnemy();
            if (closestEnemy != null)
            {
                transform.DOLookAt(closestEnemy.transform.position, 0.15f);
            }
        }
        else if (positionToLookAt != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    bool CanRotateWhileFighting()
    {
        if (_isFighting && _enemyDetection.GetClosestEnemy() != null)
            return true;
        else
            return false;
    }

    public void MoveTowardsTarget()
    {
        if (_enemyDetection.GetClosestEnemy() != null)
        {
            GameObject currentEnemy = _enemyDetection.GetClosestEnemy();
            transform.DOMove(TargetOffset(currentEnemy.transform), 0.4f);
        }
    }
    Vector3 TargetOffset(Transform target)
    {
        Vector3 position;
        position = target.position;
        return Vector3.MoveTowards(position, transform.position, .85f);
    }
    public void CheckLightCombo()
    {
        _canIPunch = false;
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("LightAttack_1") && _lightsPunchsCount == 1)
        {
            EndLightPunchCombo();
        }
        else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("LightAttack_1") && _lightsPunchsCount >= 2)
        {
            _animator.SetInteger(_lightAttackHash, 2);
            _canIMove = true;
            _canIPunch = true;
        }
        else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("LightAttack_2") && _lightsPunchsCount == 2)
        {
            EndLightPunchCombo();
        }
        else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("LightAttack_2") && _lightsPunchsCount >= 3)
        {
            _animator.SetInteger(_lightAttackHash, 3);
            _canIPunch = false;
        }
        else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("LightAttack_3"))
        {
            EndLightPunchCombo();
        }

    }

    void EndLightPunchCombo()
    {
        _animator.SetInteger(_lightAttackHash, 0);
        _canIPunch = true;
        _lightsPunchsCount = 0;

        _isFighting = false;
    }
    public void UpdateAnimations()
    {
        _isGettingHit = false;
        EndLightPunchCombo();
    }
}
