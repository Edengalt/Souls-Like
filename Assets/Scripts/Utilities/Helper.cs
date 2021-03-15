using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class Helper : MonoBehaviour
    {
        [SerializeField] [Range(0, 1)] 
        private float vertical;
        [SerializeField]
        [Range(-0.5f, 1)]
        private float horizontal;

        [SerializeField] private string[] oh_attaks;
        [SerializeField] private string[] th_attaks;

        [SerializeField] private bool playAnim;
        [SerializeField] private bool twoHanded;
        [SerializeField] private bool enableRootMotion;
        [SerializeField] private bool useItem;
        [SerializeField] private bool interactig;
        [SerializeField] private bool lockon;



        private Animator anim;

        void Start()
        {
            anim = GetComponent<Animator>();
        }


        void Update()
        {
            enableRootMotion = !anim.GetBool("canMove");
            interactig = anim.GetBool("interacting");
            string targetnAnim;

            anim.applyRootMotion = enableRootMotion;


            if (!lockon)
            {
                horizontal = 0f;
                vertical = Mathf.Clamp01(vertical);
            }

            anim.SetBool("lockOn", lockon);

            if (enableRootMotion)
                return;

            if (useItem)
            {
                anim.CrossFade("Armature|31_UseItem", 0.5f);
                useItem = false;
            }

            if (interactig)
            {
                playAnim = false;
                vertical = Mathf.Clamp(vertical, 0, 0.5f);
            }

            if (!twoHanded)
            {
                int r = Random.Range(0, oh_attaks.Length);
                targetnAnim = oh_attaks[r];
            }
            else
            {
                int r = Random.Range(0, th_attaks.Length);
                targetnAnim = th_attaks[r];

                if (vertical > 0.5f)
                    targetnAnim = "Armature|22_Hit";
            }

            

            anim.SetBool("two_handed",twoHanded);

            if (playAnim)
            {
                vertical = 0f;
                anim.CrossFade(targetnAnim, 0.2f);
                playAnim = false;
            }

            anim.SetFloat("vertical", vertical);
            anim.SetFloat("horizontal", horizontal);
        }
    }
}
