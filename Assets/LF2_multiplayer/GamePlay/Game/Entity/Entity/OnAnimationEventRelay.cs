using System;
using UnityEngine;
namespace LF2.Client{

    public class OnAnimationEventRelay : MonoBehaviour {
        [SerializeField] ClientCharacterVisualization clientCharacterVisualization;
        public void OnAnimEvent(int id){
            clientCharacterVisualization.OnAnimEvent(id);
        }
    }



}
