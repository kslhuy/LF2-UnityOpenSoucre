using System;
using System.Collections.Generic;
using LF2.Client;
using Unity.Netcode;
using UnityEngine;

namespace LF2
{
    /// <summary>
    /// A component for syncing transforms
    /// NetworkTransform will read the underlying transform and replicate it to clients.
    /// The replicated value will be automatically be interpolated (if active) and applied to the underlying GameObject's transform
    /// </summary>
    [DefaultExecutionOrder(100000)] // this is needed to catch the update time after the transform was updated by user scripts
    public class NetTransform : NetworkBehaviour
    {
        public const float PositionThresholdDefault = 0.5f;
        public const float RotAngleThresholdDefault = 1f;
        public delegate (Vector3 pos, byte rotYOut) OnClientRequestChangeDelegate(Vector3 pos, byte rotY);
        public OnClientRequestChangeDelegate OnClientRequestChange;

        //  
        private byte lastRotaion;


        [SerializeField] ClientCharacterMovement clientCharaterMovement;


        [SerializeField] private int distanceY;
        //
        internal struct NetTransformState : INetworkSerializable
        {

            private const int k_PositionXBit = 0;
            private const int k_PositionYBit = 1;
            private const int k_PositionZBit = 2;
            private const int k_RotAngleYBit = 3;
            private const int k_TeleportingBit = 4;

            // 11-15: <unused>
            private byte m_Bitset;


            // Position
            internal bool HasPositionX
            {
                get => (m_Bitset & (1 << k_PositionXBit)) != 0;
                set
                {
                    if (value) { m_Bitset = (byte)(m_Bitset | (1 << k_PositionXBit)); }
                    else { m_Bitset = (byte)(m_Bitset & ~(1 << k_PositionXBit)); }
                }
            }

            internal bool HasPositionY
            {
                get => (m_Bitset & (1 << k_PositionYBit)) != 0;
                set
                {
                    if (value) { m_Bitset = (byte)(m_Bitset | (1 << k_PositionYBit)); }
                    else { m_Bitset = (byte)(m_Bitset & ~(1 << k_PositionYBit)); }
                }
            }

            internal bool HasPositionZ
            {
                get => (m_Bitset & (1 << k_PositionZBit)) != 0;
                set
                {
                    if (value) { m_Bitset = (byte)(m_Bitset | (1 << k_PositionZBit)); }
                    else { m_Bitset = (byte)(m_Bitset & ~(1 << k_PositionZBit)); }
                }
            }

            internal bool HasPositionChange
            {
                get
                {
                    return HasPositionX | HasPositionY | HasPositionZ;
                }
            }

            // RotAngles


            internal bool HasRotAngleY
            {
                get => (m_Bitset & (1 << k_RotAngleYBit)) != 0;
                set
                {
                    if (value) { m_Bitset = (byte)(m_Bitset | (1 << k_RotAngleYBit)); }
                    else { m_Bitset = (byte)(m_Bitset & ~(1 << k_RotAngleYBit)); }
                }
            }



            internal bool IsTeleportingNextFrame
            {
                get => (m_Bitset & (1 << k_TeleportingBit)) != 0;
                set
                {
                    if (value) { m_Bitset = (byte)(m_Bitset | (1 << k_TeleportingBit)); }
                    else { m_Bitset = (byte)(m_Bitset & ~(1 << k_TeleportingBit)); }
                }
            }

            internal float PositionX, PositionY, PositionZ;
            internal byte RotAngleY; // can use sbyte Here
            internal double SentTime;

            // Authoritative and non-authoritative sides use this to determine if a NetworkTransformState is
            // dirty or not.
            internal bool IsDirty;

            // Non-Authoritative side uses this for ending extrapolation of the last applied state
            internal int EndExtrapolationTick;

            /// <summary>
            /// This will reset the NetworkTransform BitSet
            /// </summary>
            internal void ClearBitSetForNextTick()
            {
                m_Bitset &= 0;  // Clear all the bits in m_Bitset
                IsDirty = false;
            }

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref SentTime);
                // InLocalSpace + HasXXX Bits
                serializer.SerializeValue(ref m_Bitset);
                // Position Values
                if (HasPositionX)
                {
                    serializer.SerializeValue(ref PositionX);
                }

                if (HasPositionY)
                {
                    serializer.SerializeValue(ref PositionY);
                }

                if (HasPositionZ)
                {
                    serializer.SerializeValue(ref PositionZ);
                }

                // RotAngle Values

                if (HasRotAngleY)
                {
                    serializer.SerializeValue(ref RotAngleY);
                }

                // Only if we are receiving state
                if (serializer.IsReader)
                {
                    // Go ahead and mark the local state dirty or not dirty as well
                    /// <see cref="TryCommitTransformToServer"/>
                    if (HasPositionChange || HasRotAngleY)
                    {
                        IsDirty = true;
                    }
                    else
                    {
                        IsDirty = false;
                    }
                }
            }
        }
        /// Whether or not x , y ,z component of position will be replicated
        public bool SyncPositionX = true, SyncPositionY = true, SyncPositionZ = true;
        public bool SyncRotAngleY = true;

        public float PositionThreshold = PositionThresholdDefault;

        [Range(0.001f, 360.0f)]
        public float RotAngleThreshold = RotAngleThresholdDefault;


        public bool Interpolate = true;
        private bool m_LastInterpolate = true; // was the last frame interpolated

        /// <summary>
        /// Used to determine who can write to this transform. Server only for this transform.
        /// Changing this value alone in a child implementation will not allow you to create a NetworkTransform which can be written to by clients. See the ClientNetworkTransform Sample
        /// in the package samples for how to implement a NetworkTransform with client write support.
        /// If using different values, please use RPCs to write to the server. Netcode doesn't support client side network variable writing
        /// </summary>
        // This is public to make sure that users don't depend on this IsClient && IsOwner check in their code. If this logic changes in the future, we can make it invisible here
        public bool CanCommitToTransform { get; protected set; }
        protected bool m_CachedIsServer;
        protected NetworkManager m_CachedNetworkManager;


        /// <summary>
        /// We have two internal NetworkVariables.
        /// One for server authoritative and one for "client/owner" authoritative.
        /// </summary>
        private readonly NetworkVariable<NetTransformState> m_ReplicatedNetworkStateServer = new NetworkVariable<NetTransformState>(new NetTransformState(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private readonly NetworkVariable<NetTransformState> m_ReplicatedNetworkStateOwner = new NetworkVariable<NetTransformState>(new NetTransformState(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        internal NetworkVariable<NetTransformState> ReplicatedNetworkState
        {
            get
            {
                if (!IsServerAuthoritative())
                {
                    return m_ReplicatedNetworkStateOwner;
                }

                return m_ReplicatedNetworkStateServer;
            }
        }

        // Used by both authoritative and non-authoritative instances.
        // This represents the most recent local authoritative state.
        private NetTransformState m_LocalAuthoritativeNetworkState;

        private ClientRpcParams m_ClientRpcParams = new ClientRpcParams() { Send = new ClientRpcSendParams() };

        private List<ulong> m_ClientIds = new List<ulong>() { 0 };





        private BufferedLinearInterpo<float> m_PositionXInterpolator; 
        private BufferedLinearInterpo<float> m_PositionYInterpolator; 
        private BufferedLinearInterpo<float> m_PositionZInterpolator; 

        private readonly List<BufferedLinearInterpo<float>> m_AllFloatInterpolators = new List<BufferedLinearInterpo<float>>(3);

        private Transform m_Transform; // cache the transform component to reduce unnecessary bounce between managed and native
        // Used by integration test
        private NetTransformState m_LastSentState;

        internal NetTransformState GetLastSentState()
        {
            return m_LastSentState;
        }
        private const int k_DebugDrawLineTime = 10;

        private bool m_HasSentLastValue = false; // used to send one last value, so clients can make the difference between lost replication data (clients extrapolate) and no more data to send.


        /// <summary>
        /// Calculated when spawned, this is used to offset a newly received non-authority side state by 1 tick duration
        /// in order to end the extrapolation for that state's values.
        /// </summary>
        /// <remarks>
        /// Example:
        /// NetworkState-A is received, processed, and measurements added
        /// NetworkState-A is duplicated (NetworkState-A-Post) and its sent time is offset by the tick frequency
        /// One tick later, NetworkState-A-Post is applied to end that delta's extrapolation.
        /// <see cref="OnNetworkStateChanged"/> to see how NetworkState-A-Post doesn't get excluded/missed
        /// </remarks>
        private double m_TickFrequency;






        private void Awake()
        {
            // we only want to create our interpolators during Awake so that, when pooled, we do not create tons
            //  of gc thrash each time objects wink out and are re-used
            m_PositionXInterpolator = new BufferedLinearInterFloat();
            m_PositionYInterpolator = new BufferedLinearInterFloat();
            m_PositionZInterpolator = new BufferedLinearInterFloat();


            if (m_AllFloatInterpolators.Count == 0)
            {
                m_AllFloatInterpolators.Add(m_PositionXInterpolator);
                m_AllFloatInterpolators.Add(m_PositionYInterpolator);
                m_AllFloatInterpolators.Add(m_PositionZInterpolator);

            }
        }

        public override void OnNetworkSpawn()
        {
            m_CachedIsServer = IsServer;
            m_CachedNetworkManager = NetworkManager;
            m_TickFrequency = 1.0 / NetworkManager.NetworkConfig.TickRate;
            // CanCommitToTransform = IsServer;
            Initialize();
            // This assures the initial spawning of the object synchronizes all connected clients
            // with the current transform values. This should not be placed within Initialize since
            // that can be invoked when ownership changes.
            if (CanCommitToTransform)
            {
                var currentPosition = transform.position;
                var currentRotation = transform.rotation;
                
                // Teleport to current position
                SetStateInternal(currentPosition, (byte)currentRotation.y, true);

                // Force the state update to be sent
                TryCommitTransform(transform, m_CachedNetworkManager.LocalTime.Time);
            }
            lastRotaion = (byte)transform.rotation.eulerAngles.y;
            m_LocalAuthoritativeNetworkState.IsTeleportingNextFrame = false;

        }


        /// <inheritdoc/>
        public override void OnNetworkDespawn()
        {
            ReplicatedNetworkState.OnValueChanged -= OnNetworkStateChanged;
        }

        /// <inheritdoc/>
        public override void OnDestroy()
        {
            base.OnDestroy();
            m_ReplicatedNetworkStateServer.Dispose();
            m_ReplicatedNetworkStateOwner.Dispose();
        }

        /// <inheritdoc/>
        public override void OnGainedOwnership()
        {
            Initialize();
        }

        /// <inheritdoc/>
        public override void OnLostOwnership()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (!IsSpawned)
            {
                return;
            }

            CanCommitToTransform = IsServerAuthoritative() ? IsServer : IsOwner;
            var replicatedState = ReplicatedNetworkState;
            m_LocalAuthoritativeNetworkState = replicatedState.Value;

            if (CanCommitToTransform)
            {
                replicatedState.OnValueChanged -= OnNetworkStateChanged;
            }
            else
            {
                replicatedState.OnValueChanged += OnNetworkStateChanged;

                // In case we are late joining
                ResetInterpolatedStateToCurrentAuthoritativeState();
            }
        }

        /// <summary>
        /// This will try to send/commit the current transform delta states (if any)
        /// </summary>
        /// <remarks>
        /// Only client owners or the server should invoke this method
        /// </remarks>
        /// <param name="transformToCommit">the transform to be committed</param>
        /// <param name="dirtyTime">time it was marked dirty</param>
        protected void TryCommitTransformToServer(Transform transformToCommit, double dirtyTime)
        {
            // Only client owners or the server should invoke this method
            if (!IsOwner && !m_CachedIsServer)
            {
                NetworkLog.LogError($"Non-owner instance, {name}, is trying to commit a transform!");
                return;
            }

            // If we are authority, update the authoritative state
            if (CanCommitToTransform)
            {
                UpdateAuthoritativeState(transform);
            }
            else // Non-Authority
            {
                // We are an owner requesting to update our state
                if (!m_CachedIsServer)
                {
                    SetStateServerRpc(transformToCommit.position, (byte)transformToCommit.rotation.y, false);
                }
                else // Server is always authoritative (including owner authoritative)
                {
                    SetStateClientRpc(transformToCommit.position, (byte)transformToCommit.rotation.y, false);
                }
            }
        }

        /// <summary>
        /// Authoritative side only
        /// If there are any transform delta states, this method will synchronize the
        /// state with all non-authority instances.
        /// </summary>
        private void TryCommitTransform(Transform transformToCommit, double dirtyTime)
        {
            if (!CanCommitToTransform && !IsOwner)
            {
                NetworkLog.LogError($"[{name}] is trying to commit the transform without authority!");
                return;
            }

            // If the transform has deltas (returns dirty) then...
            if (ApplyTransformToNetworkState(ref m_LocalAuthoritativeNetworkState, dirtyTime, transformToCommit))
            {
                // ...commit the state
                ReplicatedNetworkState.Value = m_LocalAuthoritativeNetworkState;
            }
        }




        private void ResetInterpolatedStateToCurrentAuthoritativeState()
        {
            var serverTime = NetworkManager.ServerTime.Time;
            var position = transform.position;
            m_PositionXInterpolator.ResetTo(position.x, serverTime);
            m_PositionYInterpolator.ResetTo(position.y, serverTime);
            m_PositionZInterpolator.ResetTo(position.z, serverTime);

            // m_RotationInterpolator.ResetTo(Quaternion.Euler(0,m_LocalAuthoritativeNetworkState.Rotation,0), serverTime);
        }

        /// <summary>
        /// Used for integration testing:
        /// Will apply the transform to the LocalAuthoritativeNetworkState and get detailed dirty information returned
        /// in the <see cref="NetworkTransformState"/> returned.
        /// </summary>
        /// <param name="transform">transform to apply</param>
        /// <returns>NetworkTransformState</returns>
        internal NetTransformState ApplyLocalNetworkState(Transform transform)
        {
            // Since we never commit these changes, we need to simulate that any changes were committed previously and the bitset
            // value would already be reset prior to having the state applied
            m_LocalAuthoritativeNetworkState.ClearBitSetForNextTick();

            // Now check the transform for any threshold value changes
            ApplyTransformToNetworkStateWithInfo(ref m_LocalAuthoritativeNetworkState, m_CachedNetworkManager.LocalTime.Time, transform);

            // Return the entire state to be used by the integration test
            return m_LocalAuthoritativeNetworkState;
        }


        /// <summary>
        /// Used for integration testing
        /// </summary>
        internal bool ApplyTransformToNetworkState(ref NetTransformState networkState, double dirtyTime, Transform transformToUse)
        {
            return ApplyTransformToNetworkStateWithInfo(ref networkState, dirtyTime, transformToUse);
        }
        private bool ApplyTransformToNetworkStateWithInfo(ref NetTransformState networkState, double dirtyTime, Transform transformToUse)
        {

            var isDirty = false;
            var isPositionDirty = false;
            var isRotationDirty = false;

            var position = transformToUse.position;
            var rotAnglesY = (byte)transformToUse.eulerAngles.y;

            // hasPositionZ set to false when it should be true?

            // we assume that if x, y or z are dirty then we'll have to send all 3 anyway, so for efficiency
            //  we skip doing the (quite expensive) Math.Approximately() and check against PositionThreshold
            //  this still is overly costly and could use more improvements.
            //
            // (ditto for scale components)
            if (SyncPositionX &&
                Mathf.Abs(networkState.PositionX - position.x) >= PositionThreshold || networkState.IsTeleportingNextFrame)
            {
                networkState.PositionX = position.x;
                networkState.HasPositionX = true;
                isPositionDirty = true;
            }

            if (SyncPositionY &&
                Mathf.Abs(networkState.PositionY - position.y) >= PositionThreshold || networkState.IsTeleportingNextFrame)
            {
                networkState.PositionY = position.y;
                networkState.HasPositionY = true;
                isPositionDirty = true;
            }

            if (SyncPositionZ &&
                Mathf.Abs(networkState.PositionZ - position.z) >= PositionThreshold || networkState.IsTeleportingNextFrame)
            {
                networkState.PositionZ = position.z;
                networkState.HasPositionZ = true;
                isPositionDirty = true;
            }


            if (SyncRotAngleY && networkState.RotAngleY != rotAnglesY || networkState.IsTeleportingNextFrame)
            {
                // Debug.Log(Mathf.Abs((networkState.RotAngleY - rotAnglesY))); 
                // Debug.Log((" isTeleporNextFrame " + networkState.IsTeleportingNextFrame));
                
                Debug.Log((" networkState.RotAngleY " + networkState.RotAngleY  + "  vs  "+ " rotAnglesY " +  rotAnglesY)); 
                networkState.RotAngleY = rotAnglesY;
                networkState.HasRotAngleY = true;
                isRotationDirty = true;
            }

            isDirty |= isPositionDirty || isRotationDirty;


            if (isDirty)
            {
                networkState.SentTime = dirtyTime;
            }

            /// We need to set this in order to know when we can reset our local authority state <see cref="Update"/>
            /// If our state is already dirty or we just found deltas (i.e. isDirty == true)
            networkState.IsDirty |= isDirty;
            return isDirty;
        }

        private void ApplyAuthoritativeState()
        {
            var networkState = ReplicatedNetworkState.Value;
            var adjustedPosition = transform.position;


            // todo: we should store network state w/ quats vs. euler angles
            // var interpolatedRotAngles = transformToUpdate.eulerAngles;
            // var interpolatedScale = transformToUpdate.localScale;

            // Position Read

            var useInterpolatedValue = !networkState.IsTeleportingNextFrame && Interpolate;
            if (useInterpolatedValue)
            {
                if (SyncPositionX) { adjustedPosition.x = m_PositionXInterpolator.GetInterpolatedValue(); }
                if (SyncPositionZ) { adjustedPosition.z = m_PositionZInterpolator.GetInterpolatedValue(); }
            }
            else
            {
                if (networkState.HasPositionX) { adjustedPosition.x = networkState.PositionX; }
                if (networkState.HasPositionZ) { adjustedPosition.z = networkState.PositionZ; }
            }


            if (((clientCharaterMovement.CheckGoundedClose(distanceY) || networkState.IsTeleportingNextFrame) && (networkState.PositionY - adjustedPosition.y < 0.2)))
            {
                adjustedPosition.y = networkState.PositionY;
            }
            else
            {
                adjustedPosition.y = m_PositionYInterpolator.GetInterpolatedValue();
            }

            // Position Apply

            transform.position = adjustedPosition;
            // RotAngles Apply

            // Dont Need This code below but i keep that 
            if (lastRotaion != networkState.RotAngleY)
            {
                Debug.Log("last" +  lastRotaion + " vs " + " networkState.RotAngleY " + networkState.RotAngleY  );
                // Debug.Log(networkState.RotAngleY);

                transform.Rotate(new Vector3(0, 180, 0));
                // if (networkState.Rotation <= 0 )  transformToUpdate.rotation = new Quaternion(0,0,0,1);
                // else if (networkState.Rotation >= 170 ) transformToUpdate.rotation = new Quaternion(0,1,0,0);
                lastRotaion = networkState.RotAngleY;
            }


        }






        private void OnNetworkStateChanged(NetTransformState oldState, NetTransformState newState)
        {
            if (!NetworkObject.IsSpawned)
            {
                return;
            }

            if (CanCommitToTransform)
            {
                // we're the authority, we ignore incoming changes
                return;
            }

            if (Interpolate)
            {
                // Add measurements for the new state's deltas
                AddInterpolatedState(newState);
            }
        }

        
        private void AddInterpolatedState(NetTransformState newState)
        {
            var sentTime = newState.SentTime;
            var currentPosition = transform.position;

            if (newState.IsTeleportingNextFrame)
            {
                // we should clear our float interpolators
                foreach (var interpolator in m_AllFloatInterpolators)
                {
                    interpolator.Clear();
                }



                if (newState.HasPositionX)
                {
                    m_PositionXInterpolator.ResetTo(newState.PositionX, sentTime);
                    currentPosition.x = newState.PositionX;

                }

                if (newState.HasPositionY)
                {
                    m_PositionYInterpolator.ResetTo(newState.PositionY, sentTime);
                    currentPosition.y = newState.PositionY;

                }

                if (newState.HasPositionZ)
                {
                    m_PositionZInterpolator.ResetTo(newState.PositionZ, sentTime);
                    currentPosition.z = newState.PositionZ;

                }



                // Apply the position

                transform.position = currentPosition;

                if (newState.HasRotAngleY) {
                    transform.Rotate(0, 180, 0);
                    lastRotaion = (byte)transform.eulerAngles.y;
                    clientCharaterMovement.ChangeValueFacingDirection(lastRotaion);

                }
                return;
            }


            if (newState.HasPositionX)
            {
                m_PositionXInterpolator.AddMeasurement(newState.PositionX, sentTime);
            }

            if (newState.HasPositionY)
            {
                m_PositionYInterpolator.AddMeasurement(newState.PositionY, sentTime);
            }

            if (newState.HasPositionZ)
            {
                m_PositionZInterpolator.AddMeasurement(newState.PositionZ, sentTime);
            }
            
            
            if (newState.HasRotAngleY) {
                transform.Rotate(0, 180, 0);
                lastRotaion = (byte)transform.eulerAngles.y;
                clientCharaterMovement.ChangeValueFacingDirection(lastRotaion);
            }

            Debug.Log( " Has Position X" + newState.HasPositionX);
            Debug.Log( " Has Rotation Y" + newState.HasRotAngleY); 
            Debug.Log( " Has Rotation Z" + newState.HasRotAngleY); 

        }

        public void SetMaxInterpolationBound(float maxInterpolationBound)
        {
            m_PositionXInterpolator.MaxInterpolationBound = maxInterpolationBound;
            m_PositionYInterpolator.MaxInterpolationBound = maxInterpolationBound;
            m_PositionZInterpolator.MaxInterpolationBound = maxInterpolationBound;
        }



        /// <summary>
        /// Directly sets a state on the authoritative transform.
        /// This will override any changes made previously to the transform
        /// This isn't resistant to network jitter. Server side changes due to this method won't be interpolated.
        /// The parameters are broken up into pos / rot / scale on purpose so that the caller can perturb
        ///  just the desired one(s)
        /// </summary>
        /// <param name="posIn"></param> new position to move to.  Can be null
        /// <param name="rotIn"></param> new rotation to rotate to.  Can be null
        /// <param name="scaleIn">new scale to scale to. Can be null</param>
        /// <param name="shouldGhostsInterpolate">Should other clients interpolate this change or not. True by default</param>
        /// new scale to scale to.  Can be null
        /// <exception cref="Exception"></exception>
        public void SetState(Vector3? posIn = null, Quaternion? rotIn = null, Vector3? scaleIn = null, bool shouldGhostsInterpolate = true)
        {
            if (!IsSpawned)
            {
                return;
            }

            // Only the server or owner can invoke this method
            if (!IsOwner && !m_CachedIsServer)
            {
                throw new Exception("Non-owner client instance cannot set the state of the NetworkTransform!");
            }

            Vector3 pos = posIn == null ? transform.position : posIn.Value;
            Quaternion rot = rotIn == null ? transform.rotation : rotIn.Value;
            var rotY = (byte)rot.eulerAngles.y;

            if (!CanCommitToTransform)
            {
                // Preserving the ability for owner authoritative mode to accept state changes from server
                if (m_CachedIsServer)
                {
                    m_ClientIds[0] = OwnerClientId;
                    m_ClientRpcParams.Send.TargetClientIds = m_ClientIds;
                    SetStateClientRpc(pos, rotY, !shouldGhostsInterpolate, m_ClientRpcParams);
                }
                else // Preserving the ability for server authoritative mode to accept state changes from owner
                {
                    SetStateServerRpc(pos, rotY, !shouldGhostsInterpolate);
                }
                return;
            }
            SetStateInternal(pos, rotY, !shouldGhostsInterpolate);


        }

        /// <summary>
        /// Authoritative only method
        /// Sets the internal state (teleporting or just set state) of the authoritative
        /// transform directly.
        /// </summary>
        private void SetStateInternal(Vector3 pos, byte rotY, bool shouldTeleport)
        {
            transform.position = pos;
            transform.rotation = Quaternion.Euler(0, rotY, 0);

            m_LocalAuthoritativeNetworkState.IsTeleportingNextFrame = shouldTeleport;

            TryCommitTransform(transform, m_CachedNetworkManager.LocalTime.Time);
        }

        [ServerRpc]
        private void SetStateServerRpc(Vector3 pos, byte rotY, bool shouldTeleport)
        {
            // server has received this RPC request to move change transform.  Give the server a chance to modify or
            //  even reject the move
            if (OnClientRequestChange != null)
            {
                (pos, rotY) = OnClientRequestChange(pos, rotY);
            }
            m_Transform.position = pos;
            m_Transform.rotation = Quaternion.Euler(0, rotY, 0);
            m_LocalAuthoritativeNetworkState.IsTeleportingNextFrame = shouldTeleport;
        }

        /// <summary>
        /// Invoked by <see cref="SetState"/>, allows a non-owner server to update the transform state
        /// </summary>
        /// <remarks>
        /// Continued support for client-driven server authority model
        /// </remarks>
        [ClientRpc]
        private void SetStateClientRpc(Vector3 pos, byte rotY, bool shouldTeleport, ClientRpcParams clientRpcParams = default)
        {
            // Server dictated state is always applied
            transform.position = pos;
            transform.rotation = Quaternion.Euler(0, rotY, 0);
            m_LocalAuthoritativeNetworkState.IsTeleportingNextFrame = shouldTeleport;
            TryCommitTransform(transform, m_CachedNetworkManager.LocalTime.Time);
        }

        /// <summary>
        /// Will update the authoritative transform state if any deltas are detected.
        /// This will also reset the m_LocalAuthoritativeNetworkState if it is still dirty
        /// but the replicated network state is not.
        /// </summary>
        /// <param name="transformSource">transform to be updated</param>
        private void UpdateAuthoritativeState(Transform transformSource)
        {
            // If our replicated state is not dirty and our local authority state is dirty, clear it.
            if (!ReplicatedNetworkState.IsDirty() && m_LocalAuthoritativeNetworkState.IsDirty)
            {
                m_LastSentState = m_LocalAuthoritativeNetworkState;
                // Now clear our bitset and prepare for next network tick state update
                m_LocalAuthoritativeNetworkState.ClearBitSetForNextTick();
            }

            TryCommitTransform(transformSource, m_CachedNetworkManager.LocalTime.Time);
        }


        // todo this is currently in update, to be able to catch any transform changes. A FixedUpdate mode could be added to be less intense, but it'd be
        // conditional to users only making transform update changes in FixedUpdate.
        protected virtual void Update()
        {

            if (!IsSpawned)
            {
                return;
            }

            // If we are authority, update the authoritative state
            if (CanCommitToTransform)
            {
                UpdateAuthoritativeState(transform);
            }
            else // Non-Authority
            {
                if (Interpolate)
                {
                    var serverTime = NetworkManager.ServerTime;
                    var cachedDeltaTime = Time.deltaTime;
                    var cachedServerTime = serverTime.Time;
                    var cachedRenderTime = serverTime.TimeTicksAgo(1).Time;
                    foreach (var interpolator in m_AllFloatInterpolators)
                    {
                        interpolator.Update(cachedDeltaTime, cachedRenderTime, cachedServerTime);
                    }

                }

                // Apply the current authoritative state
                ApplyAuthoritativeState();
            }
        }

        /// <summary>
        /// Teleport the transform to the given values without interpolating
        /// </summary>
        /// <param name="newPosition"></param> new position to move to.
        /// <param name="newRotation"></param> new rotation to rotate to.
        /// <param name="newScale">new scale to scale to.</param>
        /// <exception cref="Exception"></exception>
        public void Teleport(Vector3 newPosition, byte newRotationY)
        {
            if (!CanCommitToTransform)
            {
                throw new Exception("Teleporting on non-authoritative side is not allowed!");
            }

            // Teleporting now is as simple as setting the internal state and passing the teleport flag
            SetStateInternal(newPosition, newRotationY, true);
        }

        /// <summary>
        /// Override this method and return false to switch to owner authoritative mode
        /// </summary>
        protected virtual bool OnIsServerAuthoritative()
        {
            return true;
        }

        /// <summary>
        /// Used by <see cref="NetworkRigidbody"/> to determines if this is server or owner authoritative.
        /// </summary>
        internal bool IsServerAuthoritative()
        {
            return OnIsServerAuthoritative();
        }
    }
}
