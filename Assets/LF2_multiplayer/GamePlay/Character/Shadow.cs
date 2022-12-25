using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace LF2.Client{

    
    public class Shadow : MonoBehaviour {
        [SerializeField] GameObject _shadowPrefab;
        [SerializeField] Vector3 Offset;
        Transform shadowTransform;
        private void Start() {
            shadowTransform = Instantiate(_shadowPrefab , transform).transform ;
        }
        private void Update() {
            shadowTransform.localPosition = Offset; 
            // new Vector3(transform.position.x + Offset.x , Offset.y, transform.position.z + Offset.z );
        }


    }
}
