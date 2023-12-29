using UnityEngine;
using LF2.Client;

public class InteractionZone : MonoBehaviour {
    [SerializeField] ClientCharacterVisualization clientCharacterVisualization;
    public bool TriggerAttack3 {get ; private set;}    

    [SerializeField] private BoxCollider Pickup_Box;
    [SerializeField] private LayerMask objectLayer;

    private void OnTriggerStay(Collider collider) {
        
        if (collider.CompareTag("HurtBox")){
            // Debug.Log($"Interaction with = {collider}"); 
            IHurtBox damageable = collider.GetComponentInParent<IHurtBox>();
        
            // ClientCharacter targetClientChar = collider.GetComponentInParent<ClientCharacter>();
            if (damageable != null && damageable.NetworkObjectId != clientCharacterVisualization.NetworkObjectId){
                // Debug.Log($"Interaction with  damageble = {damageable}"); 
                // Debug.Log($"targetClientChar = {targetClientChar}");
                // Debug.Log($"targetClientChar = {clientCharacterVisualization}");
                
                if (damageable.GetStateType() == LF2.StateType.DOP ){
                    // Debug.Log($"Enemy In State = {damageable.GetStateType()}"); 
                    TriggerAttack3 = true;
                    return;
                }
                TriggerAttack3 = false;
            
            }
        }
    }


    // private void OnTriggerExit(Collider collider) {
    //     // if (clientCharacterVisualization.m_NetState.IsNpc){
    //     //     return;
    //     // }
    //     if (collider.CompareTag("HurtBox")){

    //         ClientCharacter targetClientChar =collider.GetComponentInParent<ClientCharacter>();
    //         if (targetClientChar != null){
    //             // Debug.Log($"targetClientChar = {targetClientChar}");
    //             if (targetClientChar.NetworkObjectId != clientCharacterVisualization.NetworkObjectId){
    //                 if (targetClientChar.ChildVizObject.MStateMachinePlayerViz.CurrentStateViz.GetId() != LF2.StateType.DOP ){
    //                     TriggerAttack3 = false;
    //                     return;
    //                 }
    //             }
    //         }
    //     }
    // }


    public bool Check_Pickup_Object(){
        Collider[] res = new Collider[1];
        int number = Physics.OverlapBoxNonAlloc(Pickup_Box.center , Pickup_Box.size,res , Quaternion.identity,objectLayer );
        if (number > 0)
            return true;
        else return false;
    }
    
}