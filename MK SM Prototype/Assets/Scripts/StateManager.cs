using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GA {
    public class StateManager : MonoBehaviour
    {
        [HideInInspector] public float verticalInput;
        [HideInInspector] public float horizontalInput;
        public bool isWalking;
        [Header(header:"Movement Settings")]
        [SerializeField] float moveSpeed = 225f;
        [SerializeField] float rotationSpeed = 500f;
        Vector3 moveDirection;

        [HideInInspector] public GameObject activeModel;
        [HideInInspector] public Rigidbody rb;
        [HideInInspector] public Animator anim;

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


        #region PlayerMovement
        void MovePlayer(float time)
        {
            moveDirection = new Vector3(horizontalInput, 0, verticalInput);
            float magnitude = moveDirection.magnitude;//Create this variable because of the normalize function, we'll always want to our magnitude correspond to our inputs 
            Mathf.Clamp01(magnitude);
            moveDirection.Normalize();
            rb.velocity = moveDirection * (magnitude * moveSpeed * time);
            #region Orientation of the character
            if (moveDirection != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * time);
            }
            #endregion
        }

        void UpdateAnimations()
        {
            anim.SetBool("IsWalking", isWalking);
        }
        #endregion

    }

}
