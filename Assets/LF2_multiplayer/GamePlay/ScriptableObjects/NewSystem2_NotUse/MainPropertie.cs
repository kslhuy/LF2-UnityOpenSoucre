using System.Collections.Generic;
using UnityEngine;
namespace LF2 {


    [CreateAssetMenu(fileName = "NewMainPropertie", menuName = "GameData/MainPropertie", order = 0)]
    public class MainPropertie : ScriptableObject {
        [Header("Main Properties")]
        public string char_name;
        public Sprite head;
        public Sprite small;
        // public PlayerType playerType;
        public int subs;
        public float walking_speed;
        public float walking_speedz;
        public float running_speed;
        public float running_speedz;
        public float jump_distance;
        public float dash_distance;
        public int shp;
        public int smp;
        public float intervalDoubleTapRunning;

        public Frame[] frames ;
        public Frame actualFrame;

        // public LF2Character owner;
        // private SpriteRenderer spriteRenderer;
        private AudioSource audioSource;
        private Rigidbody rb;
        private GameObject hurtBox;
        private GameObject hitBox;
        private BoxCollider boxCollider;
        public GameObject mugshot;

        public float frameTimer;
        public bool execOpointOneTime;
        public bool execRecoverManaOneTime;
        public bool execAudioOneTime;
        public float inertiaMoveHorizontal;
        public float inertiaMoveVertical;
        public float constantGravity;
        public bool lockRightForce;

        //RUNNING
        public bool stepOneRunningRightEnabled;
        public bool stepOneRunningLeftEnabled;
        public float runningCountTapRight;
        public float runningCountTapLeft;

        //Disabled variables
        public bool isRunningEnabled;
        public bool isWalkingEnabled;

        //Trigger
        public float moveHorizontal;
        public float moveVertical;
        public bool moveHorizontalDown;
        public bool moveHorizontalUp;

        public bool pressForwardAttackDown;
        public bool pressUpAttackDown;
        public bool pressDownAttackDown;

        public bool pressDefenseDown;
        public bool pressWeaponDown;
        public bool pressAttackDown;
        public bool pressJumpDown;
        public bool pressFrameShortcut;
        public bool shortcutFrameEnabled;
        public bool onGround;
        public bool onWall;
        public bool enableInjured;
        public bool isInjured;

        //Combination: <^v> ADJW
        public bool jump_defense;//DashDefense
        public bool weapon_defense;
        public bool weapon_jump;
        public bool defense_up_attack;
        public bool defense_up_jump;
        public bool defense_forward_attack;
        public bool defense_down_attack;
        public bool defense_jump_attack;
        public bool defense_forward_jump;

        public bool selfOpointAffectedFrame;
        public int ownerFrame;
        // private bool flipOneTimeForFrame = true;

        public bool isLyingDown;
        public bool isFacingRight;
        public bool isDead = false;
        public bool isAttacking;

        public Vector3 opointPosition;

        //Params
        public bool isDummy;
        public bool stopEditorByFrame;
        public int stopEditorInFrame;
        public int shortcutFrame;
        public float yGroundDebug = -0.01f;
        public Itr externalItr;
        public bool sameExternalItr;
        public float damageRestTimer;
        public StateType lastButtonPressed = StateType.NONE;
        public string[] buttonsPressed = new string[3];

        public int initialHP;
        public int initialMP;

        //Id
        public TeamType team;
        public string objName;
        public int actualFrameNumber;
        public int actualNextFrame;
        public string actualState;
        // private List<Transform> enemies = new List<Transform>();
        // private List<Transform> alies = new List<Transform>();

    }
}