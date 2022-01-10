using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace GA {
    public class PlayerMovementHandler : MonoBehaviour
    {
        #region Components
        private GameObject activeModel;
        private Rigidbody rb;
        private Animator anim;
        FightingSystem fightingSystem;
        CharacterController characterController;
        EnemyDetection enemyDetection;
        #endregion

        [Header(header:"Movement Settings")]
        const float movementVelocity = 7.0f;
        const float rotationSpeed = 10.0f;
        [HideInInspector] public bool isMovingRight = false;
        [HideInInspector] public bool isMovingLeft = false;
        Vector2 currentMovementInput;
        Vector3 currentMovement;
        Vector3 appliedMovement;
        [HideInInspector] public bool isMovementPressed;
        [HideInInspector] public bool isInBorderArea = false;

        [Header(header: "JumpSettings")]
        //Gravity Variables
        float gravity = -9.8f;
        float groundedGravity = -0.05f;
        float fallMultiplier = 2.0f;
        bool isFalling = false;

        float initialJumpVelocity;
        const float maxJumpHeight = 2.0f;
        const float maxJumpTime = 1.0f;
        bool isJumping = false;
        bool isJumpPressed = false;
        bool isJumpAnimating;

        int isWalkingHash;
        int isJumpingHash;

        public GameObject ActiveModel { get => activeModel; }
        public Rigidbody Rb { get => rb; }
        public Animator Anim { get => anim; }
        public Vector2 CurrentMovementInput { get => currentMovementInput; set => currentMovementInput = value; }
        public bool IsJumpPressed { get => isJumpPressed; set => isJumpPressed = value; }
        public bool IsJumpAnimating { get => isJumpAnimating;}

        void Start()
        {
            Init();
        }

       void Init()
       {
            SetupAnimator();
            SetupJumpVariables();

            rb = GetComponent<Rigidbody>();
            characterController = GetComponent<CharacterController>();
            fightingSystem = GetComponentInChildren<FightingSystem>();
            enemyDetection = GetComponentInChildren<EnemyDetection>();
       }

        void SetupAnimator()
        {
            if (activeModel == null)
            {
                anim = GetComponentInChildren<Animator>();
                if (anim == null)
                {
                    Debug.Log("No model found");
                }
                else
                {
                    activeModel = anim.gameObject;
                }
            }

            if (anim == null)
            {
                anim = activeModel.GetComponent<Animator>();
            }
            anim.applyRootMotion = false;

            isWalkingHash = Animator.StringToHash("IsWalking");
            isJumpingHash = Animator.StringToHash("IsJumping");
        }
        void Update()
        {
            MovePlayer();
        }
        
        void MovePlayer()
        {
            #region PlayerMovement
            if (!FightingSystem.isFighting)
            {
                HandleMovement();
            }
            #endregion

            HandleRotation();
            HandleGravity();
            HandleJump();
        }

        void HandleMovement()
        {
            currentMovementInput.Normalize();
            currentMovement.x = currentMovementInput.x;
            currentMovement.z = currentMovementInput.y;

            isMovementPressed = (currentMovementInput.x != 0 || currentMovementInput.y != 0) ? true : false;

            isMovingRight = (currentMovement.x > 0.5f) ? true : false;
            isMovingLeft = (currentMovement.x < -0.5f) ? true : false;

            anim.SetBool(isWalkingHash, isMovementPressed);

            appliedMovement.x = currentMovement.x * movementVelocity;
            appliedMovement.z = currentMovement.z * movementVelocity;

            if (!FightingSystem.isBlocking) {
                characterController.Move(appliedMovement * Time.deltaTime);
            }
            else
            {
                bool isWalkingAnimationActive = anim.GetBool(isWalkingHash);
                if (isWalkingAnimationActive)
                    anim.SetBool(isWalkingHash, false);
            }
        }

        void SetupJumpVariables()
        {
            float timeToApex = maxJumpTime / 2;
            gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
            initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
        }

        void HandleJump()
        {
            if(CanJump())
            {
                anim.SetBool(isJumpingHash, true);
                isJumping = true;
                isJumpAnimating = true;
                currentMovement.y = initialJumpVelocity;
                appliedMovement.y = initialJumpVelocity;
            }else if(!CanJump())
            {
                isJumping = false;
            }
        }

        bool CanJump()
        {
            bool canJump = false;
            if(!isJumping && characterController.isGrounded && isJumpPressed && !FightingSystem.isBlocking)
            {
                canJump = true;
            }else if(isJumping && characterController.isGrounded && isJumpPressed)
            {
                canJump = false;
            }
            return canJump;
        }

        void HandleRotation()
        {
            Vector3 positionToLookAt;

            positionToLookAt.x = currentMovement.x;
            positionToLookAt.y = 0;
            positionToLookAt.z = currentMovement.z;

            Quaternion currentRotation = transform.rotation;

            if (CanRotateWhileFighting() || FightingSystem.isBlocking)
            {
                GameObject closestEnemy = enemyDetection.GetClosestEnemy();
                if (closestEnemy != null)
                {
                    transform.DOLookAt(closestEnemy.transform.position, 0.15f);
                }
            }else if (positionToLookAt != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
                transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        bool CanRotateWhileFighting()
        {
            if (FightingSystem.isFighting && enemyDetection.GetClosestEnemy() != null)
                return true;
            else
                return false;
        }

        void HandleGravity()
        {
            isFalling = currentMovement.y <= 0.0f;
            if (characterController.isGrounded)
            {
                if (isJumpAnimating)
                {
                    anim.SetBool(isJumpingHash, false);
                    isJumpAnimating = false;
                }
                currentMovement.y = groundedGravity;
                appliedMovement.y = groundedGravity;
            }
            else if (isFalling)
            {
                float previousYVelocity = currentMovement.y;
                currentMovement.y = currentMovement.y + (gravity * fallMultiplier * Time.deltaTime);
                appliedMovement.y = Mathf.Max((previousYVelocity + currentMovement.y) * 0.5f, -20.0f);
            }
            else
            {
                float previousYVelocity = currentMovement.y;
                currentMovement.y = currentMovement.y + (gravity * Time.deltaTime);
                appliedMovement.y = (previousYVelocity + currentMovement.y) * 0.5f;
            }
        }

        public void MoveTowardsTarget()
        {
            if(enemyDetection.GetClosestEnemy() != null) {
                GameObject currentEnemy = enemyDetection.GetClosestEnemy();
                transform.DOMove(TargetOffset(currentEnemy.transform), 0.4f);
            }
        }
        Vector3 TargetOffset(Transform target)
        {
            Vector3 position;
            position = target.position;
            return Vector3.MoveTowards(position, transform.position, .85f);
        }
    }
}
