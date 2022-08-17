using UnityEngine;

public class LocalPos_Rot_UI : MonoBehaviour {
    [SerializeField] RectTransform rectTransform;

    private void Update() {
        rectTransform.rotation = Quaternion.Euler(0,0,0);
    }
}