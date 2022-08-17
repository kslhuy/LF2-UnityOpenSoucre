using System.Collections.Generic;
using UnityEngine;

namespace LF2.Client
{
    public class GameDataSourceNew : MonoBehaviour
    {
        [Tooltip("All CharacterClass data should be slotted in here")]
        [SerializeField]
        // [ExtendEditorSOs]
        private CharacterClass[] m_CharacterData;

        [SerializeField] 
        // [ExtendEditorSOs]

        private CharacterStateSOs[] m_CharacterStateSOs; //All CharacterClass Skills 


        // Huy
        private Dictionary<CharacterTypeEnum, CharacterClass> m_CharacterDataMap;

        private Dictionary<CharacterTypeEnum, CharacterStateSOs> m_CharacterStateDataMap;




        /// <summary>
        /// static accessor for all GameData.
        /// </summary>
        public static GameDataSourceNew Instance { get; private set; }
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

        public  Dictionary<CharacterTypeEnum, CharacterStateSOs> AllStateCharacterByType{
            get
            {
                if( m_CharacterStateDataMap == null )
                {
                    m_CharacterStateDataMap = new Dictionary<CharacterTypeEnum, CharacterStateSOs>();
                    foreach (CharacterStateSOs data in m_CharacterStateSOs)
                    {
                        if( m_CharacterStateDataMap.ContainsKey(data.CharacterType))
                        {
                            throw new System.Exception($"Duplicate character definition detected: {data.CharacterType}");
                        }
                        m_CharacterStateDataMap[data.CharacterType] = data;
                    }
                }
                return m_CharacterStateDataMap;
            }
    }

        public int BackGround;
        public Transform pfDamagePopup;


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
