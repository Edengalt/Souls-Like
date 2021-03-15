using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class CameraManger : MonoBehaviour
    {
        public static CameraManger singleton;

        public bool lockon;
        public float mouseSpeed = 2;
        public float controllerSpeed = 7;
        public float followSpeed = 9;
        public float minAngle = -35;
        public float maxAngle = 35;



        public Transform target;
        public Transform lockOnTarget;

        [HideInInspector]public Transform pivot;
        [HideInInspector]public Transform camTransform;

        private float turnSmoothing = 0.1f;
        private float smoothX;
        private float smoothY;
        private float smoothXVelocity;
        private float smoothYVelocity;
        [SerializeField] private float lookAngle;
        [SerializeField] private float tiltAngle;

        public void Init(Transform t)
        {
            target = t;
            camTransform = Camera.main.transform;
            pivot = camTransform.parent;
        }

        private void Awake()
        {
            singleton = this;
        }

        public void Tick(float d)
        {
            float h = Input.GetAxis("Mouse X");
            float v = Input.GetAxis("Mouse Y");

            float c_h = Input.GetAxis("RightAxis X");
            float c_v = Input.GetAxis("RightAxis Y");

            float targetSpeed = mouseSpeed;

            if (c_h != 0 || c_v != 0)
            {
                h = c_h;
                v = -c_v;
                targetSpeed = controllerSpeed;
            }

            FollowTarget(d);
            HandleRotations(d, v, h, targetSpeed);
        }

        public void FollowTarget(float d)
        {
            float speed = d * followSpeed;
            Vector3 targetPosition = Vector3.Lerp(transform.position, target.position, speed);
            transform.position = targetPosition;
        }

        private void HandleRotations(float d, float v, float h, float targetSpeed)
        {
            if(turnSmoothing > 0)
            {
                smoothX = Mathf.SmoothDamp(smoothX, h, ref smoothXVelocity, turnSmoothing);
                smoothY = Mathf.SmoothDamp(smoothY, v, ref smoothYVelocity, turnSmoothing);
            }
            else
            {
                smoothX = h;
                smoothY = v;
            }

            tiltAngle -= smoothY * targetSpeed;
            tiltAngle = Mathf.Clamp(tiltAngle, minAngle, maxAngle);
            pivot.localRotation = Quaternion.Euler(tiltAngle, 0, 0);

            if (lockon && lockOnTarget != null)
            {
                Vector3 targetDir = lockOnTarget.position - transform.position;
                
                targetDir.Normalize();
                //targetDir.y = 0;

                if (targetDir == Vector3.zero)
                    targetDir = Vector3.forward;

                Quaternion targetRot = Quaternion.LookRotation(targetDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, d*9);
                lookAngle = transform.eulerAngles.y;

                return;
            }

            lookAngle += smoothX * targetSpeed;
            transform.rotation = Quaternion.Euler(0, lookAngle, 0);
        }
    }
}
