// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// namespace LF2{

//     [System.Serializable]
//     public class AnimationClipExtended {
//         public Animator animator;
//         public AnimationClip clip;
//         public string animatorStateName;
//         private int _totalFrames = 0;
//         private int _animationFullNameHash;

//         public void initialize() {
//             _totalFrames = Mathf.RoundToInt(clip.length * clip.frameRate);
//         }

//         public bool isActive() {
//             return animator.isPlayingOnLayer(_animationFullNameHash, 0);
//         }

//         // public bool itsOnLastFrame() {
//         //     float percentage = animator.normalizedTime(layerNumber);
//         //     return (percentage > percentageOnFrame(_totalFrames - 1));
//         // }
        
//         // public bool biggerOrEqualThanFrame(int frameNumber) {
//         //     float percentage = animator.normalizedTime(layerNumber);
//         //     return (percentage >= percentageOnFrame(frameNumber));
//         // }

//         public int TotalFrames(){
//             return _totalFrames;
//         }

//     }
// }
