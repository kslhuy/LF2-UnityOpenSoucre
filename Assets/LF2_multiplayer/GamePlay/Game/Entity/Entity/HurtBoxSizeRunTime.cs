using System;
using UnityEngine;
namespace LF2.Client{

    public class HurtBoxSizeRunTime : MonoBehaviour {
        public AnimationBox hurtBox;
        private AnimationBox LastBox;

        public bool EnabledBox;
        public Transform Tf_RenderHurtBox;

        [SerializeField] BoxCollider HurtboxCollider;
        private void FixedUpdate() {
            
            if (LastBox.size != hurtBox.size || LastBox.center != hurtBox.center  ){
                LastBox = hurtBox ;
                UpdateBoxCollider();
            }
        }

        private void UpdateBoxCollider()
        {
            
            HurtboxCollider.enabled = EnabledBox;
            HurtboxCollider.center = hurtBox.center;
            HurtboxCollider.size = hurtBox.size;

            Tf_RenderHurtBox.localPosition = hurtBox.center;
            Tf_RenderHurtBox.localScale = hurtBox.size;
        }
        // private void OnValidate() {
        //     Tf_Something.position = hurtBox.center;
        //     Tf_Something.localScale = hurtBox.size;
            
        // }
        private void OnDrawGizmos() {
            Tf_RenderHurtBox.localPosition = hurtBox.center;
            Tf_RenderHurtBox.localScale = hurtBox.size;
            
        }
    }



}
