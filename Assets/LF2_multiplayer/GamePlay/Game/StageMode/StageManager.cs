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



    [SerializeField] SpwanPostion[] m_SpawnPositions_Left;
    [SerializeField] SpwanPostion[] m_SpawnPositions_Right;

    [SerializeField]
    [Tooltip("Enemy to spawn. Make sure this is included in the NetworkManager's list of prefabs!")]
    NetworkObject m_AIPrefab;
    [SerializeField] AvatarRegistry avatarRegistry;
    [SerializeField] PersistentPlayerRuntimeCollection m_PersistentPlayerCollection;



    private PersistentPlayer m_persistentPlayer;
    public Action StageFinishEvent;



    // keep reference to our wave spawning coroutine
    Coroutine m_WaveSpawning;

    

    // track wave index and reset once all waves are complete
    // int waveIndex;

    // a running tally of spawned entities, used in determining which spawn-point to use next


    // the currently-spawned entities. We only bother to track these if m_MaxActiveSpawns is non-zero
    List<NetworkObject> m_ActiveSpawnsPhase = new List<NetworkObject>();

    private float timerCheckGameOver;
    int m_PhaseIndex;
    private Coroutine m_PhaseSpwaning;

    int nbToT_Players;
    int nbToT_Characters; // use for Vesus mode only

// #if UNITY_EDITOR || DEVELOPMENT_BUILD
    [SerializeField] LifeStateEventChannelSO lifeStateEventChannelSO;
    [SerializeField] BoolEventChannelSO m_DebugStoryMode;


    public bool Debug_SpawnWave_Active;
    private bool m_FinishGame;

    // #endif 



    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }

        if (lifeStateEventChannelSO != null){
            lifeStateEventChannelSO.LifeStateEvent_AI += OnLifeStateChangedEventMessage_NPC;
            lifeStateEventChannelSO.LifeStateEvent_Player += OnLifeStateChangedEventMessage_Player;
        }

// #if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (m_DebugStoryMode == null)
        {
            Debug.LogWarning("you miss debug Spawn Waves");
        }else{
            // For Debug Only
            m_DebugStoryMode.ActionBool += Debug_StoryMode;
        }
// #endif

        var persistentPlayerExists = m_PersistentPlayerCollection.TryGetPlayer(NetworkManager.ServerClientId , out m_persistentPlayer);
        Assert.IsTrue(persistentPlayerExists,
            $"Stage Manger ,  PersistentPlayer  not found!");
        
        nbToT_Players = NbPlayer.GetPlayers().Count;
        if (m_persistentPlayer.gameMode == GameMode.VS ){
            nbToT_Characters =   PlayerServerCharacter.GetPlayerServerCharacters().Count;

        }else{
            m_PhaseSpwaning = StartCoroutine(CallSpwanPhase_Coro(5f));
        }

    }



    public override void OnNetworkDespawn()
    {
        if (lifeStateEventChannelSO != null){
            lifeStateEventChannelSO.LifeStateEvent_AI -= OnLifeStateChangedEventMessage_NPC;
            lifeStateEventChannelSO.LifeStateEvent_Player -= OnLifeStateChangedEventMessage_Player;
        }
        StopWaveSpawning();
    }


               
#region GameMode_StageLogic
    
    private void Debug_StoryMode(bool active)
    {
        m_persistentPlayer.gameMode = GameMode.Stage;
        if (m_PhaseSpwaning != null){
            Debug_SpawnWave_Active = !Debug_SpawnWave_Active;
            return;
        } 
        Debug_SpawnWave_Active = active;
        m_PhaseSpwaning = StartCoroutine(CallSpwanPhase_Coro(5f));

    }

    // // Not use yet
    // Trigger when a character died ( no matter PC or NPC )
    private void LifeStateChange(LifeState lifeState )
    {
        if (lifeState == LifeState.Dead) StartCoroutine(CheckGameOverStoryMode(2f));
    }

    // wait for delay timer , to ensure bot destroy correctly after check 
    // Not Work Yet 
    private IEnumerator CheckGameOverStoryMode(float delay)
    {
        yield return new WaitForSeconds(delay);
        // Check , in the game ,  how many bot left
        // But is not a good way to check 
        // Need to chance 
        m_ActiveSpawnsPhase.RemoveAll(spawnedNetworkObject => { return spawnedNetworkObject == null; });
        if ((m_ActiveSpawnsPhase.Count == 0 ) && m_PhaseIndex >= stageModeData.Phases.Count)
        {
                Debug.Log("EndGame CheckGameOverStoryMode function");
                SetWinStateStoryMode(WinState.Win);
                // EndGameEventPubliser.RaiseEvent();
        }
    }



    // Phase
    //  | 
    // Many Waves
    //  | 
    // 1 Wave
    //  |     
    //  Bot



    IEnumerator CallSpwanPhase_Coro(float delayBetweenPhases){
        while (m_PhaseIndex < stageModeData.Phases.Count){
            Debug.Log($"New Phase  {m_PhaseIndex}");
            if (Debug_SpawnWave_Active){
                // Check if current Phase is finish , 
                // If yes , spwan new Phase 
                if ((m_ActiveSpawnsPhase.Count == 0))
                {
                    // Update position for rigth Bord , LEft 
                    // Also spwan position
                    for (int i = 0; i < m_SpawnPositions_Right.Length; i++)
                    {
                        m_SpawnPositions_Right[i].Position += stageModeData.Phases[m_PhaseIndex].BoundLimit;
                    }
                    for (int i = 0; i < m_SpawnPositions_Left.Length; i++)
                    {
                        m_SpawnPositions_Left[i].Position += 0.5f * stageModeData.Phases[m_PhaseIndex].BoundLimit;
                    }

                    Tf_BordRight.position += stageModeData.Phases[m_PhaseIndex].BoundLimit;


                    // spwan Wave 
                    if (m_WaveSpawning == null)
                    {
                        m_WaveSpawning = StartCoroutine(Waves(m_PhaseIndex));
                        yield return m_WaveSpawning;
                    }

                    Debug.Log("Increment Phase number");
                    // Increment Phase number 
                    m_PhaseIndex++;
                }
                // SpawnPhases();  
            }
            yield return new WaitForSeconds(delayBetweenPhases);

        }
        

    }


    private void FixedUpdate() {
        if  (Time.time - timerCheckGameOver > 5f){
            if ( m_PhaseIndex >= stageModeData.Phases.Count  && !m_FinishGame && m_ActiveSpawnsPhase.Count == 0 ){
                m_FinishGame = true;
                SetWinStateStoryMode(WinState.Win);
            }

            // Clean the list that contain number of bot currenly active (alive) in a phase
            if (Debug_SpawnWave_Active && !m_FinishGame ) 
                m_ActiveSpawnsPhase.RemoveAll(spawnedNetworkObject => { return spawnedNetworkObject == null; });
        }
    }

    private void SetWinStateStoryMode(WinState winState){
        Debug.Log( "Set Win State Story Mode ");
        foreach (ClientCharacterVisualization player in NbPlayer.GetPlayers()){
            PersistentPlayer persistentPlayer;
            // Debug.Log("player Owner ID server" + player.OwnerClientId);
                
            m_PersistentPlayerCollection.TryGetPlayer(player.OwnerClientId , out persistentPlayer);
            persistentPlayer.SetWinState(winState);
        }

        Debug.Log("Raise EndGame Event ");
        m_PersistentPlayerCollection.TryGetPlayer(NetworkManager.ServerClientId  , out PersistentPlayer persitentPlayer0);
        persitentPlayer0.RaiseEndGameClientRPC();


    }






    // Dont know much : 
    // May be when the all player died , Wave Still spwan 
    // So call that to Stop , 
    // Also call when end Game  
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
    IEnumerator Waves(int phaseIndex)
    {
        for (int waveIndex = 0 ; waveIndex < stageModeData.Phases[phaseIndex].spwanWaves.Count ; waveIndex++){
            Debug.Log($"Waves {waveIndex} of phaseIndex : {phaseIndex}");

            yield return SpawnWave(phaseIndex, waveIndex);

            yield return new WaitForSeconds(stageModeData.Phases[phaseIndex].m_TimeBetweenWaves);
        }

        m_WaveSpawning = null;
    }



    /// <summary>
    /// Coroutine that spawns a wave of prefab clones, with some time between spawns.
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnWave(int phaseIndex, int waveIndex)
    {
        // iterat throgh Number Spawn 
        // Debug.Log($"Number Spwan of SpawnWave  {stageModeData.Phases[phaseIndex].spwanWaves[waveIndex].waveInfo.NumberSpawn} of waveIndex : {waveIndex}");
        for (int i = 0; i < stageModeData.Phases[phaseIndex].spwanWaves[waveIndex].waveInfo.NumberSpawn; i++)
        {
            int posIdx = 0;

            if (stageModeData.Phases[phaseIndex].spwanWaves[waveIndex].m_SpawnLeft)
            {
                posIdx = UnityEngine.Random.Range(0, m_SpawnPositions_Left.Length);
            }
            else
            {
                posIdx = UnityEngine.Random.Range(0, m_SpawnPositions_Right.Length);
            }
            // Debug.Log($"Phase : {phaseIndex} -> SpwanWave : {waveIndex} -> Bot type : {stageModeData.Phases[phaseIndex].spwanWaves[waveIndex].waveInfo.BotType}"  );

            var newSpawn = SpawnPrefab(stageModeData.Phases[phaseIndex].spwanWaves[waveIndex].waveInfo.BotType,
                                     stageModeData.Phases[phaseIndex].spwanWaves[waveIndex].m_SpawnLeft ? m_SpawnPositions_Left[posIdx].Position:m_SpawnPositions_Right[posIdx].Position  ,
                                     Quaternion.identity);

            m_ActiveSpawnsPhase.Add(newSpawn);
            
            yield return new WaitForSeconds(stageModeData.Phases[phaseIndex].m_TimeBetweenSpawns);

        }

    }

#endregion



    /// <summary>
    /// Spawn a NetworkObject prefab clone.
    /// </summary>
    NetworkObject SpawnPrefab(CharacterTypeEnum characterType, Vector3 position, Quaternion rotation)
    {
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

        // LF2.Avatar _botavatarValue;
        // avatarRegistry.TryGetAvatar(characterType, out _botavatarValue);
        // networkAvatarGuidState.RegisterAvatar(_botavatarValue);
        networkAvatarGuidState.AvatarType.Value = characterType;

        // pass name , team type from persistent player to avatar
        if (newBOT.TryGetComponent(out NetworkNameState networkNameState))
        {
            networkNameState.Name.Value = characterType.ToString();
            networkNameState.Team.Value = TeamType.COM;
        }
    
        newBOT.Spawn( true);

        return newBOT;
    }


    // Every time a player's life state changes we check to see if game is over
    void OnLifeStateChangedEventMessage_NPC(LifeState lifeState)
    {

        // Logic below for versus mode only
        if (m_persistentPlayer.gameMode == GameMode.VS ){
            CheckGameOverVsMode(lifeState);
        }else{
            if (lifeState == LifeState.Dead){
                StartCoroutine(CheckGameOverStoryMode(2f));
            } 
        }
                
    }

    // Make sur this event triger when one character died
    void OnLifeStateChangedEventMessage_Player(LifeState lifeState , ulong playerID)
    {
        nbToT_Players--;
        // Logic below for versus mode only
        if (m_persistentPlayer.gameMode == GameMode.VS ){
            CheckGameOverVsMode(lifeState);
        }else{
            //check atless one player still alive to continue the game 
            foreach (var clientCharacter in NbPlayer.GetPlayers()){
                if ( clientCharacter.m_NetState.LifeState == LifeState.Alive){
                    return;
                }
            }
            // if all player died , call engame 

            StopWaveSpawning();
            Debug.Log("EndGame OnLifeStateChangedEventMessage_Player function");
            SetWinStateStoryMode(WinState.Loss);

        }
        
        
    }

#region GameMode_Vesus
    
    void CheckGameOverVsMode(LifeState lifeState , ulong playerID = default){
        // Logic end game 

        // Player VS (Com or Player)
        // Logic : 
        //       : if all player died , End 
        // Or    : if one team left in the game , so the game end


        List<TeamType> listTeamNow = new List<TeamType>();
        int i = 0;

        // TODO FiXBug : 2 player With the same Team Type  (Independant + Independant) still End the game

        // Check the life state of all characters ( Player and Bot ) in the scene
        foreach (var serverCharacter in PlayerServerCharacter.GetPlayerServerCharacters())
        {
            // Because a BOT will destroyed when died after few seconds , ( Player Not ) 
            // so they  dont have Component NetStat  , that reasone why we check NetState first
            if (serverCharacter.NetState && serverCharacter.NetState.LifeState == LifeState.Alive)
            {
                i++;
                if (listTeamNow.Contains(serverCharacter.NetState.TryGetTeamType())) continue;
                listTeamNow.Add(serverCharacter.NetState.TryGetTeamType());
                if (listTeamNow.Count > 1) return;   // more than 1 mean 2 , 3 team left during the game 
                                                    // so that mean can not End the game 
            }
        }


        // 1: Find Team Left 
        TeamType teamType_left = listTeamNow[0];

        // 2: Set Win State For Each player
        // _ClientLF2State.EndGameServerRPC(teamType_left);
        foreach (ClientCharacterVisualization player in NbPlayer.GetPlayers()){
            PersistentPlayer persistentPlayer;
            Debug.Log("player Owner ID server" + player.OwnerClientId);
            if (player.m_NetState.TryGetTeamType() != teamType_left){
                
                m_PersistentPlayerCollection.TryGetPlayer(player.OwnerClientId , out persistentPlayer);
                persistentPlayer.SetWinState(WinState.Loss);
                // continue;
            }
            else if (teamType_left == TeamType.INDEPENDANT && (player.m_NetState.LifeState == LifeState.Dead)){
                m_PersistentPlayerCollection.TryGetPlayer(player.OwnerClientId , out persistentPlayer);
                persistentPlayer.SetWinState(WinState.Loss);

            }else{
                // 3: Find All Players In this Team  
                m_PersistentPlayerCollection.TryGetPlayer(player.OwnerClientId , out persistentPlayer);
                persistentPlayer.SetWinState(WinState.Win);

            }
        }



        // If we made it this far, all players are down! switch to post game
        
        // Coro_GameEnd = StartCoroutine(CoroGameOver(k_LoseDelay, true));
        // TODO : trigger event for ServerLF2State to End the game
        Debug.Log("EndGame Vesus Mode function");

        m_PersistentPlayerCollection.TryGetPlayer(NetworkManager.ServerClientId  , out PersistentPlayer persitentPlayer0);
        persitentPlayer0.RaiseEndGameClientRPC();


    }


#endregion




}
