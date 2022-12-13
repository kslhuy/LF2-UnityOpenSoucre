using UnityEngine;

namespace LF2 {
    [CreateAssetMenu(fileName = "Frame", menuName = "NewSysAnimation/Frame")]
    public class Frame : ScriptableObject {
        
        [Header("Main Properties")]
        public int number;
        [HideInInspector]
        public string description;
        public Sprite pic;
        [HideInInspector]
        public AudioClip audio;
        [HideInInspector]
        public bool isActive;
        [HideInInspector]
        public float wait;
        [HideInInspector]
        public int next;
        [HideInInspector]
        public float dvx;
        [HideInInspector]
        public float dvy;
        [HideInInspector]
        public float dvz;
        [HideInInspector]
        public bool teleportToEnemy;
        [HideInInspector]
        public bool teleportToAlly;
        [HideInInspector]
        public bool disableBdy;
        [HideInInspector]
        public bool enableItr;
        [HideInInspector]
        public bool showBdyInEditor;
        [HideInInspector]
        public bool useEditorBdy;
        [HideInInspector]
        public bool showItrInEditor;
        [HideInInspector]
        public bool useEditorItr;
        [HideInInspector]
        public AudioClip sound;

        [HideInInspector]
        [Header("Bodies")]
        [SerializeField]
        public Bdy bdy;

        [SerializeField]
        public Bdy[] bdys;

        [Header("Objects Point")]
        [SerializeField]
        public Opoint[] opoints;

        [HideInInspector]
        [Header("Interactions")]
        [SerializeField]
        public Itr itr;

        [SerializeField]
        public Itr[] itrs;

        [HideInInspector]
        [Header("Catch Point")]
        [SerializeField]
        public Cpoint cpoint;

        //Inertia movement
        [HideInInspector]
        public bool hasHorizontalMovement; //Get value
        [HideInInspector]
        public bool useHorizontalInertia; //Lock value used
        [HideInInspector]
        public bool hasVerticalMovement; //Get value
        [HideInInspector]
        public bool useVerticalInertia; //Lock value used

        //Gravity
        [HideInInspector]
        public bool useConstantGravity; //Lock gravity value used

        //Flip frame
        [HideInInspector]
        public bool canFlip;
        [HideInInspector]
        public bool flip;
        [HideInInspector]
        public bool useSharedOpointPosition;
        [HideInInspector]
        public bool disableFlipInterference;
        [HideInInspector]
        public bool lockDirectionForce;
        [HideInInspector]
        public bool useEnemyForceInNextFrame;
        [HideInInspector]
        public bool stopGravity;
        [HideInInspector]
        public int recoverMP;

        //Triggers
        [HideInInspector]
        public int deathFrame;
        [HideInInspector]
        public int defendeButtonFrame;
        [HideInInspector]
        public int attackButtonFrame;
        [HideInInspector]
        public int jumpButtonFrame;
        [HideInInspector]
        public int weaponButtonFrame;
        [HideInInspector]
        public int groundFrame;
        [HideInInspector]
        public int flyFrame;
        [HideInInspector]
        public int wallFrame;
        [HideInInspector]
        public int forwardAttackFrame;
        [HideInInspector]
        public int upAttackFrame;
        [HideInInspector]
        public int downAttackFrame;
        [HideInInspector]
        public int walkingFrame;
        [HideInInspector]
        public int runningFrame;
        [HideInInspector]
        public bool disableOpoints;
        
        //Combinations
        [HideInInspector]
        public bool showCombinations;
        [HideInInspector]
        public int jumpDefenseButtonFrame;
        [HideInInspector]
        public int defenseJumpAttackFrame;
        [HideInInspector]
        public int defenseForwardJumpFrame;
        [HideInInspector]
        public int defenseForwardAttackFrame;
        [HideInInspector]
        public int defenseUpAttackFrame;
        [HideInInspector]
        public int defenseUpJumpFrame;
        [HideInInspector]
        public int weaponDefense;
        [HideInInspector]
        public int weaponJump;

        public float getWait(){
            //Tranform a second in TU = 1/30
            return wait / 30f;
        }
    }
}
