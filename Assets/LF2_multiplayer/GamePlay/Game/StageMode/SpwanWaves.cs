using System;
using System.Collections.Generic;
using UnityEngine;
/* 
    One stage have n Phase , 1 Phase have m Wave 

    if wave 1 finsh after m_TimeBetweenSpawns , and so on 

    If Phase 1  fnish , we bascul to phase 2 

 */

[Serializable]
public class SpwanWaves {

    public WaveInfo waveInfo;
    
    // [Tooltip("Time between individual spawns, in seconds.")]
    // public float m_TimeBetweenSpawns = 0.5f;

    [Tooltip("Normal we spawn in the right , check this for add more spawn point on the left.")]
    public bool m_SpawnLeft ;
}
