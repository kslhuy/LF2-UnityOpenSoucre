using System;
using UnityEngine;
namespace LF2.Client{

    public class HurtBoxSizeRunTime2 : MonoBehaviour {
        public AnimationBox hurtBox;
        public bool EnabledBox;

        public Transform Tf_RenderHurtBox;

        private void OnDrawGizmos() {
            Tf_RenderHurtBox.position = hurtBox.center;
            Tf_RenderHurtBox.localScale = hurtBox.size;
            
        }
    }



}
