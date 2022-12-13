using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ComboKey", menuName = "ComboKey")]
public class ComboKey : ScriptableObject
{
    [SerializeField] List<KeyPressedType> movesKeyCodes; //the List and order of the Moves
    [SerializeField] LF2.StateType stateType; //The kind of the move

    public bool isComboAvilable(List<KeyPressedType> playerKeyCodes) //Check if we can perform this move from the entered keys
    {
        for (int i = 0; i < playerKeyCodes.Count; i++)
        {
            if (playerKeyCodes[i] != movesKeyCodes[i])
            {
                return false;
            }
        }
        return true;
   
    }

    public LF2.StateType GetTypeOfSkill()
    {
        return stateType;
    }
}
