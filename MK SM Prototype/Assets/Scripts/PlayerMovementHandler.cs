using System.Collections;
using System.Collections.Generic;
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

        public Vector2 CurrentMovementInput { get => currentMovementInput; set => currentMovementInput = value; }
        public bool IsJumpPressed { get => isJumpPressed; set => isJumpPressed = value; }
        public GameObject ActiveModel { get => activeModel;}
        public Rigidbody Rb { get => rb;}
        public Animator Anim { get => anim;}

        private void Start()
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
            //The change in position our character should point to 
            positionToLookAt.x = currentMovement.x;
            positionToLookAt.y = 0;
            positionToLookAt.z = currentMovement.z;
            //The current rotation of our character
            Quaternion currentRotation = transform.rotation;
            //Creates  new rotation based on where the player is currently pressing 
            if (positionToLookAt != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
                transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
            }else if (CanRotateWhileFighting())
            {
                Vector3 targetDirection = enemyDetection.GetTargetDirection();
                targetDirection.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        bool CanRotateWhileFighting()
        {
            if (FightingSystem.isFighting && EnemyDetection.enemyInRadius && enemyDetection.EnemyInFieldOfView())
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

        public void MoveForwardWhilePunching(float deltaTime)
        {
            characterController.Move(transform.forward * (movementVelocity/2) * deltaTime);
        }

    }
}
