// using System;
// using System.Collections.Generic;
// using Unity.Netcode;
// using UnityEngine;

// namespace LF2
// {
//     /// <summary>
//     /// A component for syncing transforms
//     /// NetworkTransform will read the underlying transform and replicate it to clients.
//     /// The replicated value will be automatically be interpolated (if active) and applied to the underlying GameObject's transform
//     /// </summary>
//     [DefaultExecutionOrder(100000)] // this is needed to catch the update time after the transform was updated by user scripts
//     public class NetTransformcopy1 : NetworkBehaviour
//     {
//         public const float PositionThresholdDefault = 0.5f;
//         public const float RotAngleThresholdDefault = 1f;
//         public delegate (Vector3 pos, byte rotYOut) OnClientRequestChangeDelegate(Vector3 pos, byte rotY);
//         public OnClientRequestChangeDelegate OnClientRequestChange;

//         //  
//         private byte lastRotaion ; 

//         [SerializeField] private BoxCollider m_BoxCollider;
//         RaycastHit[] GroundHit  = new RaycastHit[1];
//         private int k_GroundLayerMask;

//         [SerializeField]
//         Rigidbody m_Rigidbody;  

//         [SerializeField] private int distanceY;
//         //
//         internal struct NetTransformState : INetworkSerializable
//         {
//             private const int k_PositionXBit = 0;
//             private const int k_PositionYBit = 1;
//             private const int k_PositionZBit = 2;
//             private const int k_RotAngleYBit = 3;
//             private const int k_TeleportingBit = 4;

//             // 11-15: <unused>
//             private ushort m_Bitset;


//             // Position
//             internal bool HasPositionX
//             {
//                 get => (m_Bitset & (1 << k_PositionXBit)) != 0;
//                 set
//                 {
//                     if (value) { m_Bitset = (ushort)(m_Bitset | (1 << k_PositionXBit)); }
//                     else { m_Bitset = (ushort)(m_Bitset & ~(1 << k_PositionXBit)); }
//                 }
//             }

//             internal bool HasPositionY
//             {
//                 get => (m_Bitset & (1 << k_PositionYBit)) != 0;
//                 set
//                 {
//                     if (value) { m_Bitset = (ushort)(m_Bitset | (1 << k_PositionYBit)); }
//                     else { m_Bitset = (ushort)(m_Bitset & ~(1 << k_PositionYBit)); }
//                 }
//             }

//             internal bool HasPositionZ
//             {
//                 get => (m_Bitset & (1 << k_PositionZBit)) != 0;
//                 set
//                 {
//                     if (value) { m_Bitset = (ushort)(m_Bitset | (1 << k_PositionZBit)); }
//                     else { m_Bitset = (ushort)(m_Bitset & ~(1 << k_PositionZBit)); }
//                 }
//             }

//             // RotAngles


//             internal bool HasRotAngleY
//             {
//                 get => (m_Bitset & (1 << k_RotAngleYBit)) != 0;
//                 set
//                 {
//                     if (value) { m_Bitset = (ushort)(m_Bitset | (1 << k_RotAngleYBit)); }
//                     else { m_Bitset = (ushort)(m_Bitset & ~(1 << k_RotAngleYBit)); }
//                 }
//             }



//             internal bool IsTeleportingNextFrame
//             {
//                 get => (m_Bitset & (1 << k_TeleportingBit)) != 0;
//                 set
//                 {
//                     if (value) { m_Bitset = (ushort)(m_Bitset | (1 << k_TeleportingBit)); }
//                     else { m_Bitset = (ushort)(m_Bitset & ~(1 << k_TeleportingBit)); }
//                 }
//             }

//             internal float PositionX, PositionY, PositionZ;
//             internal byte  RotAngleY;
//             internal float SentTime;

//             internal Vector3 Position
//             {
//                 get { return new Vector3(PositionX, PositionY, PositionZ); }
//                 set
//                 {
//                     PositionX = value.x;
//                     PositionY = value.y;
//                     PositionZ = value.z;
//                 }
//             }

//             internal byte Rotation
//             {
//                 get { return  RotAngleY; }
//                 set
//                 {
//                     RotAngleY = value;
//                 }
//             }


//             public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
//             {
//                 serializer.SerializeValue(ref SentTime);
//                 // InLocalSpace + HasXXX Bits
//                 serializer.SerializeValue(ref m_Bitset);
//                 // Position Values
//                 if (HasPositionX)
//                 {
//                     serializer.SerializeValue(ref PositionX);
//                 }

//                 if (HasPositionY)
//                 {
//                     serializer.SerializeValue(ref PositionY);
//                 }

//                 if (HasPositionZ)
//                 {
//                     serializer.SerializeValue(ref PositionZ);
//                 }

//                 // RotAngle Values

//                 if (HasRotAngleY)
//                 {
//                     serializer.SerializeValue(ref RotAngleY);
//                 }

//                 // Scale Values
//             }
//         }

//         public bool SyncPositionX = true, SyncPositionY = true, SyncPositionZ = true;
//         public bool  SyncRotAngleY = true;

//         public float PositionThreshold = PositionThresholdDefault;

//         [Range(0.001f, 360.0f)]
//         public float RotAngleThreshold = RotAngleThresholdDefault;

//         // /// <summary>
//         // /// Sets whether this transform should sync in local space or in world space.
//         // /// This is important to set since reparenting this transform could have issues,
//         // /// if using world position (depending on who gets synced first: the parent or the child)
//         // /// Having a child always at position 0,0,0 for example will have less possibilities of desync than when using world positions
//         // /// </summary>
//         // [Tooltip("Sets whether this transform should sync in local space or in world space")]
//         // private bool m_LastInterpolateLocal = false; // was the last frame local

//         public bool Interpolate = true;
//         private bool m_LastInterpolate = true; // was the last frame interpolated

//         /// <summary>
//         /// Used to determine who can write to this transform. Server only for this transform.
//         /// Changing this value alone in a child implementation will not allow you to create a NetworkTransform which can be written to by clients. See the ClientNetworkTransform Sample
//         /// in the package samples for how to implement a NetworkTransform with client write support.
//         /// If using different values, please use RPCs to write to the server. Netcode doesn't support client side network variable writing
//         /// </summary>
//         // This is public to make sure that users don't depend on this IsClient && IsOwner check in their code. If this logic changes in the future, we can make it invisible here
//         public bool CanCommitToTransform { get; protected set; }
//         protected bool m_CachedIsServer;
//         protected NetworkManager m_CachedNetworkManager;

//         private readonly NetworkVariable<NetTransformState> m_ReplicatedNetworkState = new NetworkVariable<NetTransformState>(new NetTransformState());




//         private NetTransformState m_LocalAuthoritativeNetworkState;

//         private ClientRpcParams m_ClientRpcParams = new ClientRpcParams() { Send = new ClientRpcSendParams() };

//         private List<ulong> m_ClientIds = new List<ulong>() { 0 };



//         private const int k_DebugDrawLineTime = 10;

//         private bool m_HasSentLastValue = false; // used to send one last value, so clients can make the difference between lost replication data (clients extrapolate) and no more data to send.


//         private BufferedLinearInterpo<float> m_PositionXInterpolator; // = new BufferedLinearInterpolatorFloat();
//         private BufferedLinearInterpo<float> m_PositionYInterpolator; // = new BufferedLinearInterpolatorFloat();
//         private BufferedLinearInterpo<float> m_PositionZInterpolator; // = new BufferedLinearInterpolatorFloat();

//         private readonly List<BufferedLinearInterpo<float>> m_AllFloatInterpolators = new List<BufferedLinearInterpo<float>>(3);

//         private Transform m_Transform; // cache the transform component to reduce unnecessary bounce between managed and native
//         private int m_LastSentTick;
//         private NetTransformState m_LastSentState;

//         /// <summary>
//         /// Tries updating the server authoritative transform, only if allowed.
//         /// If this called server side, this will commit directly.
//         /// If no update is needed, nothing will be sent. This method should still be called every update, it'll self manage when it should and shouldn't send
//         /// </summary>
//         /// <param name="transformToCommit"></param>
//         /// <param name="dirtyTime"></param>
//         protected void TryCommitTransformToServer(Transform transformToCommit, float dirtyTime)
//         {
//             var isDirty = ApplyTransformToNetworkState(ref m_LocalAuthoritativeNetworkState, dirtyTime, transformToCommit);
//             TryCommit(isDirty);
//         }

//         private void TryCommitValuesToServer(Vector3 position, Vector3 rotation, float dirtyTime)
//         {
//             var isDirty = ApplyTransformToNetworkStateWithInfo(ref m_LocalAuthoritativeNetworkState, dirtyTime, position, (byte)rotation.y);

//             TryCommit(isDirty.isDirty);
//         }

//         private void TryCommit(bool isDirty)
//         {
//             void Send(NetTransformState stateToSend)
//             {

//                 if (m_CachedIsServer)
//                 {
//                     // server RPC takes a few frames to execute server side, we want this to execute immediately
//                     CommitLocallyAndReplicate(stateToSend);
//                 }
//                 else
//                 {
//                     CommitTransformServerRpc(stateToSend);
//                 }
//             }

//             // if dirty, send
//             // if not dirty anymore, but hasn't sent last value for limiting extrapolation, still set isDirty
//             // if not dirty and has already sent last value, don't do anything
//             // extrapolation works by using last two values. if it doesn't receive anything anymore, it'll continue to extrapolate.
//             // This is great in case there's message loss, not so great if we just don't have new values to send.
//             // the following will send one last "copied" value so unclamped interpolation tries to extrapolate between two identical values, effectively
//             // making it immobile.
//             if (isDirty)
//             {
//                 Send(m_LocalAuthoritativeNetworkState);
//                 m_HasSentLastValue = false;
//                 m_LastSentTick = m_CachedNetworkManager.LocalTime.Tick;
//                 m_LastSentState = m_LocalAuthoritativeNetworkState;
//             }
//             else if (!m_HasSentLastValue && m_CachedNetworkManager.LocalTime.Tick >= m_LastSentTick + 1) // check for state.IsDirty since update can happen more than once per tick. No need for client, RPCs will just queue up
//             {
//                 m_LastSentState.SentTime = m_CachedNetworkManager.LocalTime.TimeAsFloat; // time 1+ tick later
//                 Send(m_LastSentState);
//                 m_HasSentLastValue = true;
//             }
//         }

//         [ServerRpc(RequireOwnership = false)]
//         private void CommitTransformServerRpc(NetTransformState networkState, ServerRpcParams serverParams = default)
//         {
//             if (serverParams.Receive.SenderClientId == OwnerClientId) // RPC call when not authorized to write could happen during the RTT interval during which a server's ownership change hasn't reached the client yet
//             {
//                 CommitLocallyAndReplicate(networkState);
//             }
//         }

//         private void CommitLocallyAndReplicate(NetTransformState networkState)
//         {
//             m_ReplicatedNetworkState.Value = networkState; 

//             if (Interpolate)
//             {
//                 AddInterpolatedState(networkState);
//             }
//         }

//         private void ResetInterpolatedStateToCurrentAuthoritativeState()
//         {
//             var serverTime = NetworkManager.ServerTime.TimeAsFloat;
//             m_PositionXInterpolator.ResetTo(m_LocalAuthoritativeNetworkState.PositionX, serverTime);
//             m_PositionYInterpolator.ResetTo(m_LocalAuthoritativeNetworkState.PositionY, serverTime);
//             m_PositionZInterpolator.ResetTo(m_LocalAuthoritativeNetworkState.PositionZ, serverTime);

//             // m_RotationInterpolator.ResetTo(Quaternion.Euler(0,m_LocalAuthoritativeNetworkState.Rotation,0), serverTime);
//         }

//         /// <summary>
//         /// Will apply the transform to the LocalAuthoritativeNetworkState and get detailed isDirty information returned.
//         /// </summary>
//         /// <param name="transform">transform to apply</param>
//         /// <returns>bool isDirty, bool isPositionDirty, bool isRotationDirty, bool isScaleDirty</returns>
//         internal (bool isDirty, bool isPositionDirty, bool isRotationDirty) ApplyLocalNetworkState(Transform transform)
//         {
//             return ApplyTransformToNetworkStateWithInfo(ref m_LocalAuthoritativeNetworkState, m_CachedNetworkManager.LocalTime.TimeAsFloat, transform);
//         }

//         // updates `NetworkState` properties if they need to and returns a `bool` indicating whether or not there was any changes made
//         // returned boolean would be useful to change encapsulating `NetworkVariable<NetworkState>`'s dirty state, e.g. ReplNetworkState.SetDirty(isDirty);
//         internal bool ApplyTransformToNetworkState(ref NetTransformState networkState, float dirtyTime, Transform transformToUse)
//         {
//             return ApplyTransformToNetworkStateWithInfo(ref networkState, dirtyTime, transformToUse).isDirty;
//         }

//         private (bool isDirty, bool isPositionDirty, bool isRotationDirty) ApplyTransformToNetworkStateWithInfo(ref NetTransformState networkState, float dirtyTime, Transform transformToUse)
//         {
//             var position = transformToUse.position;
//             var rotAnglesY = (byte)transformToUse.eulerAngles.y;
//             return ApplyTransformToNetworkStateWithInfo(ref networkState, dirtyTime, position, rotAnglesY);
//         }

//         private (bool isDirty, bool isPositionDirty, bool isRotationDirty) ApplyTransformToNetworkStateWithInfo(ref NetTransformState networkState, float dirtyTime, Vector3 position, byte rotAnglesY)
//         {
//             var isDirty = false;
//             var isPositionDirty = false;
//             var isRotationDirty = false;

//             // hasPositionZ set to false when it should be true?

//             // we assume that if x, y or z are dirty then we'll have to send all 3 anyway, so for efficiency
//             //  we skip doing the (quite expensive) Math.Approximately() and check against PositionThreshold
//             //  this still is overly costly and could use more improvements.
//             //
//             // (ditto for scale components)
//             if (SyncPositionX &&
//                 Mathf.Abs(networkState.PositionX - position.x) > PositionThreshold)
//             {
//                 networkState.PositionX = position.x;
//                 networkState.HasPositionX = true;
//                 isPositionDirty = true;
//             }

//             if (SyncPositionY &&
//                 Mathf.Abs(networkState.PositionY - position.y) > PositionThreshold)
//             {
//                 networkState.PositionY = position.y;
//                 networkState.HasPositionY = true;
//                 isPositionDirty = true;
//             }

//             if (SyncPositionZ &&
//                 Mathf.Abs(networkState.PositionZ - position.z) > PositionThreshold)
//             {
//                 networkState.PositionZ = position.z;
//                 networkState.HasPositionZ = true;
//                 isPositionDirty = true;
//             }


//             if (SyncRotAngleY &&
//                 Mathf.Abs(Mathf.DeltaAngle(networkState.RotAngleY, rotAnglesY)) > RotAngleThreshold)
//             {
//                 networkState.RotAngleY = rotAnglesY;
//                 networkState.HasRotAngleY = true;
//                 isRotationDirty = true;
//             }

//             isDirty |= isPositionDirty || isRotationDirty ;

//             if (isDirty)
//             {
//                 networkState.SentTime = dirtyTime;
//             }

//             return (isDirty, isPositionDirty, isRotationDirty);
//         }

//         private void ApplyInterpolatedNetworkStateToTransform(NetTransformState networkState, Transform transformToUpdate)
//         {
//             var interpolatedPosition =  transformToUpdate.position;

//             // todo: we should store network state w/ quats vs. euler angles
//             // var interpolatedRotAngles = transformToUpdate.eulerAngles;
//             // var interpolatedScale = transformToUpdate.localScale;

//             // Position Read

//             interpolatedPosition.x = networkState.IsTeleportingNextFrame || !Interpolate ? networkState.Position.x : m_PositionXInterpolator.GetInterpolatedValue();
            
//             if (((CheckGoundedClose(distanceY) || networkState.IsTeleportingNextFrame) && (networkState.Position.y - interpolatedPosition.y < 0.1) )  ){
//                 interpolatedPosition.y = networkState.Position.y;     
//             }
//             else{
//                 interpolatedPosition.y = m_PositionYInterpolator.GetInterpolatedValue();
//             }
            
//             // interpolatedPosition.y = networkState.IsTeleportingNextFrame || !Interpolate ? networkState.Position.y : m_PositionYInterpolator.GetInterpolatedValue();
        

//             interpolatedPosition.z = networkState.IsTeleportingNextFrame || !Interpolate ? networkState.Position.z : m_PositionZInterpolator.GetInterpolatedValue();

//             // Position Apply
            
//             transformToUpdate.position = interpolatedPosition;
//             // RotAngles Apply

//             if (lastRotaion != networkState.Rotation ){
//                 transformToUpdate.Rotate(new Vector3 (0,180,0));
//                 // if (networkState.Rotation <= 0 )  transformToUpdate.rotation = new Quaternion(0,0,0,1);
//                 // else if (networkState.Rotation >= 170 ) transformToUpdate.rotation = new Quaternion(0,1,0,0);
//                 lastRotaion = networkState.Rotation;
//             }    
            
            
            
//         }

//         private void AddInterpolatedState(NetTransformState newState, bool reset = false)
//         {
//             var sentTime = newState.SentTime;

//             if (reset)
//             {
//                 if (newState.HasPositionX)
//                 {
//                     m_PositionXInterpolator.ResetTo(newState.PositionX, sentTime);
//                 }

//                 if (newState.HasPositionY)
//                 {
//                     m_PositionYInterpolator.ResetTo(newState.PositionY, sentTime);
//                 }

//                 if (newState.HasPositionZ)
//                 {
//                     m_PositionZInterpolator.ResetTo(newState.PositionZ, sentTime);
//                 }
//                 return;
//             }
//             if (newState.HasPositionX)
//             {
//                 m_PositionXInterpolator.AddMeasurement(newState.PositionX, sentTime);
//             }

//             if (newState.HasPositionY)
//             {
//                 m_PositionYInterpolator.AddMeasurement(newState.PositionY, sentTime);
//             }

//             if (newState.HasPositionZ)
//             {
//                 m_PositionZInterpolator.AddMeasurement(newState.PositionZ, sentTime);
//             }
//         }

//         public bool CheckGoundedClose(float distance){
//             int number = Physics.RaycastNonAlloc(m_BoxCollider.bounds.center,Vector3.down ,GroundHit,m_BoxCollider.bounds.extents.y+distance,k_GroundLayerMask);
            
//             // Color rayColor;
//             // if (number == 0){
//             //     rayColor = Color.green;
//             // }else {
//             //     rayColor = Color.red;
//             // }
//             // Debug.DrawRay(m_BoxCollider.bounds.center , (m_BoxCollider.bounds.extents.y+distance)*Vector3.down,rayColor);
//             if (number == 0 ) return false;
//             return true;
//         }  

//         private void OnNetworkStateChanged(NetTransformState oldState, NetTransformState newState)
//         {
//             if (!NetworkObject.IsSpawned)
//             {
//                 return;
//             }

//             if (CanCommitToTransform)
//             {
//                 // we're the authority, we ignore incoming changes
//                 return;
//             }

//             if (Interpolate)
//             {
//                 AddInterpolatedState(newState);
//             }

//             if (m_CachedNetworkManager.LogLevel == LogLevel.Developer)
//             {
//                 var pos = new Vector3(newState.PositionX, newState.PositionY, newState.PositionZ);
//             }
//         }

//         public void SetMaxInterpolationBound(float maxInterpolationBound)
//         {
//             m_PositionXInterpolator.MaxInterpolationBound = maxInterpolationBound;
//             m_PositionYInterpolator.MaxInterpolationBound = maxInterpolationBound;
//             m_PositionZInterpolator.MaxInterpolationBound = maxInterpolationBound;
//         }

//         private void Awake()
//         {
//             // we only want to create our interpolators during Awake so that, when pooled, we do not create tons
//             //  of gc thrash each time objects wink out and are re-used
//             m_PositionXInterpolator = new BufferedLinearInterFloat();
//             m_PositionYInterpolator = new BufferedLinearInterFloat();
//             m_PositionZInterpolator = new BufferedLinearInterFloat();


//             if (m_AllFloatInterpolators.Count == 0)
//             {
//                 m_AllFloatInterpolators.Add(m_PositionXInterpolator);
//                 m_AllFloatInterpolators.Add(m_PositionYInterpolator);
//                 m_AllFloatInterpolators.Add(m_PositionZInterpolator);

//             }
//         }

//         public override void OnNetworkSpawn()
//         {
//             // must set up m_Transform in OnNetworkSpawn because it's possible an object spawns but is disabled
//             //  and thus awake won't be called.
//             // TODO: investigate further on not sending data for something that is not enabled
//             m_Transform = transform;
//             m_ReplicatedNetworkState.OnValueChanged += OnNetworkStateChanged;
//             k_GroundLayerMask = LayerMask.GetMask(new[] { "Ground" });

//             lastRotaion = (byte)m_Transform.rotation.eulerAngles.y;
//             CanCommitToTransform = IsServer;
//             m_CachedIsServer = IsServer;
//             m_CachedNetworkManager = NetworkManager;

//             if (CanCommitToTransform)
//             {
//                 TryCommitTransformToServer(m_Transform, m_CachedNetworkManager.LocalTime.TimeAsFloat);
//             }
//             m_LocalAuthoritativeNetworkState = m_ReplicatedNetworkState.Value;

//             // crucial we do this to reset the interpolators so that recycled objects when using a pool will
//             //  not have leftover interpolator state from the previous object
//             Initialize();

//             CanCommitToTransform = IsOwner;

//         }


//         public override void OnNetworkDespawn()
//         {
//             m_ReplicatedNetworkState.OnValueChanged -= OnNetworkStateChanged;
//         }

//         public override void OnGainedOwnership()
//         {
//             Initialize();
//         }

//         public override void OnLostOwnership()
//         {
//             Initialize();
//         }

//         private void Initialize()
//         {
//             ResetInterpolatedStateToCurrentAuthoritativeState(); // useful for late joining

//             if (CanCommitToTransform)
//             {
//                 m_ReplicatedNetworkState.SetDirty(true);
//             }
//             else if (m_Transform != null)
//             {
//                 ApplyInterpolatedNetworkStateToTransform(m_ReplicatedNetworkState.Value, m_Transform);
//             }
//         }

//         /// <summary>
//         /// Directly sets a state on the authoritative transform.
//         /// This will override any changes made previously to the transform
//         /// This isn't resistant to network jitter. Server side changes due to this method won't be interpolated.
//         /// The parameters are broken up into pos / rot / scale on purpose so that the caller can perturb
//         ///  just the desired one(s)
//         /// </summary>
//         /// <param name="posIn"></param> new position to move to.  Can be null
//         /// <param name="rotIn"></param> new rotation to rotate to.  Can be null
//         /// <param name="scaleIn">new scale to scale to. Can be null</param>
//         /// <param name="shouldGhostsInterpolate">Should other clients interpolate this change or not. True by default</param>
//         /// new scale to scale to.  Can be null
//         /// <exception cref="Exception"></exception>
//         public void SetState(Vector3? posIn = null, Quaternion? rotIn = null, Vector3? scaleIn = null, bool shouldGhostsInterpolate = true)
//         {
//             if (!IsOwner)
//             {
//                 throw new Exception("Trying to set a state on a not owned transform");
//             }

//             if (m_CachedNetworkManager && !(m_CachedNetworkManager.IsConnectedClient || m_CachedNetworkManager.IsListening))
//             {
//                 return;
//             }

//             Vector3 pos = posIn == null ? transform.position : (Vector3)posIn;
//             Quaternion rot = rotIn == null ? transform.rotation : (Quaternion)rotIn;
//             var rotY = (byte)rot.eulerAngles.y;
//             if (!CanCommitToTransform)
//             {
//                 if (!m_CachedIsServer)
//                 {
//                     SetStateServerRpc(pos, rotY, shouldGhostsInterpolate);
//                 }
//             }
//             else
//             {
//                 m_Transform.position = pos;
//                 m_Transform.rotation = rot;
//                 m_LocalAuthoritativeNetworkState.IsTeleportingNextFrame = shouldGhostsInterpolate;
//             }
//         }

//         [ServerRpc]
//         private void SetStateServerRpc(Vector3 pos, byte rotY, bool shouldTeleport)
//         {
//             // server has received this RPC request to move change transform.  Give the server a chance to modify or
//             //  even reject the move
//             if (OnClientRequestChange != null)
//             {
//                 (pos, rotY) = OnClientRequestChange(pos, rotY);
//             }
//             m_Transform.position = pos;
//             m_Transform.rotation = Quaternion.Euler(0,rotY,0) ;
//             m_LocalAuthoritativeNetworkState.IsTeleportingNextFrame = shouldTeleport;
//         }

//         // todo this is currently in update, to be able to catch any transform changes. A FixedUpdate mode could be added to be less intense, but it'd be
//         // conditional to users only making transform update changes in FixedUpdate.
//         protected virtual void Update()
//         {
//             if (!IsSpawned)
//             {
//                 return;
//             }

//             if (!Interpolate && m_LastInterpolate)
//             {
//                 // if we just stopped interpolating, let's clear the interpolators
//                 foreach (var interpolator in m_AllFloatInterpolators)
//                 {
//                     interpolator.Clear();
//                 }
//             }

//             m_LastInterpolate = Interpolate;

//             if (CanCommitToTransform) // // Owner Player Send Transform value to Server 
//             {
//                 TryCommitTransformToServer(m_Transform, m_CachedNetworkManager.LocalTime.TimeAsFloat);
//             }
//             if (!CanCommitToTransform) // Remote Player Apply interpolation value to Transform 
//             {
//                 // apply interpolated value
//                 if (m_CachedNetworkManager.IsConnectedClient || m_CachedNetworkManager.IsListening)
//                 {
//                     // eventually, we could hoist this calculation so that it happens once for all objects, not once per object
//                     var cachedDeltaTime = Time.deltaTime;
//                     var serverTime = NetworkManager.ServerTime;
//                     var cachedServerTime = serverTime.Time;
//                     var cachedRenderTime = serverTime.TimeTicksAgo(1).Time;

//                     if (Interpolate)
//                     {
//                         foreach (var interpolator in m_AllFloatInterpolators)
//                         {
//                             interpolator.Update(cachedDeltaTime, cachedRenderTime, cachedServerTime);
//                         }
//                     }
//                         // Apply updated interpolated value
//                         ApplyInterpolatedNetworkStateToTransform(m_ReplicatedNetworkState.Value, m_Transform);
//                 }
//             }
//             m_LocalAuthoritativeNetworkState.IsTeleportingNextFrame = false;
//         }

//         /// <summary>
//         /// Teleports the transform to the given values without interpolating
//         /// </summary>
//         public void Teleport(Vector3 newPosition, Quaternion newRotation)
//         {
//             if (!CanCommitToTransform)
//             {
//                 throw new Exception("Teleport not allowed");
//             }

//             var newRotationEuler = newRotation.eulerAngles;
//             var stateToSend = m_LocalAuthoritativeNetworkState;
//             stateToSend.IsTeleportingNextFrame = true;
//             stateToSend.Position = newPosition;
//             stateToSend.Rotation = (byte)newRotationEuler.y;
//             ApplyInterpolatedNetworkStateToTransform(stateToSend, transform);
//             // set teleport flag in state to signal to ghosts not to interpolate
//             m_LocalAuthoritativeNetworkState.IsTeleportingNextFrame = true;
//             // check server side
//             TryCommitValuesToServer(newPosition, newRotationEuler, m_CachedNetworkManager.LocalTime.TimeAsFloat);
//             m_LocalAuthoritativeNetworkState.IsTeleportingNextFrame = false;
//         }

//         /// <summary>
//         /// Override this method and return false to switch to owner authoritative mode
//         /// </summary>
//         protected virtual bool OnIsServerAuthoritative()
//         {
//             return true;
//         }

//         /// <summary>
//         /// Used by <see cref="NetworkRigidbody"/> to determines if this is server or owner authoritative.
//         /// </summary>
//         internal bool IsServerAuthoritative()
//         {
//             return OnIsServerAuthoritative();
//         }
//     }
// }
