using System;
using Unity.Multiplayer.Samples.BossRoom.ApplicationLifecycle.Messages;
using Unity.Multiplayer.Infrastructure;
using UnityEngine;
using VContainer;
using LF2.ConnectionManagement;

namespace Gameplay.UI
{
    public class UIQuitPanel : MonoBehaviour
    {
        enum QuitMode
        {
            ReturnToMenu,
            QuitApplication
        }

        [SerializeField]
        QuitMode m_QuitMode = QuitMode.ReturnToMenu;

        [Inject] ConnectionManager m_ConnectionManager;
        [Inject] IPublisher<QuitApplicationMessage> m_QuitApplicationPub;

        public void Quit()
        {
            switch (m_QuitMode)
            {
                case QuitMode.ReturnToMenu:
                    m_ConnectionManager.RequestShutdown();
                    break;
                case QuitMode.QuitApplication:
                    m_QuitApplicationPub.Publish(new QuitApplicationMessage());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            gameObject.SetActive(false);
        }
    }
}
