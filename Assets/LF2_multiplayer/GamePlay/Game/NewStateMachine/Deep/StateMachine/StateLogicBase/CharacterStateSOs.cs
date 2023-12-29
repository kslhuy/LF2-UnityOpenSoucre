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

        public ComboSkill[] ListAllComboSkills;
        // public StateLogicSO[] RunTimeComboSkills ;
        
        [Header("----------Runtime State-----------")]


        public ComboSkill RunTimeDDA ;
        public ComboSkill RunTimeDDJ ;
        public ComboSkill RunTimeDUA ;
        public ComboSkill RunTimeDUJ ;

        [Header("----------Defaut-----------")]


        public ComboSkill DefautDDA ;
        public ComboSkill DefautDDJ ;
        public ComboSkill DefautDUA ;
        public ComboSkill DefautDUJ ;


        

        private Dictionary<StateType, StateLogicSO> m_StateLogicSOsMap ;
        public Dictionary<StateType, StateLogicSO> StateLogicSOsByType {
            get
                {
                    if( m_StateLogicSOsMap == null )
                    {
                        m_StateLogicSOsMap = new Dictionary<StateType, StateLogicSO>();
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


        // private Dictionary<StateType, ComboSkill> m_ComboSOsMap ;
        // public Dictionary<StateType, ComboSkill> ComboSOsByType {
        //     get
        //         {
        //             if( m_ComboSOsMap == null )
        //             {
        //                 m_ComboSOsMap = new Dictionary<StateType, ComboSkill>();
        //                 // Hoi bi rac roi cach viet
        //                 // co 1 list SkillsDescription o tren , lay tung cai 1 .
        //                 foreach (ComboSkill comboSO in ListAllComboSkills)
        //                 {
        //                     if (m_ComboSOsMap.ContainsKey(comboSO.StateType))
        //                     {
        //                         throw new System.Exception($"Duplicate action definition detected: {comboSO.StateType} in {CharacterType}");
        //                     }
        //                     m_ComboSOsMap[comboSO.StateType] = comboSO;
        //                 }
        //             }
        //             return m_ComboSOsMap;
        //         }
        // }

        // Trong trương hợp người chơi quên set Skill trước trận đấu ,
        //  hay có như cầu thay đổi Skill cho phù hợp với trận đấu đang diễn ra , 
        //  Người chơi phải trừ tiền (vàng kiếm được trong game)

        public StateLogicSO Get_StateLogicSO(StateType stateType, SkillNumber skillNumber){
            if (RunTimeDDA.StateType == stateType)
            {
                return _getStateLogic(RunTimeDDA , skillNumber);
            }
            else if (RunTimeDDJ.StateType == stateType){
                return _getStateLogic(RunTimeDDJ , skillNumber);

            }else if (RunTimeDUA.StateType == stateType){
                return _getStateLogic(RunTimeDUA , skillNumber);
            }
            else if (RunTimeDUJ.StateType == stateType){
                return _getStateLogic(RunTimeDUJ , skillNumber);
            }
            else{
                return null;
            }

        }

        private StateLogicSO _getStateLogic(ComboSkill combo, SkillNumber skillNumber)
        {
            Debug.Log((int)skillNumber + 1);
            if (combo.StateLogicSOs.Length >= (int)skillNumber + 1)
            {
                return combo.StateLogicSOs[(int)skillNumber];
            }
            else
            {
                return null;
            }
        }



        // public StateLogicSO GetSkill(StateType stateType, SkillType skillType){

        //     foreach(ComboSkill skill in  ListAllComboSkills){
        //         if (skill.stateType == stateType ){
        //             if (skillType == SkillType.Skill1){
        //                 return skill.StateLogicSO[0];
        //             }else if(skillType == SkillType.Skill2){
        //                 return skill.StateLogicSO[1];
        //             }else if(skillType == SkillType.Skill3){
        //                 return skill.StateLogicSO[2];
        //             }else{
        //                 return skill.StateLogicSO[3];
        //             }
        //         }else{
        //             return null;
        //         }
        //     }
        //     return null;
        // }







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
   