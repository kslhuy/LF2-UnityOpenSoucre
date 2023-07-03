using UnityEngine;
using UnityEngine.EventSystems;

public class SettingPositionUIGame : MonoBehaviour, IPointerDownHandler, IDragHandler ,IPointerUpHandler,ISelectHandler
{
	[SerializeField] RectTransform rectTransform ; 
	[SerializeField] CanvasGroup canvasGroup;
	
	[SerializeField] ButtonType buttonType;
	private Vector2 m_PointerDownPos;
	private Vector2 m_StartPos;



	PositionUIButton positionUI;

	private void Start()
	{
		m_StartPos = rectTransform.anchoredPosition;
		positionUI = GetComponentInParent<PositionUIButton>();

	}

    public void OnDrag(PointerEventData eventData)
    {
		RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponentInParent<RectTransform>(), eventData.position, eventData.pressEventCamera, out var position);
		var delta = position - m_PointerDownPos;

		rectTransform.anchoredPosition = m_StartPos +  delta;

	}

    public void OnPointerDown(PointerEventData eventData)
	{
		RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponentInParent<RectTransform>(), eventData.position, eventData.pressEventCamera, out m_PointerDownPos);

    }

    public void OnPointerUp(PointerEventData eventData)
    {
		// positionUI.ButtonSelected(ButtonType.None);

		m_StartPos = rectTransform.anchoredPosition;
        // throw new System.NotImplementedException();
    }

    public void OnSelect(BaseEventData eventData)
    {
		// Debug.Log("sleected ");
		positionUI.ButtonSelected(buttonType);

    }


}

