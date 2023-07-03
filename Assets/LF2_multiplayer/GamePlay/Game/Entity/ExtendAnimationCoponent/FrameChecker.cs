using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LF2
{
    [System.Serializable]
    public struct Interaction{
        public int damageAmount ; // INJURY
        public DamageEffect Effect;
        public byte fall ; 
        public byte Bdefend ; 

        public int vrest ;


        [Tooltip("How far the enemy falls backwards , upwards when hit")]
        public Vector3 Dirxyz;

    }
    [System.Serializable]
    public struct FrameStruct{
        // How far this frame can move 
        public Sprite sprite;
        public int dvx;
        public int dvy;
        public int dvz;
        // return to frame when press a key  
        public int pressed_A;
        
        public int pressed_J;

        public int pressed_D;
        public bool haveInteraction;
        public Interaction interaction;
        public GameObject SpwanObject;
        public AudioCueSO Sound;

        public int NextFrame;
        public int wait;

        public AnimationBox HitBox;
        public AnimationBox HurtBox;
        
    }

    [Serializable]
    public struct AnimationBox {
        public Vector3 size;
        public Vector3 center;
    }




    [Serializable]
    public class FrameChecker
    {
        public AnimationClip clip;
        public int totalFrames;
        public float length {private set ; get;}
        

        // 	If you want to hit a single enemy, use arest. 
        // For multiple enemies, you need vrest. The values of these tags are the duration in TU that the object must wait before hitting the character again.
        [Tooltip("If you want to hit a single enemy, use arest.")]
        public bool arest;
        [Tooltip("For multiple enemies, you need vrest. The values of these tags are the duration in TU that the object must wait before hitting the character again.")]
        public float vrest ;

        public FrameStruct[] _frameStruct;


        
        [HideInInspector]
        [SerializeField] private int layerNumber = 0;

        private IFrameCheckHandler _frameCheckHandler;
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;

        

        private int _frameNow;
        private float _tickNow;
        private int _timerAttack;

        private AttackDataSend Atk_data = new AttackDataSend();


        // Call in Awake for state that use this Animation Methode  
        public void initialize(IFrameCheckHandler frameCheckHandler, Animator animator,SpriteRenderer sp)
        {
            _frameCheckHandler = frameCheckHandler;
            _animator = animator;
            _spriteRenderer = sp;
            
            // initCheck();
        }

        public void InitializeVFX(SpriteRenderer sp)
        {
            _spriteRenderer = sp;

            // initCheck();
        }
        public void initialize(IFrameCheckHandler frameCheckHandler,SpriteRenderer sp)
        {
            _frameCheckHandler = frameCheckHandler;
            _spriteRenderer = sp;
            
            // initCheck();
        }

        // Get Data from existing Animation Clip to initialize FramStruct 
#if UNITY_EDITOR

        public void InitializeFrameStruct()
        {
            if (!clip) return;
            totalFrames = Mathf.RoundToInt(clip.length * clip.frameRate);
            length = clip.length;
            _frameStruct = new FrameStruct[totalFrames];
            var sprites = new List<Sprite> ();
            // var times = new List<float> ();


            foreach(var binding in AnimationUtility.GetObjectReferenceCurveBindings(clip))
            {
                var keyframes = AnimationUtility.GetObjectReferenceCurve (clip, binding);
                foreach(var frame in keyframes)
                {
                    sprites.Add((Sprite) frame.value);
                    // times.Add(frame.time);
                }
            }
            for (int i=0 ; i<totalFrames ; i++){
                _frameStruct[i].sprite = sprites[i]; 
                // _frameStruct[i].time = times[i]; 
                _frameStruct[i].wait = 1; 

                _frameStruct[i].NextFrame = i + 1; 
                if (i == totalFrames -1 ){
                    _frameStruct[i].NextFrame = 999;
                }
            }
            
        }
#endif 
        //First Frame 
        public void initCheck(int renderFrame)
        {
            _tickNow = 0;
            _frameNow = renderFrame - 1;
            _timerAttack = _frameNow;
            _spriteRenderer.sprite =_frameStruct[_frameNow].sprite;

        }


        public void CheckFrame(Action endState ){

            if (_frameNow != 999 && (_tickNow % (_frameStruct[_frameNow].wait + 1)) == 0) {
                // _timeNow = Time.time ;
                _tickNow = 0;
                _frameNow = _frameStruct[_frameNow].NextFrame;
                if(_frameNow == 999) {
                    endState.Invoke();
                    return;
                }
                _spriteRenderer.sprite =_frameStruct[_frameNow].sprite;
                if (_frameStruct[_frameNow].Sound){
                    _frameCheckHandler.playSound(_frameStruct[_frameNow].Sound);
                }
                _frameCheckHandler.onMoveFrame(_frameNow);
            }
            // increment timer attack 
            _timerAttack ++;
            _tickNow++;
            // The values of these tags are the duration in TU that the object must wait before hitting the character again.
            if (_frameStruct[_frameNow].haveInteraction) {
                if (arest){
                    Atk_data.Amount_injury = _frameStruct[_frameNow].interaction.damageAmount;
                    Atk_data.Direction = _frameStruct[_frameNow].interaction.Dirxyz;
                    Atk_data.BDefense_p = _frameStruct[_frameNow].interaction.Bdefend;
                    Atk_data.Fall_p = _frameStruct[_frameNow].interaction.fall;
                    Atk_data.Effect = (byte)_frameStruct[_frameNow].interaction.Effect;
                    // Debug.Log("ON attack frame");
                    _frameCheckHandler.onAttackFrame(Atk_data);    
                }else if (_timerAttack >= vrest){      
                    _timerAttack = 0;

                    Atk_data.Amount_injury = _frameStruct[_frameNow].interaction.damageAmount;
                    Atk_data.Direction = _frameStruct[_frameNow].interaction.Dirxyz;
                    Atk_data.BDefense_p = _frameStruct[_frameNow].interaction.Bdefend;
                    Atk_data.Fall_p = _frameStruct[_frameNow].interaction.fall;
                    Atk_data.Effect = (byte)_frameStruct[_frameNow].interaction.Effect;
                    // Debug.Log("ON attack frame");
                    _frameCheckHandler.onAttackFrame(Atk_data);
                }
            }
            
        }

        public void CheckFrameVFX(Action endState ){
            
            if (_frameNow != 999 && (_tickNow % (_frameStruct[_frameNow].wait + 1)) == 0) {
                // _timeNow = Time.time ;
                _tickNow = 0;
                _frameNow = _frameStruct[_frameNow].NextFrame;
                if(_frameNow == 999) {
                    endState.Invoke();
                    return;
                }
                _spriteRenderer.sprite =_frameStruct[_frameNow].sprite;
                // if (_frameStruct[_frameNow].Sound){
                //     _frameCheckHandler.playSound(_frameStruct[_frameNow].Sound);
                // }
                // _frameCheckHandler.onMoveFrame(_frameNow);
            }
            // increment timer attack 
            // _timerAttack ++;
            _tickNow++;
            // The values of these tags are the duration in TU that the object must wait before hitting the character again.
            // if (_frameStruct[_frameNow].haveInteraction) {
            //     if (arest){
            //         Atk_data.Amount_injury = _frameStruct[_frameNow].interaction.damageAmount;
            //         Atk_data.Direction = _frameStruct[_frameNow].interaction.Dirxyz;
            //         Atk_data.BDefense_p = _frameStruct[_frameNow].interaction.Bdefend;
            //         Atk_data.Fall_p = _frameStruct[_frameNow].interaction.fall;
            //         Atk_data.Effect = (byte)_frameStruct[_frameNow].interaction.Effect;
            //         // Debug.Log("ON attack frame");
            //         _frameCheckHandler.onAttackFrame(Atk_data);    
            //     }else if (_timerAttack >= vrest){      
            //         _timerAttack = 0;

            //         Atk_data.Amount_injury = _frameStruct[_frameNow].interaction.damageAmount;
            //         Atk_data.Direction = _frameStruct[_frameNow].interaction.Dirxyz;
            //         Atk_data.BDefense_p = _frameStruct[_frameNow].interaction.Bdefend;
            //         Atk_data.Fall_p = _frameStruct[_frameNow].interaction.fall;
            //         Atk_data.Effect = (byte)_frameStruct[_frameNow].interaction.Effect;
            //         // Debug.Log("ON attack frame");
            //         _frameCheckHandler.onAttackFrame(Atk_data);
            //     }
            // }
            
        }

        public void CheckTransition(StateType stateType ){
            if (stateType ==  StateType.Attack){
                _frameNow = _frameStruct[_frameNow].pressed_A;
            }
            else if (stateType ==  StateType.Defense){
                _frameNow = _frameStruct[_frameNow].pressed_D;
            }
            else if (stateType ==  StateType.Jump){
                _frameNow = _frameStruct[_frameNow].pressed_J;
            }else{
                return;
            }
            // Debug.Log(_frameNow);
            // _timeNow = Time.time ;
            _spriteRenderer.sprite =_frameStruct[_frameNow].sprite;
            _frameCheckHandler.onMoveFrame(_frameNow);

        }

#region NotUse
    
        
        public float percentageAnimationNow(){
            return _animator.normalizedTime(layerNumber);

        }

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

#endregion


    }
}