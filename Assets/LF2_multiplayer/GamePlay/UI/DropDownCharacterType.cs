using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using LF2;
using System.Linq;

public class DropDownCharacterType : MonoBehaviour
{

    [SerializeField] TMP_Dropdown dropdown;
    void Start()
    {
        
        string[] m_stateTypeString =  Enum.GetNames(typeof(CharacterTypeEnum));

        List<string> listStateType = new List<string>(m_stateTypeString);
        dropdown.AddOptions(listStateType);
    }


}
