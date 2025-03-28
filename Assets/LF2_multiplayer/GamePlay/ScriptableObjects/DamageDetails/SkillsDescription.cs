
using System;
using System.Collections.Generic;
using LF2.Client;
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

        public StateLogicSO[] SubStateLogicSO;

        // public bool UseMana;
        public int ManaCost;

        [Header("----- DAmage ------")]
        // [ExtendEditorSOs]
        public DamageDetails[] DamageDetails;
        // public Interaction[] Interactions;

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
        
        [Tooltip("mesure by total frame in the animation ")]
        public int Duration;


        public bool expirable;
        [Tooltip("After this Action is successfully started, the server will discard any attempts to perform it again until this amount of time has elapsed.")]

        public float ReuseTimeSeconds;

        public bool ActionInterruptible = true;


        public AudioCueSO Sounds;
        public float delayLoopSound;


        public AudioCueSO[] Start_Sounds;


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

        [Header("----- Animation ------")]

        public FrameChecker frameChecker;

        // [SerializeField] private bool refeshFramCheckerOnce ;




        private void OnValidate() {
            
            if (Logic == StateLogic.Attack ){
                for( int i = 0; i < DamageDetails.Length; i++ ){
                    if (DamageDetails[i] == null ) {
                        Debug.LogWarning($"You may forgot SO_DamageDetails in  {StateType} SOs ");
                        break;
                    }

                }
            }
            if (Logic != StateLogic.Movement && DurationSeconds == 0){
                Debug.LogWarning($"You may forgot setting DurationSeconds in  {StateType} SOs ");
            }
            Duration = (int)(Math.Round(DurationSeconds / 0.016667f));

            // if (DamageDetails.Length > 0 ){
            //     Debug.Log(Logic);
            //     Interactions = new Interaction[1];
            //     Interactions[0].Bdefend = DamageDetails[0].Bdefend;
            //     Interactions[0].fall = DamageDetails[0].fall;
            //     Interactions[0].Dirxyz = DamageDetails[0].Dirxyz;
            //     Interactions[0].Effect = DamageDetails[0].Effect;
            //     Interactions[0].damageAmount = DamageDetails[0].damageAmount;
            // }
            

            

            // if (!refeshFramCheckerOnce && frameChecker != null  ){
            //     refeshFramCheckerOnce = !refeshFramCheckerOnce;
            //     frameChecker.initialize();
            //     DurationSeconds = frameChecker.length;
            // }

        }

    }
}

