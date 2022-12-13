using System.Collections.Generic;
using UnityEngine;

namespace LF2
{
    public class GameDataSource : MonoBehaviour
    {
        [Tooltip("All CharacterClass data should be slotted in here")]
        [SerializeField]
        // [ExtendEditorSOs]
        private CharacterClass[] m_CharacterData;

        [SerializeField] 
        // [ExtendEditorSOs]

        private CharacterSkillsDescription[] m_CharacterSkillsDescription; //All CharacterClass Skills 


        private Dictionary<CharacterTypeEnum, CharacterClass> m_CharacterDataMap;
        // Huy
        private Dictionary<CharacterTypeEnum, CharacterSkillsDescription> m_CharacterSkillDataMap;




        /// <summary>
        /// static accessor for all GameData.
        /// </summary>
        public static GameDataSource Instance { get; private set; }
        /// <summary>
        /// Contents of the CharacterData list, indexed by CharacterType for convenience.
        /// </summary>
        public Dictionary<CharacterTypeEnum, CharacterClass> CharacterDataByType
        {
            get
            {
                if( m_CharacterDataMap == null )
                {
                    m_CharacterDataMap = new Dictionary<CharacterTypeEnum, CharacterClass>();
                    foreach (CharacterClass data in m_CharacterData)
                    {
                        if( m_CharacterDataMap.ContainsKey(data.CharacterType))
                        {
                            throw new System.Exception($"Duplicate character definition detected: {data.CharacterType}");
                        }
                        m_CharacterDataMap[data.CharacterType] = data;
                    }
                }
                return m_CharacterDataMap;
            }
        }

        public  Dictionary<CharacterTypeEnum, CharacterSkillsDescription> CharacterSkillDataByType{
            get
            {
                if( m_CharacterSkillDataMap == null )
                {
                    m_CharacterSkillDataMap = new Dictionary<CharacterTypeEnum, CharacterSkillsDescription>();
                    foreach (CharacterSkillsDescription data in m_CharacterSkillsDescription)
                    {
                        if( m_CharacterSkillDataMap.ContainsKey(data.CharacterType))
                        {
                            throw new System.Exception($"Duplicate character definition detected: {data.CharacterType}");
                        }
                        m_CharacterSkillDataMap[data.CharacterType] = data;
                    }
                }
                return m_CharacterSkillDataMap;
            }
    }

        public int BackGround;


        private void Awake()
        {
            if (Instance != null)
            {
                throw new System.Exception("Multiple GameDataSources defined!");
            }

            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
    }
}
