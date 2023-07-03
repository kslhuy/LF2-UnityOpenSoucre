using UnityEngine;
using UnityEngine.UI;

namespace LF2.Gameplay.UI
{
    [RequireComponent(typeof(Image))]
    public class UITinter : MonoBehaviour
    {
        [SerializeField]
        Color[] m_TintColors;
        [SerializeField]
        Image m_Image;
        void Awake()
        {
            m_Image = GetComponent<Image>();
        }

        public void SetToColor(int colorIndex)
        {
            if (colorIndex >= m_TintColors.Length)
                return;
            // m_Image.color = m_TintColors[colorIndex];
        }
    }
}
