using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GA {
    public class StateManager : MonoBehaviour
    {
        //Movement Inputs
        [HideInInspector] public float verticalInput;
        [HideInInspector] public float horizontalInput;

        [Header(header:"Movement Settings")]
        [SerializeField] float moveSpeed = 300f;
        [SerializeField] float rotationSpeed = 500f;
        Vector2 currentMovementInput;
        Vector3 currentMovement;
        bool isMovementPressed;

        [Header(header: "JumpSettings")]
        //Gravity Variables
        float gravity = -9.8f;
        float groundedGravity = -0.05f;
        float fallMultiplier = 2.0f;
        bool isFalling = false;

        float initialJumpVelocity;
        float maxJumpHeight = 1.0f;
        float maxJumpTime = 1.0f;
        bool isJumping = false;
        [HideInInspector] public bool isJumpPressed = false;
        bool isJumpAnimating;

        #region Components
        [HideInInspector] public GameObject activeModel;
        [HideInInspector] public Rigidbody rb;
        [HideInInspector] public Animator anim;
        CharacterController characterController;
        #endregion

        int isWalkingHash;
        int isJumpingHash;

        [HideInInspector] public float delta;

        public void Init()
        {
            SetupAnimator();
            SetupJumpVariables();
            rb = GetComponent<Rigidbody>();
            characterController = GetComponent<CharacterController>();
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

        public void Tick(float d)
        {
            delta = d;
            MovePlayer(d);
            HandleAnimations();
        }
        
        void MovePlayer(float time)
        {
            #region PlayerMovement
            currentMovementInput = new Vector2(horizontalInput, verticalInput);
            currentMovementInput.Normalize();
            currentMovement.x = currentMovementInput.x;
            currentMovement.z = currentMovementInput.y;
            if (currentMovementInput.x != 0 || currentMovementInput.y != 0)
                isMovementPressed = true;
            else
                isMovementPressed = false;

            characterController.Move(currentMovement * Time.deltaTime * moveSpeed);
            #endregion


            HandleRotation();
            HandleGravity();
            HandleJump();

        }

        void SetupJumpVariables()
        {
            float timeToApex = maxJumpTime / 2;
            gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
            initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
        }

        void HandleJump()
        {
            if(!isJumping && characterController.isGrounded && isJumpPressed)
            {
                anim.SetBool(isJumpingHash, true);
                isJumping = true;
                isJumpAnimating = true;
                currentMovement.y = initialJumpVelocity * 0.5f;
            }else if(isJumping && characterController.isGrounded && isJumpPressed)
            {
                isJumping = false;
            }
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
                transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * delta);
            }
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
            }else if (isFalling)
            {
                float previousYVelocity = currentMovement.y;
                float newYVelocity = currentMovement.y + (gravity * fallMultiplier * Time.deltaTime);
                float nextYVelocity = Mathf.Max((previousYVelocity + newYVelocity) * 0.5f, -20.0f);
                currentMovement.y = nextYVelocity;
            }
            else
            {
                float previousYVelocity = currentMovement.y;
                float newYVelocity = currentMovement.y + (gravity * Time.deltaTime);
                float nextYVelocity = (previousYVelocity + newYVelocity) * 0.5f;
                currentMovement.y = nextYVelocity;
            }
        }

        void HandleAnimations()
        {
            bool isWalking = anim.GetBool("IsWalking");
            if (isMovementPressed && !isWalking)
                anim.SetBool(isWalkingHash, true);
            else if (!isMovementPressed && isWalking)
                anim.SetBool(isWalkingHash, false);
        }
    }
}
