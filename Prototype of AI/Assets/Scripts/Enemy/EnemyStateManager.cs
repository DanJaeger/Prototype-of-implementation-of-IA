using DG.Tweening;
using System.Collections;
using UnityEngine;

public class EnemyStateManager : MonoBehaviour
{
    #region Components
    EnemyBaseState currentState;
    CharacterController characterController;
    Animator animator;
    PlayerDetection playerDetection;
    Grid grid;
    #endregion

    #region PatrollingVariables
    [SerializeField] Transform[] patrolPoints;
    int currentPatrolPointsIndex = 0;

    #endregion

    #region Movement variables
    [SerializeField] float movementSpeed = 3.0f;
    float rotationSpeed = 10.0f;
    Vector3 movementDirection = Vector3.zero;
    #endregion

    #region Pathfinding variables
    Vector3[] path;
    Transform currentTarget;
    bool followingPath = false;
    const float pathUpdateMoveThreshold = 1.0f;
    const float minPathUpdateTime = 0.2f;
    #endregion

    private float health;
    [SerializeField] GameObject player;
    GameObject playerGameObject = null;
    Transform playerTransform;
    bool isGettingHit = false;
    bool canAttack = false;
    bool canPunch = true;
    bool playerIsOutOfView = true;
    float timer = 0;
    const float timeLimitToGoPatrol = 4.0f;

    public GameObject Player { get => playerGameObject; }
    public bool PlayerIsOutOfView { get => playerIsOutOfView; }
    public bool GettingHit { get => isGettingHit; set => isGettingHit = value; }
    public float Health { get => health; set => health = value; }
    public Transform CurrentTarget { get => currentTarget; set => currentTarget = value; }
    public bool CanAttack { get => canAttack; set => canAttack = value; }

    public Transform GetPatrolPoint()
    {
        return patrolPoints[currentPatrolPointsIndex];
    }

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerDetection = GetComponent<PlayerDetection>();
        grid = FindObjectOfType<Grid>().GetComponent<Grid>();
    }
    void Start()
    {
        currentTarget = patrolPoints[currentPatrolPointsIndex];
        GetPath(currentTarget);

        if (currentState == null)
            currentState = PatrollingState.Instance;

        health = 100;
    }
    void Update()
    {
        playerGameObject = playerDetection.Player; 
        if (playerGameObject != null)
            playerTransform = playerGameObject.transform;

        if (!isGettingHit)
        {
            currentState.UpdateState(this);
            HandleRotation();
        }
        else
        {
            if (playerTransform != null)
                transform.DOLookAt(playerTransform.position, 0.2f);
        }

        if (health <= 0)
        {
            Debug.Log("AHhhhhhhhhhhhhhhhhhhhh");
            Destroy(this.gameObject);
        }
    }
    public void ChangeState(EnemyBaseState newState)
    {
        currentState.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }
    public void GetPath(Transform target)
    {
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }
    void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            //grid.path = newPath; 
            followingPath = true;

            if (currentState == ChasingState.Instance) {
                if (CanChase())
                {
                    animator.SetBool("IsWalking", true);
                    StopCoroutine("Move");
                    StartCoroutine("Move");
                }
            }
            else
            {
                animator.SetBool("IsWalking", true);
                StopCoroutine("Move");
                StartCoroutine("Move");
            }
        }
    }
    public void Patrol()
    {
        Transform nextPatrolPoint = patrolPoints[currentPatrolPointsIndex];
        float distanceToNextPoint = (nextPatrolPoint.position - transform.position).sqrMagnitude;
        if (distanceToNextPoint < 0.1f)
        {
            StopCoroutine("Move");
            animator.SetBool("IsWalking", false);

            currentPatrolPointsIndex = (currentPatrolPointsIndex + 1) % patrolPoints.Length;
            currentTarget = patrolPoints[currentPatrolPointsIndex];
            GetPath(currentTarget);
        }
    }
    public void Chase()
    {
        if (!CanChase())
        {
            StopCoroutine("Move");
            animator.SetBool("IsWalking", false);
        }
    }
    bool CanChase()
    {
        float distanceToTarget = (playerTransform.position - transform.position).sqrMagnitude;

        if (playerTransform == null || distanceToTarget <= 4)
            return false;
        else
            return true;
    }
    IEnumerator Move()
    {
        int targetIndex = 0;
        Vector3 targetPosition = transform.forward;

        if (path.Length > 0) {
            Vector3 currentWaypoint = path[0];

            while (true)
            {
                if (followingPath)
                {
                    float distanceToCurrentPoint = (currentWaypoint - transform.position).sqrMagnitude;
                    if (distanceToCurrentPoint < 0.1f)
                    {
                        targetIndex++;
                        if (targetIndex >= path.Length)
                        {
                            followingPath = false;
                        }
                        else
                        {
                            currentWaypoint = path[targetIndex];
                        }
                    }
                    targetPosition = currentWaypoint;
                }
                else
                {
                    targetPosition = currentTarget.position;
                }
                movementDirection = targetPosition - transform.position;
                movementDirection.Normalize();
                movementDirection.y = 0;

                animator.SetBool("IsWalking", true);

                characterController.Move(movementDirection * movementSpeed * Time.deltaTime);

                yield return null;
            }
        }
    }
    public IEnumerator UpdatePath()
    {
        PathRequestManager.RequestPath(transform.position, player.transform.position, OnPathFound);

        float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 targetPosOld = player.transform.position;

        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime);
            if ((player.transform.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
            {
                PathRequestManager.RequestPath(transform.position, player.transform.position, OnPathFound);
                targetPosOld = player.transform.position;
            }
        }
    }
    public void CheckIfCanChase()
    {
        if (playerGameObject != null)
        {
            playerIsOutOfView = false;
            timer = 0;
        }
    }
    public void PlayerOutOfView()
    {
        timer += Time.deltaTime;
        if (timer >= timeLimitToGoPatrol)
        {
            playerIsOutOfView = true;
        }
    }
    public void CheckIfCanAttack()
    {
        float distanceToTarget = (playerTransform.position - transform.position).sqrMagnitude;
        if (playerGameObject != null && distanceToTarget <= 4)
        {
            canAttack = true;
        }
        else
        {
            canAttack = false;
        }
    }
    public void Attack()
    {
        if (canPunch)
        {
            animator.Play("RightPunch");
            canPunch = false;
            transform.DOLookAt(playerTransform.position, 0.15f);
            transform.DOMove(TargetOffset(playerTransform), 0.4f);
        }
    }
    Vector3 TargetOffset(Transform target)
    {
        Vector3 position;
        position = target.position;
        return Vector3.MoveTowards(position, transform.position, .85f);
    }

    void HandleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = movementDirection.x;
        positionToLookAt.y = 0;
        positionToLookAt.z = movementDirection.z;

        Quaternion currentRotation = transform.rotation;

        if (positionToLookAt != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    public void UpdateAnimations()
    {
        isGettingHit = false;
        StopCoroutine("RechargePunchCoroutine");
        StartCoroutine("RechargePunchCoroutine");
    }

    IEnumerator RechargePunchCoroutine()
    {
        yield return new WaitForSeconds(2.0f);
        canPunch = true;
    }

}
