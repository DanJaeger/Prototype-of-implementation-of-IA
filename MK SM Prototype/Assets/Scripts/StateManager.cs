using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GA {
    public class StateManager : MonoBehaviour
    {
        [HideInInspector] public float verticalInput;
        [HideInInspector] public float horizontalInput;

        [Header(header:"Movement Settings")]
        [SerializeField] float moveSpeed = 300f;
        [SerializeField] float rotationSpeed = 500f;
        [HideInInspector] public static float moveAmount = 0f;
        Vector3 moveDirection;

        [Header(header: "JumpSettings")]
        [SerializeField] float jumpHeight = 5.0f;
        [SerializeField] float gravity = -9.8f;
        bool isGrounded;
        [HideInInspector] public bool isJumping = false;

        #region Components
        [HideInInspector] public GameObject activeModel;
        [HideInInspector] public Rigidbody rb;
        [HideInInspector] public Animator anim;
        #endregion

        [HideInInspector] public float delta;

        public void Init()
        {
            SetupAnimator();
            rb = GetComponent<Rigidbody>();
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
        }

        public void Tick(float d)
        {
            delta = d;
            MovePlayer(d);
            UpdateAnimations();
        }
        
        void MovePlayer(float time)
        {
            #region PlayerMovement

            if (isGrounded)
            {
                if (isJumping)
                    rb.velocity = new Vector3(rb.velocity.x, Mathf.Sqrt(jumpHeight * -2f * gravity), rb.velocity.z);

                moveDirection = new Vector3(horizontalInput, 0, verticalInput);
                moveDirection.Normalize();
            }
            else
            {
                rb.velocity += Vector3.up * (gravity * 2 * time);
            }

            transform.Translate(moveDirection * (moveSpeed * time), Space.World);
            #endregion

            HandleRotation();

        }

        void HandleRotation()
        {
            Vector3 positionToLookAt;
            //The change in position our character should point to 
            positionToLookAt.x = moveDirection.x;
            positionToLookAt.y = 0;
            positionToLookAt.z = moveDirection.z;
            //The current rotation of our character
            Quaternion currentRotation = transform.rotation;
            //Creates  new rotation based on where the player is currently pressing 
            if (positionToLookAt != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
                transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * delta);
            }
        }

        private void OnCollisionStay(Collision other)
        {
            if (other.gameObject.tag == ("Ground"))
                isGrounded = true;
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.tag == ("Ground"))
                isGrounded = false;
        }

        void UpdateAnimations()
        {

            anim.SetFloat("MoveAmount", moveAmount);
        }

    }

}
