using System;
using UnityEngine;
namespace LF2.Client{

    public class HitBoxSizeRunTime : MonoBehaviour {
        public AnimationBox hitBox;
        private AnimationBox LastHitBox;
        
        public bool EnabledBox;

        public Transform Tf_RenderHitBox;
        [SerializeField] BoxCollider boxCollider;
        private void FixedUpdate() {
            
            if (LastHitBox.size != hitBox.size || LastHitBox.center != hitBox.center  ){
                LastHitBox = hitBox ;
                UpdateBoxCollider();
            }
        }

        private void UpdateBoxCollider()
        {
            
            boxCollider.enabled = EnabledBox;
            boxCollider.center = hitBox.center;
            boxCollider.size = hitBox.size;

            Tf_RenderHitBox.localPosition = hitBox.center;
            Tf_RenderHitBox.localScale = hitBox.size;
        }
        // // private void OnValidate() {
        // //     Tf_Something.position = hurtBox.center;
        // //     Tf_Something.localScale = hurtBox.size;

        // // }
        private void OnDrawGizmos() {
            Tf_RenderHitBox.position = hitBox.center;
            Tf_RenderHitBox.localScale = hitBox.size;

        }
    }



}
