using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimatorExtension {
        public static bool isPlayingOnLayer(this Animator animator, int fullPathHash, int layer) {
                return animator.GetCurrentAnimatorStateInfo(layer).fullPathHash == fullPathHash;
        }
        public static float normalizedTime(this Animator animator, System.Int32 layer) {
                float time = animator.GetCurrentAnimatorStateInfo(layer).normalizedTime;
                return time > 1 ? 1 : time;
        }

}
