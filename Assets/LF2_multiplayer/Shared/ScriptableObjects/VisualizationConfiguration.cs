using UnityEngine;

namespace LF2
{
    /// <summary>
    /// Describes how a specific character visualization should be animated.
    /// </summary>
    [CreateAssetMenu]
    public class VisualizationConfiguration : ScriptableObject
    {
    #region StringName
        
    [Header("------------ Animation -------------")]

    [Header("------------ Movement -------------")]
    [SerializeField] string Idle = "Idle_anim";
    [SerializeField] string Walk = "Walk_anim";
    [SerializeField] string Run = "Run_anim";
    [SerializeField] string Jump = "Jump_anim";
    [SerializeField] string DoubleJump = "DoubleJump_anim";
    [SerializeField] string DoubleJump2 = "DoubleJump2_anim";
    [SerializeField] string Land = "Land_anim";
    [SerializeField] string Defense = "Defense_anim";
    [SerializeField] string Sliding = "Sliding_anim";

    [SerializeField] string Rolling = "Rolling_anim";
    [SerializeField] string Air = "Air_anim"; 

    [Header("------------ Attack -------------")]
    [SerializeField] string Attack1 = "Attack1_anim";

    [SerializeField] string Attack2 = "Attack2_anim";
    [SerializeField] string Attack3 = "Attack3_anim";

    [SerializeField] string RunAttack = "Run_Attack_anim";
    [SerializeField] string JumpAttack = "Jump_Attack_anim";

    
    [Header("------------ Hurt -------------")]
    [SerializeField] string Hurt_1 = "Hurt1";

    [SerializeField] string Hurt_2_Back = "Hurt2Back";
    [SerializeField] string Hurt_2_Front = "Hurt2Front";


    [SerializeField] string Fall_Back = "Fall_Back";

    [SerializeField] string Fall_Front = "Fall_Front";

    [SerializeField] string Laying_Back = "LayingBack";

    [SerializeField] string Laying_Front = "LayingFront";
    
    [Header("------------ DDA -------------")]
    [SerializeField] string DDA_1 = "DDA_1_anim";
    [SerializeField] string DDA_2 = "DDA_2_anim";
    [SerializeField] string DDA_3 = "DDA_3_anim";
    [Header("------------ DDJ -------------")]
    [SerializeField] string DDJ_1 = "DDJ_1_anim";
    [SerializeField] string DDJ_2 = "DDJ_2_anim";
    [SerializeField] string DDJ_3 = "DDJ_3_anim";
    [Header("------------ DUA -------------")]
    [SerializeField] string DUA_1 = "DUA_1_anim";
    [SerializeField] string DUA_2 = "DUA_2_anim";
    [SerializeField] string DUA_3 = "DUA_3_anim";

    [Header("------------ DUJ -------------")]
    [SerializeField] string DUJ_1 = "DUJ_1_anim";
    [SerializeField] string DUJ_2 = "DUJ_2_anim";
    [SerializeField] string DUJ_3 = "DUJ_3_anim";

    [SerializeField] string Empty = "Empty_anim";



    #endregion

    // These are maintained by our OnValidate(). Code refers to these hashed values, not the string versions!
    #region HashID
        
    [SerializeField] [HideInInspector] public int a_Idle;
    [SerializeField] [HideInInspector] public int a_Walk;
    [SerializeField] [HideInInspector] public int a_Run;
    [SerializeField] [HideInInspector] public int a_Land;
    [SerializeField] [HideInInspector] public int a_Sliding;
    [SerializeField] [HideInInspector] public int a_Rolling;
    
    [SerializeField] [HideInInspector] public int a_Jump;
    [SerializeField] [HideInInspector] public int a_DoubleJump_1;
    
    [SerializeField] [HideInInspector] public int a_DoubleJump_2;

    [SerializeField] [HideInInspector] public int a_Air;

    // [SerializeField] [HideInInspector] public int a_Rolling;
            //-----------------------------
    [SerializeField] [HideInInspector] public int a_Attack_1;
    [SerializeField] [HideInInspector] public int a_Attack_2;
    [SerializeField] [HideInInspector] public int a_Attack_3;

    
    [SerializeField] [HideInInspector] public int a_Jump_Attack;
    [SerializeField] [HideInInspector] public int a_Run_Attack;



        //-----------------------------
    [SerializeField] [HideInInspector] public int a_Hurt_1;
    [SerializeField] [HideInInspector] public int a_Hurt_2_Front;
    [SerializeField] [HideInInspector] public int a_Hurt_2_Back;

    
    [SerializeField] [HideInInspector] public int a_Fall_Back;
    [SerializeField] [HideInInspector] public int a_Fall_Front;

    [SerializeField] [HideInInspector] public int a_Laying_Back;
    [SerializeField] [HideInInspector] public int a_Laying_Front;
    //-----------------------------

    [SerializeField] [HideInInspector] public int a_DDA_1;
    [SerializeField] [HideInInspector] public int a_DDA_2;
    [SerializeField] [HideInInspector] public int a_DDA_3;
    //-----------------------------
    [SerializeField] [HideInInspector] public int a_DDJ_1;
    [SerializeField] [HideInInspector] public int a_DDJ_2;
    [SerializeField] [HideInInspector] public int a_DDJ_3;
    //-----------------------------
    [SerializeField] [HideInInspector] public int a_DUA_1;
    [SerializeField] [HideInInspector] public int a_DUA_2;
    [SerializeField] [HideInInspector] public int a_DUA_3;
    //-----------------------------
    [SerializeField] [HideInInspector] public int a_DUJ_1;
    [SerializeField] [HideInInspector] public int a_DUJ_2;
    [SerializeField] [HideInInspector] public int a_DUJ_3;

    [SerializeField] [HideInInspector] public int a_Empty;

    #endregion

    void OnValidate()
    {
        a_Idle = Animator.StringToHash(Idle);
        a_Walk= Animator.StringToHash(Walk);
        a_Run =  Animator.StringToHash(Run);
        a_Land= Animator.StringToHash(Land);
        a_Sliding= Animator.StringToHash(Sliding);
        a_Rolling= Animator.StringToHash(Rolling);

        a_Jump= Animator.StringToHash(Jump);
        a_DoubleJump_1 = Animator.StringToHash(DoubleJump);
        a_DoubleJump_2 = Animator.StringToHash(DoubleJump2);

        // -----------------------------------------


        a_Attack_1 = Animator.StringToHash(Attack1);

        a_Attack_2 = Animator.StringToHash(Attack2);

        a_Attack_3 = Animator.StringToHash(Attack3);

        a_Jump_Attack = Animator.StringToHash(JumpAttack);

        a_Run_Attack = Animator.StringToHash(RunAttack);



        //-----------------------------
        a_Hurt_1= Animator.StringToHash(Hurt_1);
        a_Hurt_2_Back= Animator.StringToHash(Hurt_2_Back);
        a_Hurt_2_Front= Animator.StringToHash(Hurt_2_Front);

        a_Fall_Back= Animator.StringToHash(Fall_Back);
        a_Fall_Front= Animator.StringToHash(Fall_Front);

        a_Laying_Back= Animator.StringToHash(Laying_Back);
        a_Laying_Front= Animator.StringToHash(Laying_Front);
    //-----------------------------

        a_DDA_1= Animator.StringToHash(DDA_1);
        a_DDA_2= Animator.StringToHash(DDA_2);
        a_DDA_3= Animator.StringToHash(DDA_3);
    //-----------------------------
        a_DDJ_1= Animator.StringToHash(DDJ_1);
        a_DDJ_2= Animator.StringToHash(DDJ_2);
        a_DDJ_3= Animator.StringToHash(DDJ_3);
    //-----------------------------
        a_DUA_1= Animator.StringToHash(DUA_1);
        a_DUA_2= Animator.StringToHash(DUA_2);
        a_DUA_3= Animator.StringToHash(DUA_3);
    //-----------------------------
        a_DUJ_1= Animator.StringToHash(DUJ_1);
        a_DUJ_2= Animator.StringToHash(DUJ_2);
        a_DUJ_3= Animator.StringToHash(DUJ_3);


        a_Empty = Animator.StringToHash(Empty);
        }
    }
}
