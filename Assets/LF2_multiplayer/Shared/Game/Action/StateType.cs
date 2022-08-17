
namespace LF2{

    public enum MovementStateType{
        Idle,

        Walk ,
        Run,
        Jump,
        DoubleJump1,
        DoubleJump2,


        Land ,
        Crouch,  // Same animation with Land but can not double jump
        Air,

        
        Sliding,
        Rolling,

    }

    public enum AttackStateType{
        Attack1,
        Attack2,
        Attack3,
        AttackRun,

        AttackJump,
        AttackJump2,

    }

    public enum HurtStateType{
        Hurt1,
        Hurt2Front,
        Hurt2Back,
        //
        FallingFront,
        FallingBack,
        //
        LayingFront,
        LayingBack,

        //
        DefenseHit,

        BrokenDefense,
        DOP , // Dance of Pain


    }

    public enum SpecialStateType{
        
        /// DUA
        DUA1,
        
        DUA2,
        DUA3,

        // DDA 
        DDA1,
        DDA2,
        DDA3 ,

        // DDJ
        DDJ1,
        DDJ2,
        DDJ3,

        // DUJ
        DUJ1,
        DUJ2,
        DUJ3,

        // DJA
        DJA , // ultimate 
    }

    public enum StateType
    {
        Idle,//0
        Move,
        Run,
        Jump,//3

        DoubleJump,

        Land,
        Crouch,

        Defense,
        Attack, // 8
        AttackJump,
        Air,

        Ground,
        Sliding,
        Rolling,


        Falling,
        Laying,
        AttackRun,

        Attack2, // 17
        Attack3,
        DOP , // Dance of Pain

        BrokenDefense,
        DefenseHit,
        Hurt1,// 22
        Hurt2Front,// 23
        Hurt2Back, // 24
        FallingFront,// 25
        FallingBack,
        LayingFront,
        LayingBack, // 28

        DoubleJump2,
        DDA1,


        DDA2,
        DDA3 ,
        DDJ1, // 33
        DDJ2,
        DDJ3,
        DUJ1,//36

        DUJ2,
        DUJ3,
        DUA1,//39
        
        DUA2,
        DUA3,

        NONE,
        AmorAbsord,
    }

}