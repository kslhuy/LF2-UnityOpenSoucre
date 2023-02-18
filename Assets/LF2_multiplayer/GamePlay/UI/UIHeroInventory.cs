using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LF2.Utils;
using Unity.Multiplayer.Infrastructure;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace LF2.Gameplay.UI
{
    public class UIHeroInventory : MonoBehaviour
    {

        [SerializeField]
        CanvasGroup m_CanvasGroup;
        public void Show()
        {
            gameObject.SetActive(true);
            m_CanvasGroup.alpha = 1f;
            m_CanvasGroup.blocksRaycasts = true;
        }
        public void Hide()
        {
            gameObject.SetActive(false);
            m_CanvasGroup.alpha = 0f;
            m_CanvasGroup.blocksRaycasts = false;
        }
    }
}
