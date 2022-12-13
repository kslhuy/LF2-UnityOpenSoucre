using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage_Data", menuName = "StageMode/Data")]
public class StageModeData : DescriptionBaseSO
{
    
    [Header("Spawn Cap (i.e. number of simultaneously spawned entities)")]
    [Tooltip("The minimum number of entities this spawner will try to maintain (regardless of player count)")]
    public int m_MinSpawnCap = 2;

    [Tooltip("The maximum number of entities this spawner will try to maintain (regardless of player count)")]
    public int m_MaxSpawnCap = 10;

    [Tooltip("For each player in the game, the Spawn Cap is raised above the minimum by this amount. (Rounds up to nearest whole number.)")]
    public float m_SpawnCapIncreasePerPlayer = 1;

    public List<Phase> Phases;



}
