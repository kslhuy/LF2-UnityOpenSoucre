using LF2;
using LF2.Data;
using LF2.Gameplay.UI;
using LF2.Test;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIShopSkillInfo : MonoBehaviour {
    public StateType stateType;

    public string nameSkill;

    [SerializeField]    
    private TextMeshProUGUI m_NameSkills;
    [SerializeField]
    private TextMeshProUGUI m_DamageAmount;

    [SerializeField]
    private TextMeshProUGUI m_ManaCost;

    
    [SerializeField]
    private GameObject background;

    private ComboSkill m_ComboSkill;
    // [SerializeField]
    // private TextMeshProUGUI m_Descption;

    private UIShopCharSelectInfoBox m_UIInfoBoxManager;
    public void SetSkillInfo(UIShopCharSelectInfoBox uiInfoBoxManager, ComboSkill comboSkill){
        m_UIInfoBoxManager = uiInfoBoxManager;
        m_ComboSkill = comboSkill;
        m_NameSkills.text =  comboSkill.nameSkills;
        m_DamageAmount.text = comboSkill.StateLogicSO[0].DamageDetails[0].damageAmount.ToString();
        m_ManaCost.text = comboSkill.StateLogicSO[0].ManaCost.ToString();
        background.SetActive(comboSkill.Own);
        // m_Descption.text = descrption;
        stateType = comboSkill.StateLogicSO[0].StateType;

        nameSkill = comboSkill.nameSkills;
    }

    public void OnPurchaseButtonClicked(){
        m_UIInfoBoxManager.OnPurchaseClicked(m_ComboSkill);
    }
    

    public void OnSetButtonClicked(){
        UIHeroInventory.Instance.showSlotButton();
        UIHeroInventory.Instance.selectedSkill = this;

    }
}