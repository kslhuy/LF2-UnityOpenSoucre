using LF2.Data;
using LF2.Gameplay.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIShopSkillInfo : MonoBehaviour {
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
        m_DamageAmount.text = comboSkill.StateLogicSOs[0].DamageDetails[0].damageAmount.ToString();
        m_ManaCost.text = comboSkill.StateLogicSOs[0].ManaCost.ToString();
        background.SetActive(comboSkill.Own);
        // m_Descption.text = descrption;
    }

    public void OnPurchaseButtonClicked(){
        m_UIInfoBoxManager.OnPurchaseClicked(m_ComboSkill);
    }
    
    public void Setbutton_Clicked(){
        
    }

}