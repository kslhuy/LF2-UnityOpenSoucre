using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SpwanPostion {
    
    [SerializeField] public Transform m_Transform;
    [HideInInspector]
    public Vector3 Position {
        get  => m_Transform.position;
        set {
            m_Transform.position = value ;
        } 
    } 
    [HideInInspector]
    public Quaternion Rotation {
        get  => m_Transform.rotation;
        set {
            m_Transform.rotation = value ;
        } 
    } 
}
