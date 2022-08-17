using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace LF2.Client{

    
    public class Shadow : NetworkBehaviour {
        [SerializeField] BoxCollider m_boxCollider;
        [SerializeField] PhysicsWrapper physicsWrapper;
        private static RaycastHit[] _GroundHit = new RaycastHit[1];


        // Use this for initialization
        private int k_GroundLayerMask;
        [SerializeField]
        private float PivotGround;

        private Transform Ground = default;


        public override void OnNetworkSpawn(){
            k_GroundLayerMask =  LayerMask.GetMask(new[] { "Ground" });
            Ground = Gounded();
        }

        // Update is called once per frameq
        void LateUpdate() {
            if (Ground == null){
                Ground = Gounded();
                Debug.Log(Ground.position);
                return;
            }
            transform.position  = new Vector3(physicsWrapper.Transform.position.x,Ground.position.y+PivotGround, physicsWrapper.Transform.position.z);
        }

        public Transform Gounded(){
            int number = Physics.RaycastNonAlloc(m_boxCollider.bounds.center,Vector3.down ,_GroundHit,500,k_GroundLayerMask);
            Color rayColor;
            if (number != 1){
                rayColor = Color.green;
            }else {
                rayColor = Color.red;
            }
            Debug.DrawRay(m_boxCollider.bounds.center , Vector3.down * (transform.position.x+500),rayColor);
            return  _GroundHit[0].transform;

        }    

    }
}
