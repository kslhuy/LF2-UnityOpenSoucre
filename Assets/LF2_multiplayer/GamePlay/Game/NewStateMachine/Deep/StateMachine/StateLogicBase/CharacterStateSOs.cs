using UnityEngine;
using System.Collections.Generic;
using LF2.Data;

namespace LF2.Client{

    /// <summary>
    /// Abstract base class for playing back the visual feedback of Current State.
    /// </summary>
	[CreateAssetMenu(fileName = "New CharacterState", menuName = "State Machines/CharacterState")]

    public class CharacterStateSOs : ScriptableObject {
        public CharacterTypeEnum CharacterType;

        public StateLogicSO[] StatesSO ;

        public ComboSkill[] ListAllComboSkills ;
        // public StateLogicSO[] RunTimeComboSkills ;
        public StateLogicSOAddInfo RunTimeDDA ;
        public StateLogicSOAddInfo RunTimeDDJ ;
        public StateLogicSOAddInfo RunTimeDUA ;
        public StateLogicSOAddInfo RunTimeDUJ ;

        private Dictionary<StateType, StateLogicSO> m_StateLogicSOsMap ;
        public Dictionary<StateType, StateLogicSO> StateLogicSOsByType {
            get
                {
                    if( m_StateLogicSOsMap == null )
                    {
                        m_StateLogicSOsMap = new Dictionary<StateType, StateLogicSO>();
                        // Hoi bi rac roi cach viet
                        // co 1 list SkillsDescription o tren , lay tung cai 1 .
                        foreach (StateLogicSO stateLogicSO in StatesSO)
                        {
                            if (m_StateLogicSOsMap.ContainsKey(stateLogicSO.StateType))
                            {
                                throw new System.Exception($"Duplicate action definition detected: {stateLogicSO.StateType} in {CharacterType}");
                            }
                            m_StateLogicSOsMap[stateLogicSO.StateType] = stateLogicSO;
                        }
                    }
                    return m_StateLogicSOsMap;
                }
        }


        
   



	// 	[Header("-------Movement State-----------")]
        
    //     public MovementStateLogicSO[] MovementStatesSO ;
        
    //     [Header("-------Attack State-----------")]
        
    //     public AttackStateLogicSO[] AttackStatesSO ;

    //     [Header("-------Hurt State-----------")]
        
    //     public HurtStateLogicSO[] HurtStatesSO ;

    //     [Header("-------Special State-----------")]

    //     public SpecialStateLogicSO[] SpecStatesSO ;



    //     private Dictionary<StateType, AttackStateLogicSO> m_AttackSOsMap ;
    //     public Dictionary<StateType, AttackStateLogicSO> AttackSOsByType {
    //         get
    //             {
    //                 if( m_AttackSOsMap == null )
    //                 {
    //                     m_AttackSOsMap = new Dictionary<StateType, AttackStateLogicSO>();
    //                     // Hoi bi rac roi cach viet
    //                     // co 1 list SkillsDescription o tren , lay tung cai 1 .
    //                     foreach (AttackStateLogicSO stateLogicSO in AttackStatesSO)
    //                     {
    //                         if (m_AttackSOsMap.ContainsKey(stateLogicSO.StateType))
    //                         {
    //                             throw new System.Exception($"Duplicate action definition detected: {stateLogicSO.StateType}");
    //                         }
    //                         m_AttackSOsMap[stateLogicSO.StateType] = stateLogicSO;
    //                     }
    //                 }
    //                 return m_AttackSOsMap;
    //             }
    //     }

    //     private Dictionary<StateType, MovementStateLogicSO> m_MovementSOsMap ;
    //     public Dictionary<StateType, MovementStateLogicSO> MovementSOsByType {
    //         get
    //             {
    //                 if( m_MovementSOsMap == null )
    //                 {
    //                     m_MovementSOsMap = new Dictionary<StateType, MovementStateLogicSO>();
    //                     // Hoi bi rac roi cach viet
    //                     // co 1 list SkillsDescription o tren , lay tung cai 1 .
    //                     foreach (MovementStateLogicSO stateLogicSO in MovementStatesSO)
    //                     {
    //                         if (m_MovementSOsMap.ContainsKey(stateLogicSO.StateType))
    //                         {
    //                             throw new System.Exception($"Duplicate action definition detected: {stateLogicSO.StateType}");
    //                         }
    //                         m_MovementSOsMap[stateLogicSO.StateType] = stateLogicSO;
    //                     }
    //                 }
    //                 return m_MovementSOsMap;
    //             }
    //     }

    //     private Dictionary<StateType, HurtStateLogicSO> m_HurtSOsMap ;
    //     public Dictionary<StateType, HurtStateLogicSO> HurtSOsByType {
    //         get
    //             {
    //                 if( m_HurtSOsMap == null )
    //                 {
    //                     m_HurtSOsMap = new Dictionary<StateType, HurtStateLogicSO>();
    //                     // Hoi bi rac roi cach viet
    //                     // co 1 list SkillsDescription o tren , lay tung cai 1 .
    //                     foreach (HurtStateLogicSO stateLogicSO in HurtStatesSO)
    //                     {
    //                         if (m_HurtSOsMap.ContainsKey(stateLogicSO.StateType))
    //                         {
    //                             throw new System.Exception($"Duplicate action definition detected: {stateLogicSO.StateType}");
    //                         }
    //                         m_HurtSOsMap[stateLogicSO.StateType] = stateLogicSO;
    //                     }
    //                 }
    //                 return m_HurtSOsMap;
    //             }
    //     }

    //     private Dictionary<StateType, SpecialStateLogicSO> m_StateLogicSOsMap ;
    //     public Dictionary<StateType, SpecialStateLogicSO> SpecSOsByType {
    //         get
    //             {
    //                 if( m_StateLogicSOsMap == null )
    //                 {
    //                     m_StateLogicSOsMap = new Dictionary<StateType, SpecialStateLogicSO>();
    //                     // Hoi bi rac roi cach viet
    //                     // co 1 list SkillsDescription o tren , lay tung cai 1 .
    //                     foreach (SpecialStateLogicSO stateLogicSO in SpecStatesSO)
    //                     {
    //                         if (m_StateLogicSOsMap.ContainsKey(stateLogicSO.StateType))
    //                         {
    //                             throw new System.Exception($"Duplicate action definition detected: {stateLogicSO.StateType}");
    //                         }
    //                         m_StateLogicSOsMap[stateLogicSO.StateType] = stateLogicSO;
    //                     }
    //                 }
    //                 return m_StateLogicSOsMap;
    //             }
    //     }
   
   
	}
    
}
   