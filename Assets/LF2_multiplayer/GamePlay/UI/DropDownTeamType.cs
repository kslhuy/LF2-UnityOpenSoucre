using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

using LF2.Utils;

public class DropDownTeamType : MonoBehaviour
{

    [SerializeField] TMP_Dropdown dropdown;
    void Start()
    {
        
        string[] m_stateTypeString =  Enum.GetNames(typeof(TeamType));

        List<string> listStateType = new List<string>(m_stateTypeString);
        dropdown.AddOptions(listStateType);
    }


}
