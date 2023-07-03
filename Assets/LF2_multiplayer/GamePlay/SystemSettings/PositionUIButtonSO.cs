using UnityEngine;
using System;

[CreateAssetMenu(fileName = "PositionButtonUI", menuName = "Settings/PositionBtnUIData")]
public class PositionUIButtonSO : ScriptableObject
{

	public PositionVetorUI AttackButton;
	public PositionVetorUI JumpButton;
	public PositionVetorUI DefenseButton;

	public PositionVetorUI UpButton;
	public PositionVetorUI DownButton;
	public PositionVetorUI JoyStickButton;

    public void SaveAll(PositionUI attackButton, PositionUI jumpButton, PositionUI defenseButton, PositionUI upButton, PositionUI downButton, PositionUI joyStickButton)
    {
        AttackButton.Position = attackButton.rectTransform.anchoredPosition;
		AttackButton.Scale = attackButton.rectTransform.localScale;

		AttackButton.alpha = attackButton.alpha.alpha;

		JumpButton.Position = jumpButton.rectTransform.anchoredPosition;
		JumpButton.Scale = jumpButton.rectTransform.localScale;
		JumpButton.alpha = jumpButton.alpha.alpha;

		DefenseButton.Position = defenseButton.rectTransform.anchoredPosition;
		DefenseButton.Scale = defenseButton.rectTransform.localScale;
		DefenseButton.alpha = defenseButton.alpha.alpha;
		
		UpButton.Position = upButton.rectTransform.anchoredPosition;
		UpButton.Scale = upButton.rectTransform.localScale;
		UpButton.alpha = upButton.alpha.alpha;
		
		DownButton.Position = downButton.rectTransform.anchoredPosition;
		DownButton.Scale = downButton.rectTransform.localScale;
		DownButton.alpha = downButton.alpha.alpha;
		
		JoyStickButton.Position = joyStickButton.rectTransform.anchoredPosition;
		JoyStickButton.Scale = joyStickButton.rectTransform.localScale;
		JoyStickButton.alpha = joyStickButton.alpha.alpha;
    }


}

[Serializable]
public class PositionUI {
	public RectTransform rectTransform;
	public CanvasGroup alpha;


}

[Serializable]
public class PositionVetorUI {
	public Vector2 Position;
	public Vector2 Scale;
	public float alpha;
}

