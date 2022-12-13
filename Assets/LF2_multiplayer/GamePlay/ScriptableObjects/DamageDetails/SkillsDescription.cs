
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LF2{
    /// <summary>
    /// Data description of a single State of sigle Player 
    // (For exemple Attack of David), including the information to visualize it (animations etc), and the information
    /// to play it back on the server.
    /// </summary>


    [CreateAssetMenu(fileName = "SkillsDescription", menuName = "SkillsDescription")]
    [System.Serializable]
    public class SkillsDescription : ScriptableObject
    {
        public StateType StateType; 
        public StateLogic Logic;

        public bool UseMana;
        public int ManaCost;

        [Header("----- DAmage ------")]
        // [ExtendEditorSOs]
        public DamageDetails[] DamageDetails;

        [Header("----- Range ------")]

        // Moves foward/upwards/downwards how much 
        public float Dx;
        public float Dy;
        public float Dz;

        public AnimationCurve curve;

        [Tooltip("If this Action spawns a projectile, describes it. (\"Charged\" projectiles can list multiple possible shots, ordered from weakest to strongest)")]
        public ProjectileInfo[] Projectiles;
        // public AnimationCurve animationCurve;
        [Header("----- Timer ------")]
        public float DurationSeconds;

        public bool expirable;
        [Tooltip("After this Action is successfully started, the server will discard any attempts to perform it again until this amount of time has elapsed.")]

        public float ReuseTimeSeconds;


        // public float maxRange;

        public bool ActionInterruptible = true;
        public AudioCueSO Sounds;
        public float delayLoopSound;


        public AudioCueSO[] Start_Sounds;





        // [Serializable]
        // public class VizAnimConfig {
        //     [Tooltip("The primary Animation trigger that gets raised when visualizing this Action")]
        //     public string AnimNames ;
        //     [HideInInspector]
        //     public int AnimHashId ;

        // }


        [Serializable]
        public class ProjectileInfo
        {
            [Tooltip("Prefab used for the projectile")]
            public GameObject ProjectilePrefab;
            public Vector3 pivot ;

        }
        [Serializable]
        public struct SpawnsObject{
            public GameObject _Object;
            public Vector3 pivot ;

        } 
        public SpawnsObject[] SpawnsFX;

        // [SerializeField] int ComboPriorty = 0; //the more complicated the move the higher the Priorty

        [Header("----- Animation ------")]

        public FrameChecker frameChecker;




        private  void OnValidate() {

            if (Logic == StateLogic.Attack )
            {
                for( int i = 0; i < DamageDetails.Length; i++ ){
                    if (DamageDetails[i] == null ) {
                        Debug.LogWarning($"You may forgot SO_DamageDetails in  {StateType} SOs ");
                        break;
                    }
                    if (DamageDetails[i].m_AnimationName == null) {
                        throw new System.Exception($"Missing name animation for DamageDetails in state {StateType} at position {i} ");
                    }

                    DamageDetails[i].AnimationNameHash = Animator.StringToHash(DamageDetails[i].m_AnimationName);
                }
            }
            if (Logic != StateLogic.Movement && DurationSeconds == 0){
                Debug.LogWarning($"You may forgot setting DurationSeconds in  {StateType} SOs ");
            }
            // if (vizAnim != null){
            //     for( int i = 0; i < vizAnim.Length; i++ ){
            //         if (vizAnim[i] == null )                         
            //             throw new System.Exception($"Missing Animation in state {StateType}"); 
            //         // Debug.Log(i);
            //         vizAnim[i].AnimHashId = Animator.StringToHash(vizAnim[i].AnimNames);
            //     }

            // }

        }

    }
}

