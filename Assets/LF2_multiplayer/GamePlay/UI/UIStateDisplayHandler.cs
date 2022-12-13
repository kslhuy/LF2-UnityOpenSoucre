using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

namespace LF2.Client
{
    /// <summary>
    /// Class designed to only run on a client. Add this to a world-space prefab to display health or name on UI.
    /// </summary>
    /// <remarks>
    /// Execution order is explicitly set such that it this class executes its LateUpdate after any Cinemachine
    /// LateUpdate calls, which may alter the final position of the game camera.
    /// </remarks>
    [DefaultExecutionOrder(300)]
    public class UIStateDisplayHandler : NetworkBehaviour
    {
        [SerializeField]
        bool m_DisplayHealth;

        [SerializeField]
        bool m_DisplayName;

        [SerializeField]
        UIStateDisplay m_UIStatePrefab;

        // spawned in world (only one instance of this)
        UIStateDisplay m_UIState;

        RectTransform m_UIStateRectTransform;

        bool m_UIStateActive;

        [SerializeField]
        NetworkNameState m_NetworkNameState;

        [SerializeField]
        NetworkStaticsPoints m_NetworkHealthState;

        // [SerializeField]
        // ClientCharacter m_ClientCharacter;

        ClientAvatarGuidHandler m_ClientAvatarGuidHandler;

        CharacterClassContainer m_CharaterClasse;

        [SerializeField]
        IntVariable m_BaseHP;

        [Tooltip("UI object(s) will appear positioned at this transforms position.")]
        [SerializeField]
        Transform m_TransformToTrack;

        Camera m_Camera;

        Transform m_CanvasTransform;

        // as soon as any HP goes to 0, we wait this long before removing health bar UI object
        const float k_DurationSeconds = 2f;

        [Tooltip("World space vertical offset for positioning.")]
        [SerializeField]
        float m_YOffset = 70;

        [Tooltip("Screen space vertical offset for positioning.")]
        [SerializeField]
        float m_XOffset;

        // [SerializeField] private Vector3 m_ScreenOffset;

        // used to compute world position based on target and offsets
        Vector3 m_WorldPos;

        public override void OnNetworkSpawn()
        {
            if (!NetworkManager.Singleton.IsClient)
            {
                enabled = false;
                return;
            }

            var cameraGameObject = GameObject.FindWithTag("MainCamera");
            if (cameraGameObject)
            {
                m_Camera = cameraGameObject.GetComponent<Camera>();
            }
            Assert.IsNotNull(m_Camera);

            var canvasGameObject = GameObject.FindWithTag("GameCanvas");
            if (canvasGameObject)
            {
                m_CanvasTransform = canvasGameObject.transform;
            }
            Assert.IsNotNull(m_CanvasTransform);

            Assert.IsTrue(m_DisplayHealth || m_DisplayName, "Neither display fields are toggled on!");
            if (m_DisplayHealth)
            {
                Assert.IsNotNull(m_NetworkHealthState, "A NetworkHealthState component needs to be attached!");
            }

            // m_Offset = new Vector3(0f, m_VerticalScreenOffset, 0f);

            // if PC, find our graphics transform and update health through callbacks, if displayed
            if (TryGetComponent(out m_ClientAvatarGuidHandler) && TryGetComponent(out m_CharaterClasse))
            {
                if (m_BaseHP == null) {
                    // Debug.Log( m_CharaterClasse.CharacterClass);
                    m_BaseHP =  m_CharaterClasse.CharacterClass.BaseHP;

                }
                if (m_DisplayHealth)
                {
                    m_NetworkHealthState.HitPointsReplenished += DisplayUIHealth;
                    m_NetworkHealthState.HitPointsDepleted += RemoveUIHealth;
                }
            }

            if (m_DisplayName)
            {
                DisplayUIName();
            }

            if (m_DisplayHealth)
            {
                DisplayUIHealth();
            }
        }

        void OnDisable()
        {
            if (!m_DisplayHealth)
            {
                return;
            }

            if (m_NetworkHealthState != null)
            {
                m_NetworkHealthState.HitPointsReplenished -= DisplayUIHealth;
                m_NetworkHealthState.HitPointsDepleted -= RemoveUIHealth;
            }

            if (m_ClientAvatarGuidHandler)
            {
                m_ClientAvatarGuidHandler.AvatarGraphicsSpawned -= TrackGraphicsTransform;
            }
        }

        void DisplayUIName()
        {
            if (m_NetworkNameState == null)
            {
                return;
            }

            if (m_UIState == null)
            {
                SpawnUIState();
            }

            m_UIState.DisplayName(m_NetworkNameState.Name);
            m_UIStateActive = true;
        }

        void DisplayUIHealth()
        {
            if (m_NetworkHealthState == null)
            {
                return;
            }

            if (m_UIState == null)
            {
                SpawnUIState();
            }

            m_UIState.DisplayHealth(m_NetworkHealthState.HPPoints, m_BaseHP.Value);
            m_UIStateActive = true;
        }

        void SpawnUIState()
        {
            m_UIState = Instantiate(m_UIStatePrefab , m_CanvasTransform);
            // make in world UI state draw under other UI elements
            m_UIState.transform.SetAsFirstSibling();
            m_UIStateRectTransform = m_UIState.GetComponent<RectTransform>();
        }

        void RemoveUIHealth()
        {
            StartCoroutine(WaitToHideHealthBar());
        }

        IEnumerator WaitToHideHealthBar()
        {
            yield return new WaitForSeconds(k_DurationSeconds);

            m_UIState.HideHealth();
        }

        void TrackGraphicsTransform(GameObject graphicsGameObject)
        {
            m_TransformToTrack = graphicsGameObject.transform;
        }

        /// <remarks>
        /// Moving UI objects on LateUpdate ensures that the game camera is at its final position pre-render.
        /// </remarks>
        void LateUpdate()
        {
            if (m_UIStateActive && m_TransformToTrack)
            {
                // set world position with world offset added
                m_WorldPos.Set(m_TransformToTrack.position.x + m_XOffset,
                    m_YOffset ,
                    m_TransformToTrack.position.z);

                m_UIStateRectTransform.position = m_WorldPos;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (m_UIState != null)
            {
                Destroy(m_UIState.gameObject);
            }
        }
    }
}
