
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
using TMPro;
using System.Collections;
using System;

namespace LF2.Client{

    public class ClientCharacterMovement : NetworkBehaviour {

#region Declare_Field
    
        [SerializeField]
        Rigidbody m_Rigidbody;  
        public Rigidbody RB => m_Rigidbody  ;

        [SerializeField]
        BoxCollider m_BoxCollider; 

        [SerializeField]
        private NetworkCharacterState m_MovementSource;

        private Vector3 moveDir;

        public int FacingDirection{get ; private set;}

        private RaycastHit[] GroundHit = new RaycastHit[1];

#endregion Declare_Field

    




        private Transform m_Transform; // cache the transform component to reduce unnecessary bounce between managed and native
        private NetworkManager m_CachedNetworkManager;




        private bool CanCommitToTransform ;



        // walking

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

        [SerializeField]  private float Drag = 1;
        [SerializeField]  private float _jumpVelocityFalloffQuick = 1;
        [SerializeField]  private float _jumpVelocityFalloffLimit = -200; 
        [SerializeField] private float _fallMultiplierNormal = 10f ;
        [SerializeField] private float _fallMultiplierQuick = 50f ;
        [SerializeField] private float _distanceFromGround = 50f ;

        [SerializeField] private bool UseFallMultiplierNormal = true ;

        



        float jumpHieght => m_MovementSource.CharacterClass.jump_height;
        float jumpDistance => m_MovementSource.CharacterClass.jump_distance;
        float doubleJumpHieght=> m_MovementSource.CharacterClass.doublejump_height;
        float doubleJumpDistance => m_MovementSource.CharacterClass.doublejump_distance;
        #endregion

        [Header("----- Collision Detection ------")]

        #region Collision Detection

        private int k_GroundLayerMask ;

        private int k_WallLayerMask;
        [SerializeField] private float extraYCheckGround = 0f;

        private Vector3 _boundExtents =>  m_BoxCollider.bounds.extents;
        private Vector3 _boundCenter =>  m_BoxCollider.bounds.center;




        #endregion


        private void Awake() {
            k_WallLayerMask= LayerMask.GetMask(new[] { "Wall" });
            k_GroundLayerMask = LayerMask.GetMask(new[] { "Ground" });

        }

        public override void OnNetworkSpawn()
        {
            if (!IsClient && IsHost)
            {
                //this component is not needed on the host (or dedicated server), because ServerCharacterMovement will directly
                //update the character's position.
                this.enabled = false;
            }            
            if (transform.rotation.eulerAngles.y <= 1){
                FacingDirection = 1;    
            }
            else {
                FacingDirection = -1;    
            }


            m_Transform = transform;
            m_CachedNetworkManager = NetworkManager;

            CanCommitToTransform =  IsServer ;

        }

        private void SyncRotationY(short preValue , short newValue)
        {
            transform.rotation = Quaternion.Euler(0,newValue,0);   
        }




        #region Set function Logics  for MoveMent 

        public void SetXZ(float dirX , float dirZ){
  
            moveDir.Set(dirX*m_walking_speed , 0 ,0.7f*dirZ * m_walking_speedz);
            // moveDir = IsBorder(moveDir);

            CheckIfShouldFlip(Mathf.RoundToInt(dirX));       
            // AddBufferPredictState()
            // Debug.Log(moveDir);
            // var newTransform = transform.position + Time.deltaTime*IsBorder(moveDir);
            // newTransform.x = Mathf.RoundToInt(newTransform.x);
            // newTransform.y = Mathf.RoundToInt(newTransform.y);
            // newTransform.z = Mathf.RoundToInt(newTransform.z);
            // Debug.Log(IsBorder(moveDir)); 
            // transform.position = transform.position + Time.deltaTime*moveDir;

            m_Rigidbody.velocity = moveDir; 



        }

        /// Sets Jump
        public void SetJump(float inputX , float inputZ ){
            moveDir.Set(inputX , 0 ,0.75f*inputZ);
            // moveDir = IsBorder(moveDir);
            m_Rigidbody.velocity = jumpHieght*Vector3.up + jumpDistance*moveDir;



        }

        public void CustomJump(float jumpHieght , float jumpDistance,float inputX = 0 , float inputZ = 0  ){
            moveDir.Set(inputX , 0 ,0.7f*inputZ);
            // moveDir = IsBorder(moveDir);

            if (inputX != 0 || inputZ != 0 ){
                m_Rigidbody.velocity = jumpHieght*Vector3.up + jumpDistance*moveDir;
            }else m_Rigidbody.velocity = jumpHieght*Vector3.up + jumpDistance*new Vector3(FacingDirection,0,0);
        }

        public void CustomMove( float speedx ,float speedz = 0 , float inputX = 0 , float inputZ = 0  ){
            moveDir = IsBorder(new Vector3(FacingDirection*speedx , 0 ,inputZ*speedz));
            // transform.position +=  Time.deltaTime*moveDir;
            m_Rigidbody.velocity = moveDir;

            // m_Rigidbody.MovePosition(transform.position + Time.deltaTime*moveDir);  
        }
        public void CustomMove_InputX( float speedx ,float speedz = 0 ){
            moveDir = IsBorder(new Vector3(FacingDirection*speedx , 0 ,0));
            m_Rigidbody.velocity =  moveDir;  
            // transform.position +=  Time.deltaTime*moveDir;

        }
        public void CustomMove_InputZ( float speedx ,float speedz = 0 , float inputZ = 0  ){
            moveDir = IsBorder(new Vector3(FacingDirection*speedx , 0 ,0.75f*inputZ*speedz));
            m_Rigidbody.velocity =  moveDir;  
            // transform.position +=  Time.deltaTime*moveDir;
        }
        
        

        public void CustomMove_InputXZ( float speedx ,float speedz = 0 , float InputX = 0, float inputZ = 0  ){
            moveDir = IsBorder(new Vector3(FacingDirection*speedx , 0 ,inputZ*speedz));
            m_Rigidbody.velocity =  moveDir;
            // transform.position +=  Time.deltaTime*moveDir;
        }

        public void CustomMove_InputXYZ( float speedx ,float speedz = 0 , float InputX = 0, float inputZ = 0  ){
            moveDir = IsBorder(new Vector3(FacingDirection*speedx , 0 ,inputZ*speedz));
            m_Rigidbody.velocity =  moveDir;
            // transform.position +=  Time.deltaTime*moveDir;
        }

        public void TeleportPlayer(Vector3 position){
            transform.position =  position ;
        }

        public void SetDoubleJump(float inputX , float inputZ){
            moveDir.Set(inputX , 0 ,0.7f*inputZ);
            // moveDir = IsBorder(moveDir);

            if (inputX != 0 || inputZ != 0 ){
                m_Rigidbody.velocity = doubleJumpHieght*Vector3.up + doubleJumpDistance*moveDir;
            }else m_Rigidbody.velocity = doubleJumpHieght*Vector3.up + doubleJumpDistance*new Vector3(FacingDirection,0,0);
        }

        public void SetPlayerInAir(float x , float z){
            CheckIfShouldFlip(Mathf.RoundToInt(x));       
            m_Rigidbody.velocity += Time.deltaTime*new Vector3(0.1f*x,0,0.1f*z) ;
        }

        public int GetFacingDirection(){
            return (int)m_Transform.right.x;
        }
        public void TurnONGravity(bool active){
            m_Rigidbody.useGravity = active;
        }




        // public void SetFallingDown(int coeficientFall = 0){
        //     if (m_Rigidbody.velocity.y < _jumpVelocityFalloffQuick  ){
        //         m_Rigidbody.velocity += _fallMultiplierQuick * Physics.gravity.y * Time.deltaTime * Vector3.up ;
        //         if (m_Rigidbody.velocity.y < _jumpVelocityFalloffLimit){
        //             m_Rigidbody.velocity = new Vector3(0,_jumpVelocityFalloffLimit ,0);
        //         }
        //         if (CheckGoundedClose(_distanceFromGround)) m_Rigidbody.velocity += _fallMultiplierNormal * Physics.gravity.y * Vector3.up * Time.deltaTime ; 

        //         return;
        //     }
        //     if (UseFallMultiplierNormal) m_Rigidbody.velocity += _fallMultiplierNormal * Physics.gravity.y * Time.deltaTime * Vector3.up ;

        //     // if (m_Rigidbody.velocity.y < -200) m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x , -150 ,m_Rigidbody.velocity.z);  
        // }
        // private bool _colUp, _colRight, _colDown, _colLeft;


        public void SetFallingDown(){
            
            if (m_Rigidbody.velocity.y < _jumpVelocityFalloffQuick || m_Rigidbody.velocity.y > 0 ){
                if (CheckGoundedClose(_distanceFromGround) && m_Rigidbody.velocity.y < _jumpVelocityFalloffLimit) {
                    // Debug.Log("Descrea velocity");
                    m_Rigidbody.velocity += _fallMultiplierNormal * Vector3.up * Time.deltaTime ;
                    m_Rigidbody.drag = Drag;
                    return;
                } 
                m_Rigidbody.velocity += _fallMultiplierQuick * Physics.gravity.y * Time.deltaTime * Vector3.up ;  
            }
        }
        public Vector3 IsBorder(Vector3 dir){
            // Wall detection x , z
            
            
            bool _colRight = Physics.Raycast(_boundCenter,Vector3.right ,_boundExtents.y + 2,k_WallLayerMask);
            bool _colLeft = Physics.Raycast(_boundCenter,Vector3.left ,_boundExtents.y +  2,k_WallLayerMask);
            
            
            bool  _colforward = Physics.Raycast(_boundCenter,Vector3.forward ,_boundExtents.y + 2,k_WallLayerMask);
            bool  _colBack = Physics.Raycast(_boundCenter,Vector3.back ,_boundExtents.y + 2 ,k_WallLayerMask);
            

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
            int number = Physics.RaycastNonAlloc(m_BoxCollider.bounds.center,Vector3.down ,GroundHit,m_BoxCollider.bounds.extents.y+0.1f,k_GroundLayerMask);
            
            // Color rayColor;
            // if (!hit_ground){
            //     rayColor = Color.green;
            // }else {
            //     rayColor = Color.red;
            // }
            // Debug.DrawRay(m_BoxCollider.bounds.center , Vector3.down * (m_BoxCollider.bounds.extents.y),rayColor);
            if (number == 0 ) return false;
        
            m_Rigidbody.velocity = Vector3.zero;
            m_Rigidbody.drag = 0;

            // transform.position = new Vector3(transform.position.x,GroundHit[0].transform.position.y+70, transform.position.z);

            return true;
        
        }    

        public bool CheckGoundedClose(float distance){
            // var _isGround = Physics.OverlapSphereNonAlloc(transform.position + new Vector3(0, _grounderOffset), _grounderRadius, _ground, k_GroundLayerMask) > 0;
            int number = Physics.RaycastNonAlloc(m_BoxCollider.bounds.center,Vector3.down ,GroundHit,m_BoxCollider.bounds.extents.y+distance,k_GroundLayerMask);
            
            // Color rayColor;
            // if (!hit_ground){
            //     rayColor = Color.green;
            // }else {
            //     rayColor = Color.red;
            // }
            // Debug.DrawRay(m_BoxCollider.bounds.center , Vector3.down * (m_BoxCollider.bounds.extents.y),rayColor);
            if (number == 0 ) return false;
        
            // m_Rigidbody.velocity = Vector3.down;
            // transform.position = new Vector3(transform.position.x,GroundHit[0].transform.position.y+70, transform.position.z);

            return true;
        
        }  
        public bool IsGoundedNotOwner(){
            // var _isGround = Physics.OverlapSphereNonAlloc(transform.position + new Vector3(0, _grounderOffset), _grounderRadius, _ground, k_GroundLayerMask) > 0;

            bool _isGround = Physics.Raycast(m_BoxCollider.bounds.center,Vector3.down ,m_BoxCollider.bounds.extents.y+extraYCheckGround,k_GroundLayerMask);
            Color rayColor;
            if (!_isGround){
                rayColor = Color.green;
            }else {
                rayColor = Color.red;
            }
            // Debug.DrawRay(m_BoxCollider.bounds.center , Vector3.down * (m_BoxCollider.bounds.extents.y+extraYCheckGround),rayColor);
            if (_isGround )   m_Rigidbody.velocity = Vector3.zero;
 
            return  _isGround;
        }   

        public void SetHurtMovement(Vector3 dir ){
            m_Rigidbody.velocity =  dir;
            // transform.position += dir;

            // m_Rigidbody.AddForce(dir,ForceMode.VelocityChange);
              

        }



        public void SetRun(float inputZ){
            moveDir = new Vector3(FacingDirection* m_speedRun,0,0.8f*inputZ);
            m_Rigidbody.velocity = moveDir;
            // m_Rigidbody.velocity +=  Time.deltaTime*moveDir;
            // transform.position +=  Time.deltaTime*moveDir;

        }
        public void SetRoll(float speed = 0){
            moveDir = IsBorder(new Vector3(FacingDirection* m_speedRoll,0,0));
            // m_Rigidbody.MovePosition(transform.position + Time.deltaTime*(moveDir));  
            // transform.position += Time.deltaTime*moveDir;
            m_Rigidbody.velocity +=  Time.deltaTime*moveDir;

        }

        public void SetSliding(float speed){

            moveDir = IsBorder(new Vector3(FacingDirection* speed,0,0));
            m_Rigidbody.velocity +=  Time.deltaTime*moveDir;
            // transform.position += Time.deltaTime*moveDir;
            // m_Rigidbody.MovePosition(transform.position + Time.deltaTime*moveDir);  
            // transform.position +=  Time.deltaTime*moveDir;

            
        }
        public void InstanceVelocity(float velX = 0, float velY = 0 , float velZ = 0){
            moveDir = IsBorder(new Vector3(FacingDirection* velX,velY,velZ));
            m_Rigidbody.velocity = moveDir;

        }



        public bool CheckIfShouldFlip(int xInput , bool wantflip = false){
            if (xInput != 0 && xInput != FacingDirection || wantflip){
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



        private void FixedUpdate() {
            m_Rigidbody.position = transform.position;
        }


    
    }
}



