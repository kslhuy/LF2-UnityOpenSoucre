using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
namespace LF2
{
    /// <summary>
    /// This ScriptableObject defines a Player Character for BossRoom. It defines its CharacterClass field for
    /// associated game-specific properties, as well as its graphics representation.
    /// </summary>
    [CreateAssetMenu(menuName = "Collection/BackGround")]
    [Serializable]
    public class BackGroundGame : GuidScriptableObject
    {
        public string NameBackGround;

        public AssetReference BackGroundPreFab;

        public Sprite BackGroundImage;
        public AudioCueSO musicTrack;

    }
}
