using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LF2{
    /// <summary>
    /// Data representation ALL skills of a Character 
    /// </summary>
    [CreateAssetMenu(menuName = "GameData/CharacterSkillsDescription", order = 2)]
    [System.Serializable]
    public class CharacterSkillsDescription : ScriptableObject
    {
        public CharacterTypeEnum CharacterType;

        public List<SkillsDescription> SkillsDescription; //The kind of the move

                
        private Dictionary<StateType, SkillsDescription> m_SkillDataMap;

        public  Dictionary<StateType, SkillsDescription> SkillDataByState{
            get
                {
                    if( m_SkillDataMap == null )
                    {
                        m_SkillDataMap = new Dictionary<StateType, SkillsDescription>();
                        // Hoi bi rac roi cach viet
                        // co 1 list SkillsDescription o tren , lay tung cai 1 .
                        foreach (SkillsDescription data in SkillsDescription)
                        {
                            if (m_SkillDataMap.ContainsKey(data.StateType))
                            {
                                throw new System.Exception($"Duplicate action definition detected: {data.StateType}");
                            }
                            m_SkillDataMap[data.StateType] = data;
                        }
                    }
                    return m_SkillDataMap;
                }
        }
    }
}

