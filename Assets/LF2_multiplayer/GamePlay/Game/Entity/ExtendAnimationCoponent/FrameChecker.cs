using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LF2
{

    [System.Serializable]
    public struct FrameStruct{
        // How far this frame can move 
        public bool UseThisFrame;
        
        public int dvx;
        public int dvy;
        public int dvz;
        public DamageDetails damageDetails;
        public GameObject SpwanObject;
        public AudioCueSO Sound;

        // transition happen when some condition valide by current state  
        // USe for animation that need to loop back  
        // like Dennis 
        public int ReturnToFrame;

    }


    [System.Serializable]
    public class FrameChecker
    {
        public AnimationClip clip;
        public int totalFrames;
        public float length;


        public FrameStruct[] _frameStruct;


        
        [HideInInspector]
        [SerializeField] private int layerNumber = 0;

        private IFrameCheckHandler _frameCheckHandler;
        private Animator _animator;

        
        private bool _checkedReturnFrame;

        private bool _IsOnlastFrame;

        private bool initOnce;

        public void initialize(IFrameCheckHandler frameCheckHandler, Animator animator)
        {
            _frameCheckHandler = frameCheckHandler;
            _animator = animator;

            initCheck();
        }

        public void initialize()
        {
            if (!initOnce){
                initOnce = !initOnce;                 
                totalFrames = Mathf.RoundToInt(clip.length * clip.frameRate);
                length = clip.length;
                _frameStruct = new FrameStruct[totalFrames];
            }

        }

        public void initCheck()
        {
            _checkedReturnFrame = false;
            _IsOnlastFrame = false;
        }



        public void CheckReturnFrame()
        {
            if (!_checkedReturnFrame && itsOnLastFrame())
            {
                _checkedReturnFrame = true;
                _frameCheckHandler.onLoopFrame();

            }
        }



        // public bool itsOnLastFrame() {
        //     float percentage = _animator.normalizedTime(layerNumber);
        //     // Debug.Log(percentage);
        //     return (percentage > percentageOnFrame(totalFrames - 1));
        // }
        public bool itsOnLastFrame()
        {
            float percentage = _animator.normalizedTime(layerNumber);
            // Debug.Log(percentage);
            return (percentage > percentageOnFrame(totalFrames - 1) + 0.033f);
        }

        public bool biggerOrEqualThanFrame(int frameNumber)
        {
            float percentage = _animator.normalizedTime(layerNumber);
            return (percentage >= percentageOnFrame(frameNumber));
        }

        public bool biggerOrEqualThanFrame1TU(int frameNumber)
        {
            float percentage = _animator.normalizedTime(layerNumber);
            return (percentage >= percentageOnFrame(frameNumber) + 0.033f);
        }
        public float percentageOnFrame(int frameNumber)
        {
            return (float)frameNumber / totalFrames;
        }


    }
}