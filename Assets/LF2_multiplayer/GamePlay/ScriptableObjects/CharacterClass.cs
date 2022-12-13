using UnityEngine;
using LF2.ObjectPool;


namespace LF2
{
    /// <summary>
    /// Data representation of a Character, containing such things as its starting HP and Mana, and what attacks it can do.
    /// </summary>
    [CreateAssetMenu(menuName = "GameData/CharacterClass", order = 1)]
    public class CharacterClass : ScriptableObject
    {
        // public Sprite ImageCharacter;
        [Tooltip("which character this data represents")]
        public CharacterTypeEnum CharacterType;


        [Header("----------Stats-----------")]

        [Tooltip("Starting HP of this character class")]
        public IntVariable BaseHP;

        [Tooltip("Starting Mana of this character class")]
        public IntVariable BaseMP;

        public int BaseAmor;

        
        public int BaseFall;
        public int BaseBdefense;

        [Tooltip("How many time character can jump ")]
        public int NbJump;

        [Header("--------Movement----------")]

        [Header("------Walk-----")]
        public float walking_speed;
        public float walking_speedz;
        [Header("------Run-----")]
        public float running_speed;   
        public float running_speed_z; 
        [Header("------Jump-----")]
        public float  jump_distance;

        public float  jump_height;

        [Header("------DoubleJump-----")]

        public float  doublejump_distance;

        public float  doublejump_height;

        [Header("------Dash-----")]
        public float  dash_height;

        public float  dash_distance;

        public float  dash_distance_z;

        [Header("------Rolling-----")]
        public float  rolling_speed;

        [Tooltip("Set to true if this represents an NPC, as opposed to a stateMachine.")]
        public bool IsNpc;

        [Tooltip("For NPCs, this will be used as the aggro radius at which enemies wake up and attack the player")]
        public float DetectRange;

        [Tooltip("For players, this is the displayed \"class name\". (Not used for monsters)")]
        public string DisplayedName;

        
        [Multiline]
        public string InfoChampion = "Bla bla";

        public StateActionAI_SO StateAI;

        public ObjectPollingRegistry objectPollingRegistry;


    }
}
