// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using CodeMonkey.Utils;

// public class DeepDLA : MonoBehaviour
// {
//     // public static event EventHandler OnBallHitTarget;
//     public static DeepDLA Create(Vector3 positon , Vector3 direction,CharacterTypeEnum creator ){
//         Transform deepDLATransform =  Instantiate(GameAsset.instance.pfDeepDLA,positon , Quaternion.identity);

//         deepDLATransform.eulerAngles = new Vector3(0,0,UtilsClass.GetAngleFromVectorFloat(direction));
//         DeepDLA deepDLA = deepDLATransform.GetComponent<DeepDLA>();
//         deepDLA.Setup(direction,creator );
//         return deepDLA;
//     }

//     float DLATime = 0f;
//     private float SPEED = 3f;
//     private const float Distance_travelMAX = 30f;
//     private float distanceTravalled;
//     private Vector3 dir;
//     [SerializeField] AnimationCurve curve;

//     SpriteRenderer spriteRenderer;
//     // Animator anim;
//     CharacterTypeEnum creator;

//     private void Awake() {
//         // Setup(new Vector3(-1 ,0,0));
//         // anim = GetComponent<Animator>();
//     }

//     private void Setup(Vector3 dir,CharacterTypeEnum creator ){

//         this.dir = dir; 
//         this.creator = creator;
        
//     }
//     private void Update() {
//         // SPEED += SPEED * Time.deltaTime;
//         // transform.position += dir*SPEED*Time.deltaTime;
//         // distanceTravalled += SPEED*Time.deltaTime;
//         DLATime += Time.deltaTime;
//         SPEED = curve.Evaluate(DLATime);
//         transform.position += dir*SPEED*Time.deltaTime;
//         distanceTravalled += SPEED*Time.deltaTime;
        
//         if (distanceTravalled > Distance_travelMAX){
//             // Ball travlled too much , destroy this
//             Destroy(gameObject);
//         }
//     }
//     private void OnTriggerEnter(Collider collider) {
//         IHurtBox damageable = collider.GetComponent<IHurtBox>();
//         // if (damageable != null){
//         //     if (creator != damageable.WhoReciveHP()){
//         //         Debug.Log("Damage + "+ damageable );
//         //     }
//         //     // damageable.ReceiveHP(10);
//         // }  
//         Debug.Log("hit");
//     }
// }


