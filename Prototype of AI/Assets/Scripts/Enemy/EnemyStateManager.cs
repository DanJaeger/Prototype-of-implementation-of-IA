using DG.Tweening;
using System.Collections;
using UnityEngine;

public class EnemyStateManager : MonoBehaviour
{
    EnemyBaseState currentState;
    CharacterController characterController;
    Animator animator;
    PlayerDetection playerDetection;

    private float health;

    #region PatrollingVariables
    [SerializeField] Transform[] patrolPoints;
    int currentPatrolPointsIndex = 0;

    #endregion

    [SerializeField] float movementSpeed = 3.0f;
    float rotationSpeed = 10.0f;
    Vector3 movementDirection = Vector3.zero;
    Vector3 oldPlayerPosition = Vector3.zero;
    Vector3[] path;


    GameObject playerGameObject = null;
    Transform playerTransform;
    Transform currentTarget;
    bool gettingHit = false;
    bool playerIsOutOfView = true;
    bool followingPath = false;
    float timer = 0;
    int targetIndex = 0;

    public GameObject PlayerGameObject { get => playerGameObject; }
    public bool PlayerIsOutOfView { get => playerIsOutOfView; }
    public Transform[] PatrolPoints { get => patrolPoints; }
    public bool GettingHit { get => gettingHit; set => gettingHit = value; }
    public float Health { get => health; set => health = value; }
    public Transform CurrentTarget { get => currentTarget; set => currentTarget = value; }
    public Transform GetPatrolPoint()
    {
        return patrolPoints[currentPatrolPointsIndex];
    }

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerDetection = GetComponent<PlayerDetection>();
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

        if (!gettingHit)
        {
            currentState.UpdateState(this);
            HandleRotation();
        }
        else
        {
            if (playerGameObject != null)
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
            followingPath = true;

            animator.SetBool("IsWalking", true);
            StopCoroutine("Move");
            StartCoroutine("Move");
        }
    }
    public void Patrol()
    {
        Transform nextPatrolPoint = patrolPoints[currentPatrolPointsIndex];
        if (transform.position == nextPatrolPoint.position)
        {
            Debug.Log("Stop Moving");
            StopCoroutine("Move");
            animator.SetBool("IsWalking", false);

            currentPatrolPointsIndex = (currentPatrolPointsIndex + 1) % patrolPoints.Length;
            currentTarget = patrolPoints[currentPatrolPointsIndex];
            GetPath(currentTarget);
        }
    }
    public void Chase()
    {
        if (!gettingHit)
        {
            float distanceToTarget = (playerTransform.position - transform.position).sqrMagnitude;
            if (playerTransform == null || distanceToTarget <= 4)
            {
                StopCoroutine("Move");
                animator.SetBool("IsWalking", false);
            }
        }
    }

    IEnumerator Move()
    {
        targetIndex = 0;
        Vector3 targetPosition = transform.forward;
        Vector3 currentWaypoint = path[0];
        Debug.Log(currentWaypoint);

        while (true)
        {
            if (followingPath)
            {
                if (transform.position == currentWaypoint)
                {
                    targetIndex++;
                    if (targetIndex >= path.Length)
                    {
                        followingPath = false;
                    }
                    else { 
                    currentWaypoint = path[targetIndex];
                    Debug.Log(currentWaypoint);
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

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
            //characterController.Move(movementDirection * movementSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public void FindNewPath()
    {
        if (playerGameObject != null)
        {
            if (oldPlayerPosition != playerTransform.position)
            {
                currentTarget = playerTransform;
                GetPath(currentTarget);
                oldPlayerPosition = playerTransform.position;
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
        int seconds = ((int)timer % 60);
        if (seconds >= 4)
        {
            playerIsOutOfView = true;
        }
    }
    Vector3 GetPlayerDirection()
    {
        Vector3 targetDirection = Vector3.zero;

        targetDirection = (playerGameObject != null) ?
            playerTransform.position - transform.position :
            transform.forward;
        targetDirection.Normalize();

        return targetDirection;
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
        gettingHit = false;
    }

    void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}
