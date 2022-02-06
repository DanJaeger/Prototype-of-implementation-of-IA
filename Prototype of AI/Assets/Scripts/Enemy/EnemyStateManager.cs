using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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

    float movementSpeed = 3.0f;
    float rotationSpeed = 10.0f;
    Vector3 movementDirection = Vector3.zero;

    GameObject playerGameObject = null;
    Transform playerTransform;
    bool gettingHit = false;
    bool playerIsOutOfView = true;
    float timer = 0;

    public GameObject PlayerGameObject { get => playerGameObject; }
    public bool PlayerIsOutOfView { get => playerIsOutOfView; }
    public bool GettingHit { get => gettingHit; set => gettingHit = value; }
    public float Health { get => health; set => health = value; }

    void Start()
    {
        if (currentState == null)
            currentState = PatrollingState.Instance;

        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerDetection = GetComponent<PlayerDetection>();

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
    public void Patrol()
    {
        Transform nextPatrolPoint = patrolPoints[currentPatrolPointsIndex];
        if (Vector3.Distance(transform.position, nextPatrolPoint.position) < 1)
        {
            currentPatrolPointsIndex = (currentPatrolPointsIndex + 1) % patrolPoints.Length;
        }
        else
        {
            Vector3 targetPosition = Vector3.Lerp(transform.position, nextPatrolPoint.position, 0.5f);
            movementDirection = targetPosition - transform.position;
            movementDirection.Normalize();
            movementDirection.y = 0;

            animator.SetBool("IsWalking", true);
            characterController.Move(movementDirection * movementSpeed * Time.deltaTime);
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
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.8f)
            gettingHit = false;
        else
            Debug.LogError("Se llamo a la funcion antes de terminar la animacion!");
    }
    public void Chase()
    {
        if (!gettingHit)
        {
            float distanceToTarget = (playerTransform.position - transform.position).sqrMagnitude;
            if (playerTransform != null && distanceToTarget >= 4)
            {
                Vector3 targetPosition = Vector3.Lerp(transform.position, playerTransform.position, 0.5f);
                movementDirection = targetPosition - transform.position;
                movementDirection.Normalize();
                movementDirection.y = 0;
                animator.SetBool("IsWalking", true);

                characterController.Move(movementDirection * movementSpeed * Time.deltaTime);
            }
            else
            {
                animator.SetBool("IsWalking", false);
            }
        }
    }

}
