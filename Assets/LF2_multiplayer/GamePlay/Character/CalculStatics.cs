using UnityEngine;
namespace LF2{

    public class CalculStatics {
        
        // This broken_defend check does not apply if the character is in mid-air(effectively giving you a super block).
            public int Current_BDefense {private set ; get;}
            public int Current_Fall {private set ; get;}

            // private int Current_Amor ;
            public int BaseAmor;
            public CalculStatics ( int amor ){
                // Current_Amor = 0;
            }
            // public StateType AmorLeft(int input ){
            //     Current_Amor -= input ;
                
            // }

//             1: If total bdefend <=30, the attack will be blocked successfully.
//             2: If total bdefend >30, the character is forced into broken_defend(frame 112).
//             3: This broken_defend check does not apply if the character is in mid-air(effectively giving you a super block), or if self-catch state 7 is used.
//             4: If an itr has bdefend >60, defend is ignored (see: Deep's >>A 1st hit bdef65 & bdefend 100).
//             5: bdefend 100 is a specially coded number; it instantly destroys weapons & completely ignores any form of defense including armor.
//             6: You generally cannot block an attack that is "behind" you - determined by whether the attack is facing the same direction as you or opposite from you.
            public StateType BdefenseLeft(int inputBDefense, bool opositeDir ){
                // 1 : if  amor absord the Bdenfens so , dont need to acumulated
                // 2 : else  amor can't absord the Bdenfens so , need to acumulated

                Current_BDefense += inputBDefense ;
                

                // 1 & -1 = oposite direction
                
                if (Current_BDefense <= 30  ) return StateType.DefenseHit; // 1*
                else if (Current_BDefense > 30 && Current_BDefense < 60 )return StateType.BrokenDefense; // 2*
                else {
                    if (opositeDir) return StateType.FallingFront;//4*
                    else return StateType.FallingBack;
                }
            }

//             There are 4 injury frames, and 6 categories of fall values:
//              1* :fall <0 (negative) = no injury frames
//              2* :fall 0 is the same as fall 20.
//              3* :fall 1-20 = injury1(frame 220). This injury can happen in mid-air.
//              4* :fall 21-40, front = injury2(frame 222). You fall instead if you get hit in mid-air.
//              5* :fall 21-40, back = injury3(frame 224). You fall instead if you get hit in mid-air.
//              6* :fall 41-60 = injury4(DOP frame 226). You fall instead if you get hit in mid-air. Itrs with this range of fall can hit falling characters and have stronger pushing power.
//              7* :Anything more than 60 will just send you into falling immediately, can hit falling characters and have stronger pushing power.
//              8* :Apparently, fall 80 is a magic number - it can cause balls to have the same power as a state 3005 ball, and seems to be related to the "multishot bounce" glitch..
//                  * When you get sent into injury, your bdefend value is automatically set to 45.

// * When you get sent into injury, your bdefend value is automatically set to 45.

// When you reach each fall threshold, your fall value is actually automatically set to the maximum for that threshold.
            public StateType FallLeft(int inputFall, bool opositeDir, bool inMid_Air,int inputBDefense ){
                // Debug.Log( "Input Fall " + inputFall);
                Current_Fall += inputFall ;
                Current_BDefense = 45;
                // Debug.Log( "Current Fall " + Current_Fall);
                // eFacing = Mathf.Abs(eFacing)/eFacing;
                // bool opositeDir = eFacing != ourFacing;
                // Debug.Log($"eFacing + {eFacing} vs ourFacing {ourFacing} = opositeDir {opositeDir}" );
                if (Current_Fall <= 20  ){ //3*
                    Current_Fall = 20;
                    return StateType.Hurt1;
                }
                else if (Current_Fall <= 40 && Current_Fall > 20){

                  if (opositeDir) {
                      if (inMid_Air){
                        return StateType.FallingFront; //7*
                      }
                      Current_Fall = 40;
                      return StateType.Hurt2Front; //4*
                    }
                  else {
                        if (inMid_Air){
                            return StateType.FallingBack; //7*
                        }
                        Current_Fall = 40;
                        return StateType.Hurt2Back;//5*
                      }
                } 
                else if (Current_Fall > 40 && Current_Fall <= 60)
                    if (inMid_Air){
                        if (opositeDir) {
                            return StateType.FallingFront; //7*
                        }
                        else 
                            return StateType.FallingBack;//5*
                    }
                    else return StateType.DOP; //6*
                else{
                    if (opositeDir) 
                        return StateType.FallingFront; //7*
                    else 
                        return StateType.FallingBack;//7* 
                } 
                
            }


            public void Reset(){
                Current_BDefense = 0;
            }
            public void ResetAll(){
                Current_BDefense = 0;
                Current_Fall = 0;
            }

            public int AmorLeft(int bDefesedInput){
                if (Current_BDefense < BaseAmor){
                    // int timeToRecoverBaseAmor 
                    return Current_BDefense =  bDefesedInput - BaseAmor + 5; 
                }
                return Current_BDefense = 0 ;

            }


            public void FixedUpdate() {
                if (Current_BDefense > 0 ){
                    Current_BDefense -= 1;
                }
                // Debug.Log(Current_Fall);
                if (Current_Fall > 0 ){
                    Current_Fall -= 1;
                }
            }
    }
}
