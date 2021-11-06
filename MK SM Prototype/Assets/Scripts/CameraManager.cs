using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GA
{
    public class CameraManager : MonoBehaviour
    {
        StateManager states;

        [SerializeField] Transform cam = null;
        [SerializeField] Transform target = null;
        [SerializeField] Transform pivot = null;
        Vector3 velocity = Vector3.zero;
        [SerializeField] float smoothTime = 0.3f;

        #region Offset Settings
        readonly Quaternion defaultRotation = Quaternion.Euler(20, 0, 0);
        readonly Vector3 defaultOffset = new Vector3(0, 3.5f, -6.5f);
        readonly Vector3 moveRightOffset = new Vector3(6, 3.5f, -6.5f);
        readonly Vector3 moveLeftOffset = new Vector3(-6, 3.5f, -6.5f);
        float smoothInputBased = 5.0f;
        #endregion

        private void Awake()
        {
            states = FindObjectOfType<StateManager>();
        }

        private void Update()
        {
            RotateCameraInputBased();   
        }

        void LateUpdate()
        {
            if (states.isInBorderArea)
            {
                //TODO
            }
            else
                FollowPlayer();
            
        }
        void FollowPlayer()
        {
            Vector3 desiredPosition;
            if (states.isMovingRight && !states.isFighting)
            {
                    desiredPosition = new Vector3(target.position.x, 0, target.position.z) + moveRightOffset;
            }
            else if (states.isMovingLeft && !states.isFighting)
            {
                    desiredPosition = new Vector3(target.position.x, 0, target.position.z) + moveLeftOffset;
            }
            else
                desiredPosition = new Vector3(target.position.x, 0, target.position.z) + defaultOffset;

                Vector3 smoothedPosition = Vector3.SmoothDamp(pivot.position, desiredPosition, ref velocity, smoothTime * Time.deltaTime);
                pivot.position = smoothedPosition;
            
        }

        void RotateCameraInputBased()
        {
            // Rotate the cube by converting the angles into a quaternion.
            if (Input.GetKey(KeyCode.UpArrow))
            {
                float tiltAngle = 5.0f;
                RotateCamera(Quaternion.Euler(tiltAngle, 0, 0), smoothInputBased);
            }else if (Input.GetKey(KeyCode.DownArrow))
            {
                float tiltAngle = 35.0f;
                RotateCamera(Quaternion.Euler(tiltAngle, 0, 0), smoothInputBased);
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                float tiltAngle = -35.0f;
                RotateCamera(Quaternion.Euler(20, tiltAngle, 0), smoothInputBased);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                float tiltAngle = 35.0f;
                RotateCamera(Quaternion.Euler(20, tiltAngle, 0), smoothInputBased);
            }
            else
            {
                RotateCamera(defaultRotation, smoothInputBased);
            }
        }

        void RotateCamera(Quaternion target, float smoothing)
        {
            cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, target, Time.deltaTime * smoothing);
        }
    }
}
