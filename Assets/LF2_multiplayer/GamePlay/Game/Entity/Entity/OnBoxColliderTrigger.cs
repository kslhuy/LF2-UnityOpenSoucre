using System;
using UnityEngine;
namespace LF2.Client{

    public class OnBoxColliderTrigger : MonoBehaviour {
        [SerializeField] ClientCharacterVisualization _ClientCharacterVisualization;
        private void OnTriggerEnter(Collider collider) {
            if (collider.CompareTag("HurtBox")){
                _ClientCharacterVisualization.OnTriggerEnter(collider);
            }
            if (collider.CompareTag("Projectile")){
                _ClientCharacterVisualization.OnTriggerEnter(collider);
            }
        }



        private void OnTriggerExit(Collider collider) {
            if (collider.CompareTag("HurtBox")){
                _ClientCharacterVisualization?.OnTriggerExit(collider);
            }
            if (collider.CompareTag("Projectile")){
                _ClientCharacterVisualization?.OnTriggerExit(collider);
            }
        }

                // private void OnTriggerStay(Collider collider) {
        //     if (collider.CompareTag("HurtBox")){
        //         MStateMachinePlayerViz?.OnTriggerStay(collider);
        //     }
        // }

    }



}
