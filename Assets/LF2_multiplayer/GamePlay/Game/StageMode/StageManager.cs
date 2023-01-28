using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using LF2.Client;
using LF2.Server;
using LF2;
using UnityEngine.Assertions;
using LF2.Utils;

public class StageManager : NetworkBehaviour
{
    public StageModeData stageModeData;
    public Transform Tf_BordRight;



    // [SerializeField]
    // List<Transform> m_SpawnPositions_Right;

    // [SerializeField] List<Transform> m_SpawnPositions_Test;
    // [Tooltip("Each spawned enemy appears at one of the points in this list")]

    [SerializeField] SpwanPostion[] m_SpawnPositions_Left;
    [SerializeField] SpwanPostion[] m_SpawnPositions_Right;

    [SerializeField]
    [Tooltip("Enemy to spawn. Make sure this is included in the NetworkManager's list of prefabs!")]
    NetworkObject m_AIPrefab;
    // [SerializeField] List<BotObject> m_NetworkedPrefabs;
    [SerializeField] AvatarRegistry avatarRegistry;



    public Action StageFinishEvent;



    // keep reference to our wave spawning coroutine
    Coroutine m_WaveSpawning;

    // track wave index and reset once all waves are complete
    int m_WaveIndex;

    // a running tally of spawned entities, used in determining which spawn-point to use next
    int m_SpawnedCount;

    // the currently-spawned entities. We only bother to track these if m_MaxActiveSpawns is non-zero
    List<NetworkObject> m_ActiveSpawnsPhase = new List<NetworkObject>();

    [Tooltip("Time between player distance & visibility scans, in seconds.")]
    public float m_PlayerProximityValidationTimestep = 2;

    int m_PhaseIndex;
    private Coroutine m_PhaseSpwaning;

    [SerializeField] LifeStateEventChannelSO lifeStateEventChannelSO;
    [SerializeField] BoolEventChannelSO m_DebugSpwanWaves;

    // [SerializeField] LifeStateEventChannelSO lifeStateEventChannelSO;


    public bool Debug_SpawnWave_Active;

    [Serializable]
    public class BotObject
    {
        // networked object that will be spawned in waves
        public CharacterTypeEnum BotType;
        public NetworkObject m_NetworkedPrefab;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }
        lifeStateEventChannelSO.LifeStateEvent_AI += LifeStateChange;
        if (m_DebugSpwanWaves == null)
        {
            Debug.LogWarning("you miss debug Spawn Waves");
        }
        m_DebugSpwanWaves.ActionBool += Debug_SpawnWave;
        StartCoroutine(CallSpwanPhase_Coro(5f));
    }

    private void Debug_SpawnWave(bool active)
    {
        Debug_SpawnWave_Active = !Debug_SpawnWave_Active;
    }

    // // Not use yet
    // Trigger when a character died ( no matter PC or NPC )
    private void LifeStateChange(LifeState lifeState )
    {
        if (lifeState == LifeState.Dead) StartCoroutine(CheckActiveSpawnWave(2f));
    }

    // wait for delay timer , to ensure bot destroy correctly after check 

    private IEnumerator CheckActiveSpawnWave(float delay)
    {
        yield return new WaitForSeconds(delay);
        // Check , in the game ,  how many bot left  
        m_ActiveSpawnsPhase.RemoveAll(spawnedNetworkObject => { return spawnedNetworkObject == null; });
        if ((m_ActiveSpawnsPhase.Count == 0))
        {
            if (m_PhaseIndex < stageModeData.Phases.Count){
                m_PhaseIndex++;
                SpawnPhases();
            }else{
                StageFinishEvent?.Invoke();
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        if (lifeStateEventChannelSO != null) lifeStateEventChannelSO.LifeStateEvent_AI -= LifeStateChange;
        StopWaveSpawning();
    }


    IEnumerator CallSpwanPhase_Coro(float delayBetweenPhases)
    {
        while (m_PhaseIndex < stageModeData.Phases.Count)
        {
            Debug.Log($"New Phase  {m_PhaseIndex}");
            if (Debug_SpawnWave_Active)
            {
                SpawnPhases();
                
            }
            yield return new WaitForSeconds(delayBetweenPhases);
            if (Debug_SpawnWave_Active) 
                m_ActiveSpawnsPhase.RemoveAll(spawnedNetworkObject => { return spawnedNetworkObject == null; });
        }
    }

    private void SpawnPhases()
    {
        if ((m_ActiveSpawnsPhase.Count == 0))
        {
            // Set position for rigth Bord , LEft 
            // Also spwan position
            for (int i = 0; i < m_SpawnPositions_Right.Length; i++)
            {
                m_SpawnPositions_Right[i].Position += stageModeData.Phases[m_PhaseIndex].BoundLimit;
            }
            for (int i = 0; i < m_SpawnPositions_Right.Length; i++)
            {
                m_SpawnPositions_Left[i].Position += 0.5f * stageModeData.Phases[m_PhaseIndex].BoundLimit;
            }

            Tf_BordRight.position += stageModeData.Phases[m_PhaseIndex].BoundLimit;

            if (m_WaveSpawning == null)
            {
                m_WaveSpawning = StartCoroutine(SpawnWaves(m_PhaseIndex));
            }
            m_PhaseIndex++;
        }
    }


    void StopWaveSpawning()
    {
        if (m_WaveSpawning != null)
        {
            StopCoroutine(m_WaveSpawning);
        }
        m_WaveSpawning = null;
        if (m_PhaseSpwaning != null)
        {
            StopCoroutine(m_PhaseSpwaning);
        }
        m_PhaseSpwaning = null;
    }


    /// <summary>
    /// Coroutine for spawning prefabs clones in waves, waiting a duration before spawning a new wave.
    /// Once all waves are completed, it waits a restart time before termination.
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnWaves(int phaseIndex)
    {

        int m_WaveIndex = 0;

        while (m_WaveIndex < stageModeData.Phases[phaseIndex].spwanWaves.Count)
        {
            yield return SpawnWave(phaseIndex, m_WaveIndex);

            yield return new WaitForSeconds(stageModeData.Phases[phaseIndex].m_TimeBetweenWaves);
            m_WaveIndex++;
        }

        yield return new WaitForSeconds(stageModeData.Phases[phaseIndex].m_RestartDelay);

        m_WaveSpawning = null;
    }



    /// <summary>
    /// Coroutine that spawns a wave of prefab clones, with some time between spawns.
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnWave(int phaseIndex, int waveIndex)
    {
        for (int i = 0; i < stageModeData.Phases[phaseIndex].spwanWaves[m_WaveIndex].waveInfo.NumberSpawn; i++)
        {
            int posIdx = 0;
            if (stageModeData.Phases[phaseIndex].spwanWaves[m_WaveIndex].m_SpawnLeft)
            {
                posIdx = m_SpawnedCount++ % m_SpawnPositions_Left.Length;
            }
            else
            {
                posIdx = m_SpawnedCount++ % m_SpawnPositions_Right.Length;
            }

            var newSpawn = SpawnPrefab(stageModeData.Phases[phaseIndex].spwanWaves[m_WaveIndex].waveInfo.Bot, m_SpawnPositions_Right[posIdx].Position, m_SpawnPositions_Right[posIdx].Rotation);

            m_ActiveSpawnsPhase.Add(newSpawn);

            yield return new WaitForSeconds(stageModeData.Phases[phaseIndex].m_TimeBetweenSpawns);
        }

        // m_WaveIndex++;
    }




    /// <summary>
    /// Spawn a NetworkObject prefab clone.
    /// </summary>
    NetworkObject SpawnPrefab(CharacterTypeEnum characterType, Vector3 position, Quaternion rotation)
    {
        // var botNetworkObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();

        var persistentPlayerExists = m_AIPrefab.TryGetComponent(out PersistentPlayer persistentPlayer);
        Assert.IsTrue(persistentPlayerExists,
            $"Matching persistent PersistentPlayer for Bot not found!");


        // Find a spawn point 
        // Instaite a Bot Object
        var newBOT = Instantiate(m_AIPrefab);
        var newBOTCharacter = newBOT.GetComponent<ServerCharacter>();

        var physicsTransform = newBOTCharacter.physicsWrapper.Transform;

        //     // Set position and rotation to the Bot Object in the scene
        physicsTransform.SetPositionAndRotation(position, rotation);

        // Check if the Bot Object have Componenet NetworkAvatarGuidState  
        var networkAvatarGuidStateExists =
            newBOT.TryGetComponent(out NetworkAvatarGuidState networkAvatarGuidState);

        Assert.IsTrue(networkAvatarGuidStateExists,
            $"NetworkCharacterGuidState not found on player avatar!");

        LF2.Avatar _botavatarValue;
        avatarRegistry.TryGetAvatar(characterType, out _botavatarValue);
        networkAvatarGuidState.RegisterAvatar(_botavatarValue);

        // pass name , team type from persistent player to avatar
        if (newBOT.TryGetComponent(out NetworkNameState networkNameState))
        {
            networkNameState.Name.Value = _botavatarValue.CharacterClass.CharacterType.ToString();
            networkNameState.Team.Value = TeamType.COM;
        }

    
        // newBOT.SpawnWithOwnership(NetworkManager.Singleton.LocalClientId, true);
        newBOT.Spawn( true);

        return newBOT;
    }





    // // / <summary>
    // // / Returns the current max number of entities we should try to maintain.
    // // / This can change based on the current number of living players; if the cap goes below
    // // / our current number of active spawns, we don't spawn anything new until we're below the cap.
    // // / </summary>
    // int GetCurrentSpawnCap()
    // {
    //     int numPlayers = 0;
    //     foreach (var clientCharacter in NbPlayer.GetCharacter())
    //     {
    //         if (clientCharacter.m_NetState.LifeState == LifeState.Alive)
    //         {
    //             ++numPlayers;
    //         }
    //     }

    //     return Mathf.CeilToInt(Mathf.Min(stageModeData.m_MinSpawnCap + (numPlayers * stageModeData.m_SpawnCapIncreasePerPlayer), stageModeData.m_MaxSpawnCap));
    // }

}
