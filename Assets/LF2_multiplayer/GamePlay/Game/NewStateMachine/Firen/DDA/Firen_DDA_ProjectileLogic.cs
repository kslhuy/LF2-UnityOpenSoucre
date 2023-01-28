using Unity.Netcode;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using System.Collections;
using System;

namespace LF2.Client
{

    public class Firen_DDA_ProjectileLogic :  ProjectileLogic
    {
       
        [SerializeField] private Animator animator;
        private int Henry_Arrow_Normal_2 = Animator.StringToHash("Fire_Ball_2");

        public override void FixedUpdate() {
            if (IsServer){
                base.FixedUpdate();
            }

        }



        protected override void OnTriggerEnter(Collider collider) {
            
            base.OnTriggerEnter(collider);


        }
        protected override void OnTriggerExit(Collider collider) {
         
            base.OnTriggerExit(collider);

        }

    }
}

