using UnityEngine;

namespace LF2
{
    /// <summary>
    /// Abstract base class containing some common members shared by Action (server) and ActionFX (client visual)
    /// </summary>
    public abstract class stateMachineBase
    {
        protected CharacterTypeEnum CharacterType;
        public CharacterSkillsDescription m_CharacterSkillsDescription
        {

            get
            {
                CharacterSkillsDescription result;
                var found = GameDataSource.Instance.CharacterSkillDataByType.TryGetValue(CharacterType, out result);
                // Debug.Log(result);
                Debug.AssertFormat(found, "Tried to find CharacterSkillsDescription but it was missing from GameDataSource!");
                return result;
            }
        }


        public virtual SkillsDescription SkillDescription(StateType stateType){
            SkillsDescription value ;
            var found = m_CharacterSkillsDescription.SkillDataByState.TryGetValue(stateType , out value);
            Debug.AssertFormat(found, "Tried to find State {0} but it was missing from GameDataSource!", stateType);
            return value;
        }
        
        public stateMachineBase(CharacterTypeEnum characterType)
        {
            CharacterType = characterType;
        }
    }

}
