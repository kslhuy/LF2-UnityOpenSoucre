using UnityEngine;
namespace LF2.Client{
    
    public class UICharSeatsManager : MonoBehaviour {
        public UICharSelectPlayerSeat uICharSeat_pf;
        public RectTransform rectTF_Seats;

        [SerializeField] private AvatarRegistry avatarRegistry;
        private void Start() {
            for (int i = 0 ; i < avatarRegistry.m_Avatars.Length ; i++){
                if (avatarRegistry.m_Avatars[i].Own){
                    UICharSelectPlayerSeat uICharSelectPlayerSeat = Instantiate(uICharSeat_pf , rectTF_Seats);
                    uICharSelectPlayerSeat.SetCharacterType(avatarRegistry.m_Avatars[i].CharacterClass.CharacterType , avatarRegistry.m_Avatars[i].Portrait);
                }
            }
        }

    }

}