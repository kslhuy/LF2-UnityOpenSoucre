// using System.Collections;
// using System.Collections.Generic;
// using Unity.Netcode;
// using UnityEngine;
// namespace LF2.Client{


//     public class DDJ_Henry_SonataDeath_Logic : NetworkBehaviour
//     {
//         private List<IHurtBox> _Listdamagable = new List<IHurtBox>();

//         [SerializeField] private float tickRate = 0.2f;
//         private float timeNow ;
//         private float timeStart;
//         AttackDataSend Atk_data = new AttackDataSend();
//         private bool m_Started;
//         private bool canHitCreator;
//         private ulong m_SpawnerId;
//         private SkillsDescription.ProjectileInfo m_ProjectileInfo;
//         private int m_CollisionMask;  //mask containing everything we test for while moving

//         public void Initialize(ulong creatorsNetworkObjectId, int facing,in SkillsDescription.ProjectileInfo projectileInfo){
//             canHitCreator = false;
//             m_SpawnerId = creatorsNetworkObjectId;
//             m_ProjectileInfo = projectileInfo;

//             // Atk_data.Facing = projectileInfo.ProjectileDamage[0].FacingDirection;
//             Atk_data.Direction = projectileInfo.ProjectileDamage[0].Dirxyz;

//             Atk_data.BDefense_p = projectileInfo.ProjectileDamage[0].Bdefend;
//             Atk_data.Fall_p = projectileInfo.ProjectileDamage[0].fall;      
//         }

//         public override void OnNetworkSpawn()
//         {
//             if (!IsServer)
//             {
//                 enabled = false;
//                 return;
//             }
//             m_Started = true;

//             m_CollisionMask = LayerMask.GetMask(new[] { "HurtBox" });
//         }

//         private void FixedUpdate() {

            
//             if (!m_Started) { 
//                 timeNow = Time.time; 
//                 return; 
//             } 
//             if (Time.time - timeNow >  tickRate){
//                 foreach (IHurtBox damagable in _Listdamagable){
//                     if (damagable != null && damagable.IsDamageable()) {
//                         damagable.ReceiveHP(Atk_data);
//                     }
//                 }
//             }
            
//             if (Time.time - timeStart >= m_ProjectileInfo.DestroyAfterSec){

//                 NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
//                 networkObject.Despawn();
//             }           

//         }
//         private  void OnTriggerEnter(Collider collider)
//         {
//             IHurtBox damagable = collider.GetComponentInParent<IHurtBox>();
//             if (damagable != null){
//                 _Listdamagable.Add(damagable);

//             }
//         }

//         private void OnTriggerExit(Collider collider)
//         {
//             IHurtBox damagable = collider.GetComponentInParent<IHurtBox>();
//             if (damagable != null){
//                 _Listdamagable.Remove(damagable);
//             }
//         }







        


//     }
// }
