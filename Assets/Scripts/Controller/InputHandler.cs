using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class InputHandler : MonoBehaviour
    {
        private float vertical;
        private float horizontal;
        private float delta;
        private bool b_input;
        private bool a_input;
        private bool x_input;
        private bool y_input;

        private bool leftAxis_Down;
        private bool rightAxis_Down;


        private bool rb_input;

        private float rt_axis;
        private bool rt_input;

        private bool lb_input;

        private float lt_axis;
        private bool lt_input;

        private StateManager states;
        private CameraManger cameraManager;
        void Start()
        {
            states = GetComponent<StateManager>();
            states.Init();
            cameraManager = CameraManger.singleton;
            cameraManager.Init(this.transform);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            delta = Time.fixedDeltaTime;
            GetInput();
            UpdateStates();
            states.FixedTick(delta);
            cameraManager.Tick(delta);
        }

        private void Update()
        {
            delta = Time.deltaTime;
            states.Tick(delta);
        }

        private void GetInput()
        {
            vertical = Input.GetAxis("Vertical");
            horizontal = Input.GetAxis("Horizontal");

            b_input = Input.GetButton("B");
            a_input = Input.GetButton("A");
            x_input = Input.GetButton("X");
            y_input = Input.GetButtonUp("Y");

            rb_input = Input.GetButton("RB");
            rt_input = Input.GetButton("RT");
            rt_axis = Input.GetAxis("RT");
            if (rt_axis != 0)
                rt_input = true;

            lb_input = Input.GetButton("LB");
            lt_input = Input.GetButton("LT");
            lt_axis = Input.GetAxis("LT");
            if (lt_axis != 0)
                lt_input = true;

            rightAxis_Down = Input.GetButtonDown("L");
        }

        public void UpdateStates()
        {
            states.horizontal = horizontal;
            states.vertical = vertical;

            Vector3 v = states.vertical * cameraManager.transform.forward;
            Vector3 h = states.horizontal * cameraManager.transform.right;
            states.moveDir = (v + h).normalized;
            float m = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            states.moveAmount = Mathf.Clamp01(m);

            states.rollInput = b_input;
            if (b_input)
            {
                //states.run = (states.moveAmount > 0);
            }
            else
            {
                //states.run = false;
            }

            states.rb = rb_input;
            states.rt = rt_input;
            states.lb = lb_input;
            states.lt = lt_input;


            if (y_input)
            {
                states.isTwoHanded = !states.isTwoHanded;
                states.HandleTwoHanded();
            }

            if (rightAxis_Down)
            {
                Debug.Log("1");
                states.lockOn = !states.lockOn;

                if (states.lockOnTarget == null)
                    states.lockOn = false;

                cameraManager.lockOnTarget = states.lockOnTarget.transform;
                cameraManager.lockon = states.lockOn;
            }
        }
    }
}
