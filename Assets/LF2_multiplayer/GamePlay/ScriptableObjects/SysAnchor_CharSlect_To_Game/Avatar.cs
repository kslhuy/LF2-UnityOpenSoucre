using System;
using UnityEngine;

namespace LF2
{
    /// <summary>
    /// This ScriptableObject defines a Player Character for BossRoom. It defines its CharacterClass field for
    /// associated game-specific properties, as well as its graphics representation.
    /// </summary>
    [CreateAssetMenu(menuName = "Collection/Avatar", order = 1)]
    [Serializable]
    public class Avatar : GuidScriptableObject
    {
        public CharacterClass CharacterClass;
        public CharacterClass CharacterClassNPC;

        public GameObject Graphics;

        public GameObject GraphicsCharacterSelect;

        public Sprite Portrait;

        // private void OnValidate() {
        //     if (CharacterClassNPC){
        //         if (CharacterClassNPC.CharacterType != CharacterClass.CharacterType){
        //             Debug.Log("Two CharacterClass need to be the same Champion");
        //         }
        //     }
        // }
    }
}
