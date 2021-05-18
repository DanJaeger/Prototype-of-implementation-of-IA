using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance;
        private void Awake()
        {
            Instance = this;
        }

        public void Init()
        {

        }

        public void Tick(float d)
        {

        }
    }
}
