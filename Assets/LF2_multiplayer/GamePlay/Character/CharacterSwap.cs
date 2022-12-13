using UnityEngine.Assertions;
using UnityEngine;
using System.Collections.Generic;
using LF2.Client;

namespace LF2.Client
{
    /// <summary>
    /// Responsible for storing of all of the pieces of a character, and swapping the pieces out en masse when asked to.
    /// </summary>
    public class CharacterSwap : MonoBehaviour
    {
        [System.Serializable]
        public class CharacterModelSet
        {
            public AnimatorTriggeredSpecialFX specialFx; // should be a component on the same GameObject as the Animator!
            public AnimatorOverrideController Normal_animatorOverrides; // references a separate stand-alone object in the project
            public Sprite _sprite; // references a separate stand-alone object in the project


            public Vector3 _CenterBoxCollider;
            public Vector3 _SizeBoxCollider;

        }

        [SerializeField]
        public CharacterModelSet m_CharacterModel;

        /// <summary>
        /// Reference to our shared-characters' animator.
        /// Can be null, but if so, animator overrides are not supported!
        /// </summary>
        [SerializeField]
        private Animator m_NormalAnimator;
        
        [SerializeField]
        private SpriteRenderer m_SpriteRenderer;

        /// <summary>
        /// Reference to the original controller in our Animator.
        /// We switch back to this whenever we don't have an Override.
        /// </summary>
        private RuntimeAnimatorController m_OriginalController;

        ClientCharacterVisualization m_ClientCharacterVisualization;

        private void Awake()
        {
            m_ClientCharacterVisualization = GetComponentInParent<ClientCharacterVisualization>();
            //  GetComponentInParent<Animator>();
            m_SpriteRenderer = GetComponentInParent<SpriteRenderer>();
            m_NormalAnimator = m_ClientCharacterVisualization.NormalAnimator;            
            // Debug.Log(m_InjuryAnimator);
            m_OriginalController = m_NormalAnimator.runtimeAnimatorController;
        }


        /// <summary>
        /// Swap the visuals of the character to the index passed in.
        /// </summary>
        /// <param name="modelIndex">Zero-based array index of the model</param>
        /// <param name="specialMaterialMode">Special Material to apply to all body parts</param>
        public void SwapToModel()
        {

            if (m_CharacterModel.specialFx)
            {
                m_CharacterModel.specialFx.enabled = true;
            }

            if (m_NormalAnimator)
            {
                // plug in the correct animator override... or plug the original non - overridden version back in!
                if (m_CharacterModel.Normal_animatorOverrides )
                {
                    m_SpriteRenderer.sprite = m_CharacterModel._sprite; 
                    m_NormalAnimator.runtimeAnimatorController = m_CharacterModel.Normal_animatorOverrides;
                }
                else
                {
                    m_SpriteRenderer.sprite = m_CharacterModel._sprite; 
                    m_NormalAnimator.runtimeAnimatorController = m_OriginalController;
                    // m_InjuryAnimator.runtimeAnimatorController = m_CharacterModel.Injury_animatorOverrides;

                }
            }

        }





#if UNITY_EDITOR
        private void OnValidate()
        {
            // if an Animator is on the same GameObject as us, assume that's the one we'll be using!
            if (!m_NormalAnimator)
                m_NormalAnimator = GetComponent<Animator>();
        }
#endif
    }
}
