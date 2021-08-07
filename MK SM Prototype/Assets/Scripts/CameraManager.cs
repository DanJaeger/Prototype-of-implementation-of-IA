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
        [SerializeField] Vector3 defaultDistance = new Vector3(0, 3.0f, -6.0f);
        Vector3 velocity = Vector3.zero;
        [SerializeField] float smoothTime = 0.3f;

        #region Offset Settings
        Quaternion defaultRotation = Quaternion.Euler(20, 0, 0);
        float smoothInputBased = 5.0f;
        #endregion

        private void Awake()
        {
            states = FindObjectOfType<StateManager>();
        }

        private void Update()
        {
            FollowPlayer();
            
            RotateCameraInputBased();
            
        }

        private void LateUpdate()
        {
            cam.LookAt(target.position);
        }

        void FollowPlayer()
        {
            Vector3 desiredPosition = new Vector3(target.position.x, 0, target.position.z) + defaultDistance;
            Vector3 smoothedPosition = Vector3.SmoothDamp(pivot.position, desiredPosition, ref velocity, smoothTime * Time.deltaTime);
            pivot.position = smoothedPosition;
                
        }

        void RotateCameraInputBased()
        {
            // Rotate the cube by converting the angles into a quaternion.
            if (Input.GetKey(KeyCode.I))
            {
                float tiltAngle = 5.0f;
                RotateCamera(Quaternion.Euler(tiltAngle, 0, 0), smoothInputBased);
            }else if (Input.GetKey(KeyCode.K))
            {
                float tiltAngle = 35.0f;
                RotateCamera(Quaternion.Euler(tiltAngle, 0, 0), smoothInputBased);
            }
            else if (Input.GetKey(KeyCode.J))
            {
                float tiltAngle = -35.0f;
                RotateCamera(Quaternion.Euler(20, tiltAngle, 0), smoothInputBased);
            }
            else if (Input.GetKey(KeyCode.L))
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

       /* bool InRange()
        {
            if (target.position.x > -12.0f && target.position.x < 12.0f)
                return true;
            else
                return false;
        }*/
    }
}
