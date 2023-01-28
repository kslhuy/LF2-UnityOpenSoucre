using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace LF2.Client{

    
    public class Shadow2 : MonoBehaviour {
        // [SerializeField] GameObject _shadowPrefab;
        [SerializeField] float OffsetY = 64;
        [SerializeField] LayerMask ground;
        Vector3 posGround;
        [SerializeField] Transform TransformParent;
        private void Start() {
            RaycastHit raycastHit; 
            if (Physics.Raycast(gameObject.transform.position ,Vector3.down,out raycastHit,100,ground.value)){
                Debug.Log("have posGround");
                posGround = raycastHit.collider.gameObject.transform.position;
            }
        }
        private void Update() {
            transform.SetPositionAndRotation(new Vector3(TransformParent.position.x ,posGround.y + OffsetY ,TransformParent.position.z) , Quaternion.identity);  
        }


    }
}
