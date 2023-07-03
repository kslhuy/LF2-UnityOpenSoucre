using UnityEngine;
using UnityEngine.UI;
using System;

public enum ButtonType {
	None,
	Attack , 
	Jump,
	Denfense,
	Up,
	Down,
	JoyStickButton,
}

public class PositionUIButton : MonoBehaviour {

	[SerializeField] GameObject Master; 

	[SerializeField] PositionUI AttackButton; 
	[SerializeField] PositionUI JumpButton;
	[SerializeField] PositionUI DefenseButton;
	[SerializeField] PositionUI UpButton;
	[SerializeField] PositionUI DownButton;
	[SerializeField] PositionUI JoyStickButton;

	[SerializeField] PositionUIButtonSO Current_DataPostionUI;
	[SerializeField] PositionUIButtonSO Defaut_DataPostionUI;

	[SerializeField] Slider alphaSlider;
	[SerializeField] Slider sizeSlider;

	[SerializeField] GameObject Quit_Panel;

	private ButtonType current_buttonSelected;


    // [SerializeField] Slider sizeSlider;

    public void Start(){
        alphaSlider.onValueChanged.AddListener(OnAlphaSliderValueChanged);
		sizeSlider.onValueChanged.AddListener(OnSizeSliderValueChanged);

    }

    private void OnSizeSliderValueChanged(float newValue)
    {

		switch (current_buttonSelected) {
			case ButtonType.Attack :
				AttackButton.rectTransform.localScale = new Vector3(sizeSlider.value , sizeSlider.value  , 0) ;
				break;
			case ButtonType.Jump :
				JumpButton.rectTransform.localScale = new Vector3(sizeSlider.value , sizeSlider.value  , 0) ;
				break;
			case ButtonType.Denfense :
				DefenseButton.rectTransform.localScale = new Vector3(sizeSlider.value , sizeSlider.value  , 0) ;
				break;
			case ButtonType.Up :
				UpButton.rectTransform.localScale = new Vector3(sizeSlider.value , sizeSlider.value  , 0) ;
				break;
			case ButtonType.Down :
				DownButton.rectTransform.localScale = new Vector3(sizeSlider.value , sizeSlider.value  , 0) ;
				break;
			case ButtonType.JoyStickButton :
				JoyStickButton.rectTransform.localScale = new Vector3(sizeSlider.value , sizeSlider.value  , 0) ;
				break;
			case ButtonType.None :
				break;
				
		}

    }

    private void OnAlphaSliderValueChanged(float value)
    {
		switch (current_buttonSelected) {
			case ButtonType.Attack :
				AttackButton.alpha.alpha  = alphaSlider.value;
				break;
			case ButtonType.Jump :
				JumpButton.alpha.alpha  = alphaSlider.value;
				break;
			case ButtonType.Denfense :
				DefenseButton.alpha.alpha  = alphaSlider.value;
				break;
			case ButtonType.Up :
				UpButton.alpha.alpha  = alphaSlider.value;
				break;
			case ButtonType.Down :
				DownButton.alpha.alpha  = alphaSlider.value;
				break;
			case ButtonType.JoyStickButton :
				JoyStickButton.alpha.alpha  = alphaSlider.value;
				break;
			case ButtonType.None :
				break;
		}
    }	


    public void SaveAll (){
		Current_DataPostionUI.SaveAll( AttackButton ,  JumpButton ,
										 DefenseButton , UpButton ,
										 DownButton , JoyStickButton);
	}
	public void SaveAll_Defaut (){
		Defaut_DataPostionUI.SaveAll( AttackButton ,  JumpButton ,
										 DefenseButton , UpButton ,
										 DownButton , JoyStickButton);
	}
	public void Quit(){
	}

	public void ResetToDefaut(){
		FetchDataToUI(Defaut_DataPostionUI);
	}

	public void FetchDataToUI(PositionUIButtonSO dataPosition){

		AttackButton.rectTransform.anchoredPosition = dataPosition.AttackButton.Position;
		AttackButton.rectTransform.localScale 		= dataPosition.AttackButton.Scale;
		AttackButton.alpha.alpha 					= dataPosition.AttackButton.alpha;

		JumpButton.rectTransform.anchoredPosition 	= dataPosition.JumpButton.Position;
		JumpButton.rectTransform.localScale 		= dataPosition.JumpButton.Scale;
		JumpButton.alpha.alpha 						= dataPosition.JumpButton.alpha;

		DefenseButton.rectTransform.anchoredPosition = dataPosition.DefenseButton.Position;
		DefenseButton.rectTransform.localScale 		= dataPosition.DefenseButton.Scale;
		DefenseButton.alpha.alpha 					= dataPosition.DefenseButton.alpha;
	
		UpButton.rectTransform.anchoredPosition 	= dataPosition.UpButton.Position;
		UpButton.rectTransform.localScale 			= dataPosition.UpButton.Scale;
		UpButton.alpha.alpha 						= dataPosition.UpButton.alpha;
	
		DownButton.rectTransform.anchoredPosition 	=	dataPosition.DownButton.Position;
		DownButton.rectTransform.localScale 		= dataPosition.DownButton.Scale;
		DownButton.alpha.alpha 						= dataPosition.DownButton.alpha;
	
		JoyStickButton.rectTransform.anchoredPosition = dataPosition.JoyStickButton.Position ;
		JoyStickButton.rectTransform.localScale 	= dataPosition.JoyStickButton.Scale;
		JoyStickButton.alpha.alpha 					= dataPosition.JoyStickButton.alpha;

	}


	public void ButtonSelected(ButtonType buttonType)
    {
		alphaSlider.interactable = true;
		sizeSlider.interactable = true;
		current_buttonSelected = buttonType;
        switch (buttonType) {
			case ButtonType.Attack :
				alphaSlider.value =  AttackButton.alpha.alpha;
				sizeSlider.value =  AttackButton.rectTransform.localScale.x;
				break;
			case ButtonType.Jump :
				alphaSlider.value =  JumpButton.alpha.alpha;
				sizeSlider.value =  JumpButton.rectTransform.localScale.x;
				break;
			case ButtonType.Denfense :
				alphaSlider.value =  DefenseButton.alpha.alpha;
				sizeSlider.value =  DefenseButton.rectTransform.localScale.x;
				break;
			case ButtonType.Up :
				alphaSlider.value =  UpButton.alpha.alpha;
				sizeSlider.value =  UpButton.rectTransform.localScale.x;
				break;
			case ButtonType.Down :
				alphaSlider.value =  DownButton.alpha.alpha;
				sizeSlider.value =  DownButton.rectTransform.localScale.x;
				break;
			case ButtonType.JoyStickButton :
				alphaSlider.value =  JoyStickButton.alpha.alpha;
				sizeSlider.value =  JoyStickButton.rectTransform.localScale.x;
				break;
			case ButtonType.None :
				alphaSlider.interactable = false;
				sizeSlider.interactable = false;
				current_buttonSelected = ButtonType.None; 
				break;
				
		}


    }

	public void DeSelectButton(){
		Debug.Log("Deselected");
		alphaSlider.interactable = false;
		sizeSlider.interactable = false;
		current_buttonSelected = ButtonType.None;
	}

	public void QuitUISetting(){
		Master.SetActive(false);
		// Quit_Panel.SetActive(true);
	}





}



