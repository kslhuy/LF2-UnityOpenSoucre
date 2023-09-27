using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LF2;
using TMPro;
using UnityEngine.UI;
using LF2.Gameplay.UI;

namespace LF2.Test {
    public class SkillSlot : MonoBehaviour
    {
        public Avatar avatar;
        public StateType state;
        public TextMeshProUGUI text;
        public Image image;
        public UIShopCharSelectInfoBox ui;
        // Start is called before the first frame update
        void Start()
        {
            text.text = state.ToString();
            image.enabled = true;
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void select() {
            text.text = state.ToString() + '\n' + UIHeroInventory.Instance.selectedSkill.nameSkill ;
            avatar.CharacterStateSOs.skillConfig.set(state, UIHeroInventory.Instance.selectedSkill.stateType);
            UIHeroInventory.Instance.hideSlotButton();
        }
    }

    public enum SkillSlotEnum {
        DDA,
        DUA,
        DDJ,
        DUJ
    }
}
