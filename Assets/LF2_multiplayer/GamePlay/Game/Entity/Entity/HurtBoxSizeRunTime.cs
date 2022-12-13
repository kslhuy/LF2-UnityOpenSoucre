using System;
using UnityEngine;
namespace LF2.Client{


    public class HurtBoxSizeRunTime : MonoBehaviour {

    
        [SerializeField] private BoxCollider _boxCollider; 

        public void ChangeSizeHurtBox (Vector3 size){
            _boxCollider.size = size;
        }

    }


}
