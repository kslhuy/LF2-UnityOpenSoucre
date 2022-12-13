using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LF2 {

    [System.Serializable]
    struct ReturnFrame{
        public int FrameToReturn; 
        public int ReturnToFrame;
    }


    [System.Serializable]
    public class FrameChecker {
        public int hitFrameStart;
        public int hitFrameEnd;

        [SerializeField] private ReturnFrame returnFrame;

        public int totalFrames;
        [SerializeField] private int layerNumber = 0;
        // public AnimationClip clip;

        private IFrameCheckHandler _frameCheckHandler;
        private Animator _animator;
        private bool _checkedHitFrameStart;
        private bool _checkedHitFrameEnd;
        private bool _checkedReturnFrame;

        private bool _lastFrame;
        

        public void initialize(IFrameCheckHandler frameCheckHandler, Animator animator) {
            _frameCheckHandler = frameCheckHandler;
            _animator = animator;
            // totalFrames = Mathf.RoundToInt(clip.length * clip.frameRate);

            initCheck();
        }

        public void initCheck() {
                _checkedHitFrameStart = false;
                _checkedHitFrameEnd = false;
                _checkedReturnFrame = false;

                _lastFrame = false;
        }

        // public void CheckHitFrames() {
        //     if (!_checkedHitFrameStart && biggerOrEqualThanFrame(hitFrameStart)) {
        //         _frameCheckHandler.onHitFrameStart();
        //         _checkedHitFrameStart = true;
        //     } else if (!_checkedHitFrameEnd && biggerOrEqualThanFrame(hitFrameEnd)) {
        //         _frameCheckHandler.onHitFrameEnd();
        //         _checkedHitFrameEnd = true;
        // }

        // }

        // public void CheckEndFrame(){
        //     if (!_lastFrame && itsOnLastFrame()) {
        //         Debug.Log("last frame " + _lastFrame);
        //         _frameCheckHandler.onLastFrameStart();
        //         _lastFrame = true; // This is here so we don't skip the last frame
        //     }
        //     else if (_lastFrame) {
        //         _lastFrame = false;
        //         _frameCheckHandler.onLastFrameEnd();
        //     }
        // }

        public float GetPercentageFrameLoop(){
            return percentageOnFrame(returnFrame.ReturnToFrame);
        }

        public void CheckReturnFrame(){
            if (!_checkedReturnFrame && itsOnLastFrame()) {
                _checkedReturnFrame = true;
                _frameCheckHandler.onLoopFrame();

            }
        }



        // public bool itsOnLastFrame() {
        //     float percentage = _animator.normalizedTime(layerNumber);
        //     // Debug.Log(percentage);
        //     return (percentage > percentageOnFrame(totalFrames - 1));
        // }
        public bool itsOnLastFrame() {
            float percentage = _animator.normalizedTime(layerNumber);
            // Debug.Log(percentage);
            return (percentage > percentageOnFrame(totalFrames - 1) + 0.066f);
        }
        
        public bool biggerOrEqualThanFrame(int frameNumber) {
            float percentage = _animator.normalizedTime(layerNumber);
            return (percentage >= percentageOnFrame(frameNumber));
        }

        public bool biggerOrEqualThanFrame1TU(int frameNumber) {
            float percentage = _animator.normalizedTime(layerNumber);
            return (percentage >= percentageOnFrame(frameNumber) + 0.033f);
        }
        private float percentageOnFrame(int frameNumber) {
            return (float)frameNumber / totalFrames;
        }

    // #if UNITY_EDITOR
    // /// <summary>
    // /// This adds a button in the Inspector. Pressing it validates that all the
    // /// animator node names we reference are actually used by our Animator. We
    // /// can also show informational messages about problems with the configuration.
    // /// </summary>
    // [CustomEditor(typeof(FrameChecker))]
    // [CanEditMultipleObjects]
    // public class FrameCheckerEditor : UnityEditor.Editor
    // {
    //     private GUIStyle m_ErrorStyle = null;
    //     public override void OnInspectorGUI()
    //     {
    //         // let Unity do all the normal Inspector stuff...
    //         DrawDefaultInspector();

    //         // ... then we tack extra stuff on the bottom
    //         var frameChecker = (FrameChecker)target;
    //         if (!HasAudioSource(fx))
    //         {
    //             GUILayout.Label("No Audio Sources Connected!", GetErrorStyle());
    //         }

    //         if (GUILayout.Button("Validate Node Names"))
    //         {
    //             ValidateNodeNames(fx);
    //         }
    // }
    }
}