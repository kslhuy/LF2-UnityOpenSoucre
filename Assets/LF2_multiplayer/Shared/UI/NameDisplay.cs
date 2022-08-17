using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace LF2.Client
{
    /// <summary>
    /// This is responsible for displaying and updating the player's chosen name.  Currently, the game does not allow just any
    /// input for a name, but instead creates a randomized 2 word combination for the stateMachine.  The player is then able to click randomize
    /// to receive a new name
    /// </summary>
    public class NameDisplay : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI m_CurrentName;


        public void Start()
        {
            ChooseNewName();
        }

        public string GetCurrentName()
        {
            return m_CurrentName.text;
        }

        /// <summary>
        /// Called to randomly select a new name for the player and displays it.
        /// </summary>
        public void ChooseNewName()
        {
            
        }
    }
}
