using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Phase {
    public Vector3 BoundLimit ;
    [Header("Wave parameters")]
    public List<SpwanWaves> spwanWaves;

    [Tooltip("Time between waves, in seconds.")]
    public float m_TimeBetweenWaves = 0.5f;

    [Tooltip("Time between waves, in seconds.")]
    public float m_TimeBetweenPhases = 5;

    [Tooltip("Time between waves, in seconds.")]
    public float m_TimeBetweenSpawns = 0.2f;

    [Tooltip("Once last wave is spawned, the spawner waits this long to restart wave spawns, in seconds.")]
    public float m_RestartDelay = 10;

    [Tooltip("A player must be within this distance to commence first wave spawn.")]
    public  float m_ProximityDistance = 30;
    

    // [Tooltip("When looking for players within proximity distance, should we count players in stealth mode?")]
    // public bool m_DetectStealthyPlayers = true;

}
