using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class StateManager : MonoBehaviour
    {
        [Header("Init")]
        [SerializeField] private GameObject activeModel;

        [HideInInspector] public Animator anim;
        [HideInInspector] public Rigidbody rigid;
        [HideInInspector] public float delta;
        [HideInInspector] public AnimatorHook a_hook;
        [HideInInspector] public LayerMask ignoreLayers;

        [Header("Inputs")]
        public float vertical;
        public float horizontal;
        public float moveAmount;
        public Vector3 moveDir;
        public float toGround = 0.5f;
        public bool rt, lt, rb, lb;
        public bool twoHanded;
        public bool rollInput;

        [Header("Stats")]
        public float moveSpeed = 10;
        public float runSpeed = 15;
        public float rotateSpeed = 5;

        [Header("States")]
        public bool run;
        public bool onGround;
        public bool lockOn;
        public bool inAction;
        public bool canMove;
        public bool isTwoHanded;

        [Header("Other")]
        public EnemyTarget lockOnTarget;


        private float _actionDelay;


        public void Init()
        {

            SetupAnimator();
            rigid = GetComponent<Rigidbody>();
            rigid.angularDrag = 999f;
            rigid.drag = 4;
            rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            a_hook = activeModel.AddComponent<AnimatorHook>();
            a_hook.Init(this);

            gameObject.layer = 8;
            ignoreLayers = ~(1 << 9);

            anim.SetBool("onGround", true);
        }

        private void SetupAnimator()
        {
            if (activeModel == null)
            {
                anim = GetComponentInChildren<Animator>();
                if (anim == null)
                {
                    Debug.LogError("No model found");
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

        public void FixedTick(float d)
        {
            delta = d;
            float targetSpeed = moveSpeed;

            DetectAction();

            if (inAction)
            {
                anim.applyRootMotion = true;

                _actionDelay += delta;
                if (_actionDelay > 0.5f)
                {
                    inAction = false;
                    _actionDelay = 0;
                }
                else
                {
                    return;
                }
            }

            canMove = anim.GetBool("canMove");

            if (!canMove)
                return;

            HandleRoll();

            anim.applyRootMotion = false;

            rigid.drag = (moveAmount > 0 || onGround == false) ?  0 : 4;


            if (run)
            {
                targetSpeed = runSpeed;
            }

            if(onGround)
            rigid.velocity = moveDir * (targetSpeed * moveAmount);

            if (run)
                lockOn = false;

                Vector3 targetDir = (lockOn == false) ? moveDir 
                    : lockOnTarget.transform.position - transform.position;
                targetDir.y = 0;
                if (targetDir == Vector3.zero)
                {
                    targetDir = Vector3.forward;
                }
                Quaternion tr = Quaternion.LookRotation(targetDir);
                Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, delta * moveAmount * rotateSpeed);
                transform.rotation = targetRotation;

            anim.SetBool("lockOn", lockOn); 

            if (!lockOn)
                HandleMovementAnimations();
            else
                HandleLockOnAnimations(moveDir);
        }

        public void DetectAction()
        {
            if (!canMove)
                return;

            if (rb == false && rt == false && lb == false && lt == false)
                return;

            string targetAnimation = null;

            if (rb)
                targetAnimation = "Armature|11_Hit";
            if (rt)
                targetAnimation = "Armature|12_Hit";
            if (lb)
                targetAnimation = "Armature|21_Hit";
            if (lt)
                targetAnimation = "Armature|22_Hit";

            if (string.IsNullOrEmpty(targetAnimation))
                return;

            canMove = false;
            inAction = true;
            anim.CrossFade(targetAnimation, 0.4f);
        }

        private void HandleRoll()
        {
            if (!rollInput)
                return;

                float v = vertical;
                float h = horizontal;

                if (!lockOn)
                {
                    v = (moveAmount > 0.3f)? 1 : 0;
                    h = 0;
                }
                else
                {
                    if (Mathf.Abs(v) < 0.3f)
                        v = 0;
                    if (Mathf.Abs(h) < 0.3f)
                        h = 0;
                }

                anim.SetFloat("vertical", v);
                anim.SetFloat("horizontal", h);

                canMove = false;
                inAction = true;
                anim.CrossFade("Rolls", 0.4f);
        }

        public void Tick(float d)
        {
            delta = d;
            onGround = OnGround();

            anim.SetBool("onGround", onGround);
        }

        private void HandleMovementAnimations()
        {
            anim.SetBool("run", run);
            anim.SetFloat("vertical", moveAmount, 0.4f, delta); 
        }

        private void HandleLockOnAnimations(Vector3 moveDir)
        {
            Vector3 relativeDir = transform.InverseTransformDirection(moveDir);
            float h = relativeDir.x;
            float v = relativeDir.z;

            anim.SetFloat("vertical", v, 0.1f, delta);
            anim.SetFloat("horizontal", h, 0.1f, delta);

        }

        public bool OnGround()
        {
            bool r = false;

            Vector3 origin = transform.position + (Vector3.up* toGround);
            Vector3 dir = Vector3.down;
            float dist = toGround + 0.3f;
            Debug.DrawRay(origin, dir);
            RaycastHit hit;
            if(Physics.Raycast(origin, dir, out hit, dist, ignoreLayers))
            {
                r = true;
                Vector3 targetPosition = hit.point;
                transform.position = targetPosition;
            }
            
            return r;
        }

        public void HandleTwoHanded()
        {
            anim.SetBool("two_handed", isTwoHanded);
        }
    }
}
