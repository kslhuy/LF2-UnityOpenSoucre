using System.Collections;
using UnityEngine;

namespace LF2{

[CreateAssetMenu(menuName = "GameData/ProjectilDetails",  fileName = "projectilDetails" )]
public class ProjectilDetails : ScriptableObject{


    [Tooltip("The name of a node in the Animator's state machine.")]
    public string m_AnimationName;

    [Header("--Effect to the enemy--")]

    public int damageAmount; // INJURY
    public DamageEffect Effect;


    // The fall-value determines how an attacked character will react to this itr by cringing,
    // getting into the "Dance of Pain", or falling. 
    // If no value is specified, the default of 20 will be used. 
    // If a character accumulates up to 20 fall-points, he will go to injured1 (220). 
    // If a character accumulates up to 40 fall-points, he will go to injured2 (222) or injured2back (224) depending on the direction he was hit and will fall if he was in mid-air. 
    // If a character accumulates up to 60 fall-points, he will go into the "Dance of Pain"(226) where he can be grabbed or hit by super punch. 
    // Additionally, attacks with more than fall: 40 can hit falling characters.
    //Here are a few values as a rule of thumb for various fall-values:
    	// -1 - does not go into injured frames and become harder to knockdown.
        //  1 - never stun, never fall (ex: Davis DvA shrafe)
        //  20 - 3 hit stun, 4 hit fall
		// 	25 - 2 hit stun, 3 hit fall (ex: Dennis normal kick)
        //  40 - does not stun, 2 hit fall (ex: baseball bat normal swing)
		// 	60 - 1 hit stun, 2 hit fall, can hit falling (ex: Henry's arrow)
		// 	70 - 1 hit fall, can hit falling

    public int fall = 20; 

//     Bdefend-points determine if a character is able to block an attack by defending or if he will go to the broken-defense-frames. 
// As long as he has 30 or less Bdefend-points, he will be able to block the attack, if it's 31 or higher, he goes to the broken-defense-frames. 
// If an itr hits the character while he is not in the defend-frames, his bdefend-counter will automatically increase to 45. 
// If you have, for example, accumulated 31 points and get hit during your defense (assuming you have specified a positive bdefend-value in the hitting itr), the character will go to the broken-defense-frames.
// Here are some common values for various bdefend-values:
// 			0 - never breaks defense (ex: John's D>J shield)
//                         12 - 4 hit break
//                         16 - 3 hit break
//                         30 - 2 hit break
// 			60 - 1 hit break
//                         100 - ignores defense, sets bdefend-counter to 45 and instantly destroys weapons.
    
    public int Bdefend = 20; 


// 	If you want to hit a single enemy, use arest. 
// For multiple enemies, you need vrest. The values of these tags are the duration in TU that the object must wait before hitting the character again.

    public bool arest;
    public int vrest ;

    [Tooltip("How far the enemy falls backwards , upwards when hit")]
    public Vector3 Dirxyz;

    public float durration;

    
    [HideInInspector]
    public int AnimationNameHash; // this is maintained via OnValidate() in the editor



}

}