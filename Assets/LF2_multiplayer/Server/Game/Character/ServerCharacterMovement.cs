
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;
using LF2;
using TMPro;

namespace LF2.Server
{
    


    /// <summary>
    /// Component responsible for moving a character on the server side based on inputs.
    /// </summary>
    // [RequireComponent(typeof(NetworkCharacterState), typeof(ServerCharacter)), RequireComponent(typeof(Rigidbody))]
    public class ServerCharacterMovement : NetworkBehaviour
    {

        

        [SerializeField] 
        BoxCollider m_BoxCollider;

        [SerializeField]
        Rigidbody m_Rigidbody;

        [SerializeField]
        private NetworkCharacterState m_MovementSource;



        protected NetworkManager m_CachedNetworkManager;





        private Vector3 moveDir;
        private Vector3 vector3;

        public int FacingDirection { get; private set; }
        #region Debug
            
        public bool DebugPlayer { get; private set; }


 


        [SerializeField] TextMeshPro localtime ;
        [SerializeField] TextMeshPro servertime ;
        [SerializeField] TextMeshPro localTick ;
        [SerializeField] TextMeshPro serverTick ;
        #endregion



        public float X_extra , Z_extra;
        private Vector3 size;
        private float currentTime;


        private bool CanCommitToTransform ;



        private float m_walking_speed => m_MovementSource.CharacterClass.walking_speed;
        private float m_walking_speedz => m_MovementSource.CharacterClass.walking_speedz;


        private float m_speedRun => m_MovementSource.CharacterClass.running_speed;    
        private float m_speedRunz => m_MovementSource.CharacterClass.running_speed_z;

        // Sliding 


     

        // Rolling 

        private float m_speedRoll => m_MovementSource.CharacterClass.rolling_speed;    



        [Header("----- JUMP ------")]
        #region Jump

        
        // 

         [SerializeField]  private float _jumpVelocityFalloff = 1;
         [SerializeField] private float _fallMultiplier = 10f ;


        float jumpHieght => m_MovementSource.CharacterClass.jump_height;
        float jumpDistance => m_MovementSource.CharacterClass.jump_distance;
        float doubleJumpHieght=> m_MovementSource.CharacterClass.doublejump_height;
        float doubleJumpDistance => m_MovementSource.CharacterClass.jump_distance;
        #endregion

        [Header("----- Collision Detection ------")]

        #region Collision Detection
    
        // [SerializeField]  float _grounderRadius = 0.15f;
        // [SerializeField]  float _grounderOffset = 0.12f;
        // private readonly Collider[] _ground = new Collider[1];

        private int k_GroundLayerMask ;

        private int k_WallLayerMask;
        private const float m_LerpTime = 0.1f;

        private Vector3 _boundExtents =>  m_BoxCollider.bounds.extents;
        private Vector3 _boundCenter =>  m_BoxCollider.bounds.center;



        #endregion





        private void Awake()
        {
            FacingDirection = 1;

        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                // Disable server component on clients
                enabled = false;
                return;
            }
            currentTime = Time.time;


            CanCommitToTransform = IsOwner;
            m_CachedNetworkManager = NetworkManager;
            // m_NetworkCharacterState.InitNetworkPositionAndRotationY(transform.position ,(int)transform.rotation.eulerAngles.y);
            k_GroundLayerMask = LayerMask.GetMask(new[] { "Ground" });


        }






#region Set function Logics  for MoveMent 
            
        public void SetXZ(float dirX , float dirZ){
  
            vector3.Set(dirX*m_walking_speed , 0 ,dirZ * m_walking_speedz);

            CheckIfShouldFlip(Mathf.RoundToInt(dirX));       
            // AddBufferPredictState()
            // Debug.Log(targetposition);
            // var newTransform = transform.position + Time.deltaTime*IsBorder(vector3);
            // newTransform.x = Mathf.RoundToInt(newTransform.x);
            // newTransform.y = Mathf.RoundToInt(newTransform.y);
            // newTransform.z = Mathf.RoundToInt(newTransform.z);
            transform.position = transform.position + Time.deltaTime*IsBorder(vector3); ;
    


        }


        /// Sets Jump
        public void SetJump(float inputX , float inputZ ){
            // vector3.Set(inputX , 0 ,inputZ);
            // Debug.Log($"SetJump = {inputX}");
            // Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
            m_Rigidbody.velocity = jumpHieght*Vector3.up + jumpDistance*new Vector3(inputX ,0, inputZ);

        }

        public void SetDoubleJump(float inputX , float inputZ){
            vector3.Set(inputX , 0 ,inputZ);
            if (inputX != 0 || inputZ != 0 ){
                m_Rigidbody.velocity = doubleJumpHieght*Vector3.up + doubleJumpDistance*vector3;
            }else m_Rigidbody.velocity = doubleJumpHieght*Vector3.up + doubleJumpDistance*new Vector3(FacingDirection,0,0);
        }

        public void SetPlayerInAir(float x , float y){
            vector3.Set(FacingDirection*x,y,0);
            // m_Rigidbody.velocity = vector3 ;
        }




        public void SetFallingDown(){
            
            if (m_Rigidbody.velocity.y < _jumpVelocityFalloff || m_Rigidbody.velocity.y > 0 )
                m_Rigidbody.velocity += _fallMultiplier * Physics.gravity.y * Time.deltaTime * Vector3.up ;  
        }
        // private bool _colUp, _colRight, _colDown, _colLeft;


        public Vector3 IsBorder(Vector3 dir){
            // Wall detection x , z
            
            
            bool _colRight = Physics.Raycast(_boundCenter,Vector3.right ,_boundExtents.y,k_WallLayerMask);
            bool _colLeft = Physics.Raycast(_boundCenter,Vector3.left ,_boundExtents.y,k_WallLayerMask);
            
            
            bool  _colforward = Physics.Raycast(_boundCenter,Vector3.forward ,_boundExtents.y,k_WallLayerMask);
            bool  _colBack = Physics.Raycast(_boundCenter,Vector3.back ,_boundExtents.y,k_WallLayerMask);
            

            if ((_colRight && dir.x > 0 ) || (_colLeft && dir.x < 0 )) {
                return new Vector3(0 , 0 ,dir.z);
            }
            
            
            if ((_colforward && dir.z > 0 )||( _colBack && dir.z < 0 ) ) {
                return new Vector3(dir.x ,0,0 );
            }

            return dir;
            


        }


        public bool IsGounded(){
            // var _isGround = Physics.OverlapSphereNonAlloc(transform.position + new Vector3(0, _grounderOffset), _grounderRadius, _ground, k_GroundLayerMask) > 0;

            bool _isGround = Physics.Raycast(m_BoxCollider.bounds.center,Vector3.down ,m_BoxCollider.bounds.extents.y+0.1f,k_GroundLayerMask);
            // Color rayColor;
            // if (!hit_ground){
            //     rayColor = Color.green;
            // }else {
            //     rayColor = Color.red;
            // }
            // Debug.DrawRay(m_BoxCollider.bounds.center , Vector3.down * (m_BoxCollider.bounds.extents.y),rayColor);
            if (_isGround )   m_Rigidbody.velocity = Vector3.zero;
 
            return  _isGround;
        }    

        public void SetHurtMovement(Vector3 dir , int enyFacing){
            m_Rigidbody.velocity += Vector3.Scale(dir , (Vector3.right*enyFacing)) ;
              

        }



        public void SetRun(float inputZ){
            moveDir = IsBorder(new Vector3(FacingDirection* m_speedRun,0,inputZ));
            // Rigidbody.velocity = moveDir;
            transform.position += Time.deltaTime*moveDir;
        }
        public void SetRoll(){
            moveDir = IsBorder(new Vector3(FacingDirection* m_speedRoll,0,0));
            // Rigidbody.velocity = moveDir;
            transform.position += Time.deltaTime*moveDir;
        }

        public void SetSliding(float speed){

            moveDir = IsBorder(new Vector3(FacingDirection* speed,0,0));
            // Rigidbody.velocity = moveDir;
            transform.position += Time.deltaTime*moveDir;
            
        }



        public bool CheckIfShouldFlip(int xInput , bool wantflipp = true){
            if (xInput != 0 && xInput != FacingDirection && wantflipp){
                Flip();
            }
            return xInput != 0 && xInput != FacingDirection;
        }

        public void Flip(){
            // Debug.Log($"FacingDirection = {tran}");

            FacingDirection *=-1;
            transform.Rotate(0,180,0) ;

        }

#endregion





        private void Update() {
            // float timer = Time.time ;
            // if (Time.time - currentTime > 2 ){
            //     currentTime = Time.time;
            //     TimerCoru();

            // }
        }



        // private void OnDrawGizmos() {
        //     Gizmos.DrawWireCube(m_BoxCollider.center+new Vector3(distance,0,0), m_BoxCollider.size/2);
        // }
    }
}
