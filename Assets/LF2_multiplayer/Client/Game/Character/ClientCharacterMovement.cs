
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
        private Vector3 vector3;

        public int FacingDirection{get ; private set;}


        private NetStatePackage predictPackage;
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

         [SerializeField]  private float _jumpVelocityFalloff = 1;
         [SerializeField] private float _fallMultiplier = 10f ;


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

            CanCommitToTransform =  IsOwner ;

            m_MovementSource.NetworkRotY.OnValueChanged += SyncRotationY;

        }

        private void SyncRotationY(short previousValue, short newValue)
        {
            transform.rotation = Quaternion.Euler(0,newValue,0);   
        }




        #region Set function Logics  for MoveMent 

        public void SetXZ(float dirX , float dirZ){
  
            vector3.Set(dirX*m_walking_speed , 0 ,0.8f*dirZ * m_walking_speedz);

            CheckIfShouldFlip(Mathf.RoundToInt(dirX));       
            // AddBufferPredictState()
            // Debug.Log(vector3);
            // var newTransform = transform.position + Time.deltaTime*IsBorder(vector3);
            // newTransform.x = Mathf.RoundToInt(newTransform.x);
            // newTransform.y = Mathf.RoundToInt(newTransform.y);
            // newTransform.z = Mathf.RoundToInt(newTransform.z);
            transform.position = transform.position + Time.deltaTime*IsBorder(vector3);
            // m_Rigidbody.MovePosition(transform.position + Time.deltaTime*IsBorder(vector3));  

        }

        public void SyncRigidVSTransform(){
            
            m_Rigidbody.position = transform.position;

        }
        /// Sets Jump
        public void SetJump(float inputX , float inputZ ){
            vector3.Set(inputX , 0 ,0.75f*inputZ);
            vector3 = IsBorder(vector3);
            m_Rigidbody.velocity = jumpHieght*Vector3.up + jumpDistance*vector3;

        }

        public void CustomJump(float jumpHieght , float jumpDistance,float inputX = 0 , float inputZ = 0  ){
            vector3.Set(inputX , 0 ,0.75f*inputZ);
            vector3 = IsBorder(vector3);

            if (inputX != 0 || inputZ != 0 ){
                m_Rigidbody.velocity = jumpHieght*Vector3.up + jumpDistance*vector3;
            }else m_Rigidbody.velocity = jumpHieght*Vector3.up + jumpDistance*new Vector3(FacingDirection,0,0);
        }

        public void CustomMove( float speedx ,float speedz = 0 , float inputX = 0 , float inputZ = 0  ){
            moveDir = IsBorder(new Vector3(FacingDirection*speedx , 0 ,inputZ*speedz));
            transform.position +=  Time.deltaTime*moveDir;

            // m_Rigidbody.MovePosition(transform.position + Time.deltaTime*moveDir);  
        }
        public void CustomMove_InputX( float speedx ,float speedz = 0 , float inputZ = 0  ){
            moveDir = IsBorder(new Vector3(FacingDirection*speedx , 0 ,inputZ*speedz));
            // m_Rigidbody.MovePosition(transform.position + Time.deltaTime*moveDir);  
            transform.position +=  Time.deltaTime*moveDir;

        }
        public void CustomMove_InputZ( float speedx ,float speedz = 0 , float inputZ = 0  ){
            moveDir = IsBorder(new Vector3(FacingDirection*speedx , 0 ,0.75f*inputZ*speedz));
            // m_Rigidbody.MovePosition(transform.position + Time.deltaTime*moveDir);  
            transform.position +=  Time.deltaTime*moveDir;
        }
        public void CustomMove_InputXZ( float speedx ,float speedz = 0 , float inputZ = 0  ){
            moveDir = IsBorder(new Vector3(FacingDirection*speedx , 0 ,inputZ*speedz));
            // m_Rigidbody.MovePosition(transform.position + Time.deltaTime*moveDir);
            transform.position +=  Time.deltaTime*moveDir;
  
        }

        public void SetDoubleJump(float inputX , float inputZ){
            vector3.Set(inputX , 0 ,0.75f*inputZ);
            vector3 = IsBorder(vector3);

            if (inputX != 0 || inputZ != 0 ){
                m_Rigidbody.velocity = doubleJumpHieght*Vector3.up + doubleJumpDistance*vector3;
            }else m_Rigidbody.velocity = doubleJumpHieght*Vector3.up + doubleJumpDistance*new Vector3(FacingDirection,0,0);
        }

        public void SetPlayerInAir(float x , float y){
            vector3.Set(FacingDirection*x,y,0);
            // m_Rigidbody.velocity = vector3 ;
        }




        public void SetFallingDown(int coeficientFall = 0){
            
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
            int number = Physics.RaycastNonAlloc(m_BoxCollider.bounds.center,Vector3.down ,GroundHit,m_BoxCollider.bounds.extents.y+0.1f,k_GroundLayerMask);
            
            // Color rayColor;
            // if (!hit_ground){
            //     rayColor = Color.green;
            // }else {
            //     rayColor = Color.red;
            // }
            // Debug.DrawRay(m_BoxCollider.bounds.center , Vector3.down * (m_BoxCollider.bounds.extents.y),rayColor);
            if (number != 1 ) return false;
        
            m_Rigidbody.velocity = Vector3.zero;
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
            Debug.DrawRay(m_BoxCollider.bounds.center , Vector3.down * (m_BoxCollider.bounds.extents.y+extraYCheckGround),rayColor);
            if (_isGround )   m_Rigidbody.velocity = Vector3.zero;
 
            return  _isGround;
        }   

        public void SetHurtMovement(Vector3 dir , int attakerFacing){
            Debug.Log(dir.x*attakerFacing);
            
            m_Rigidbody.velocity =  new Vector3(dir.x*attakerFacing,dir.y);

            // m_Rigidbody.AddForce(new Vector3(dir.x*attakerFacing,dir.y),ForceMode.Impulse);
              

        }



        public void SetRun(float inputZ){
            moveDir = IsBorder(new Vector3(FacingDirection* m_speedRun,0,0.8f*inputZ));
            // Rigidbody.velocity = moveDir;
            // m_Rigidbody.MovePosition(transform.position + Time.deltaTime*(moveDir));  
            transform.position +=  Time.deltaTime*moveDir;

        }
        public void SetRoll(float speed = 0){
            moveDir = IsBorder(new Vector3(FacingDirection* m_speedRoll,0,0));
            // m_Rigidbody.MovePosition(transform.position + Time.deltaTime*(moveDir));  
            transform.position += Time.deltaTime*moveDir;
        }

        public void SetSliding(float speed){

            moveDir = IsBorder(new Vector3(FacingDirection* speed,0,0));
            // Rigidbody.velocity = moveDir;
            // transform.position += Time.deltaTime*moveDir;
            // m_Rigidbody.MovePosition(transform.position + Time.deltaTime*moveDir);  
            transform.position +=  Time.deltaTime*moveDir;

            
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
            m_MovementSource.NetworkRotY.Value = (short)transform.rotation.eulerAngles.y;


        }

#endregion



        private void FixedUpdate() {
            m_Rigidbody.position = transform.position;
        }




        // IEnumerator SomthInterpolation(Vector3 target){
        //     Vector3 m_LerpStart = transform.position;
        //     float m_CurrentLerpTime = 0f;
        //     while (Vector3.Distance(transform.position , target) > 0.5){
        //         // Debug.Log(Vector3.Distance(transform.position , target));
        //         m_CurrentLerpTime += Time.deltaTime;
        //         if (m_CurrentLerpTime > m_LerpTime)
        //         {
        //             m_CurrentLerpTime = m_LerpTime;
        //         }

        //         var lerpPercentage = m_CurrentLerpTime / m_LerpTime;
        //         transform.position = Vector3.Lerp(m_LerpStart, target, lerpPercentage);
        //         yield return null; 
        //     }
        //     Debug.Log("finish");
        // }


    
    }
}



