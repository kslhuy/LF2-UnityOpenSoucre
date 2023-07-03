using System;
using UnityEngine;
namespace LF2.Client{

    public class HitBoxSizeRunTime2 : MonoBehaviour {
        public AnimationBox hitbox;
        public bool EnabledBox;

        public Transform Tf_RenderHitBox;

        private void OnDrawGizmos() {
            Tf_RenderHitBox.position = hitbox.center;
            Tf_RenderHitBox.localScale = hitbox.size;
            
        }
    }



}
