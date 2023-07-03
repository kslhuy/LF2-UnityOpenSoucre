using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using LF2;
using System;

public class FrameStructComponent: MonoBehaviour {
    public BoxCollider boxCollider;
    public SkillsDescription skillsDescription;

    public SpriteRenderer spriteRenderer;

    public Transform Tf_renderHitBox;
    public Transform Tf_renderHurtBox;

    public AnimationClip animationClip;
    public AnimationClip animationClip2;

    public Animator animator;



    private int frameNow; 


    public void SetHitBoxSame(){

        
        // AnimationEvent[] eventAnims = animationClip.events ;

        // Get the animation curve for the BoxCollider size
        var allAnimationClip = animator.runtimeAnimatorController.animationClips;
        for (int j =0 ; j < allAnimationClip.Length ; j++){
            EditorCurveBinding[] curveBindings =  AnimationUtility.GetCurveBindings(allAnimationClip[j]);

            // // Create an array to store the BoxCollider size values
            // float[] sizeValues = new float[(int)(animationClip.length * animationClip.frameRate)];

            // // Iterate through each frame of the animation clip and store the BoxCollider size value in the array

            for (int i = 0; i < curveBindings.Length; i++)
            {
                
                if (curveBindings[i].propertyName == "m_Size.x"){
                    
                    AnimationCurve newAnimationCurve = AnimationUtility.GetEditorCurve(allAnimationClip[j], curveBindings[i]);
                    for (int indexKey = 0 ; indexKey< newAnimationCurve.keys.Length ; indexKey++){
                        AnimationUtility.SetKeyLeftTangentMode(newAnimationCurve ,indexKey,AnimationUtility.TangentMode.Constant);
                        AnimationUtility.SetKeyRightTangentMode(newAnimationCurve ,indexKey,AnimationUtility.TangentMode.Constant);
                    }
                    
                    var newcurveBindings = curveBindings[i]; 
                    newcurveBindings.propertyName = "hitBox.size.x";
                    newcurveBindings.type = typeof(LF2.Client.HitBoxSizeRunTime);
                    // Add new HitBox 
                    AnimationUtility.SetEditorCurve(allAnimationClip[j] ,newcurveBindings,newAnimationCurve );
                    // Remove the old Hit Box
                    AnimationUtility.SetEditorCurve(allAnimationClip[j] ,curveBindings[i],null );

                    
                }else if (curveBindings[i].propertyName == "m_Size.y"){
                    AnimationCurve newAnimationCurve = AnimationUtility.GetEditorCurve(allAnimationClip[j], curveBindings[i]);
                    for (int indexKey = 0 ; indexKey< newAnimationCurve.keys.Length ; indexKey++){
                        AnimationUtility.SetKeyLeftTangentMode(newAnimationCurve ,indexKey,AnimationUtility.TangentMode.Constant);
                        AnimationUtility.SetKeyRightTangentMode(newAnimationCurve ,indexKey,AnimationUtility.TangentMode.Constant);
                    }

                    var newcurveBindings = curveBindings[i]; 
                    newcurveBindings.propertyName = "hitBox.size.y";
                    newcurveBindings.type = typeof(LF2.Client.HitBoxSizeRunTime);
                    AnimationUtility.SetEditorCurve(allAnimationClip[j] ,newcurveBindings,newAnimationCurve );   
                    // Remove the old Hit Box
                    
                    AnimationUtility.SetEditorCurve(allAnimationClip[j] ,curveBindings[i],null );
                    
                }else if (curveBindings[i].propertyName == "m_Size.z"){
                    AnimationCurve newAnimationCurve = AnimationUtility.GetEditorCurve(allAnimationClip[j], curveBindings[i]);
                    for (int indexKey = 0 ; indexKey< newAnimationCurve.keys.Length ; indexKey++){
                        AnimationUtility.SetKeyLeftTangentMode(newAnimationCurve ,indexKey,AnimationUtility.TangentMode.Constant);
                        AnimationUtility.SetKeyRightTangentMode(newAnimationCurve ,indexKey,AnimationUtility.TangentMode.Constant);
                    }

                    var newcurveBindings = curveBindings[i]; 
                    newcurveBindings.propertyName = "hitBox.size.z";
                    newcurveBindings.type = typeof(LF2.Client.HitBoxSizeRunTime);
                    AnimationUtility.SetEditorCurve(allAnimationClip[j] ,newcurveBindings,newAnimationCurve );   
                    // Remove the old Hit Box
                    
                    AnimationUtility.SetEditorCurve(allAnimationClip[j] ,curveBindings[i],null );
                    
                }
                else if (curveBindings[i].propertyName == "m_Center.x"){
                    AnimationCurve newAnimationCurve = AnimationUtility.GetEditorCurve(allAnimationClip[j], curveBindings[i]);
                    for (int indexKey = 0 ; indexKey< newAnimationCurve.keys.Length ; indexKey++){
                        AnimationUtility.SetKeyLeftTangentMode(newAnimationCurve ,indexKey,AnimationUtility.TangentMode.Constant);
                        AnimationUtility.SetKeyRightTangentMode(newAnimationCurve ,indexKey,AnimationUtility.TangentMode.Constant);
                    }
                    
                    var newcurveBindings = curveBindings[i]; 
                    newcurveBindings.propertyName = "hitBox.center.x";
                    newcurveBindings.type = typeof(LF2.Client.HitBoxSizeRunTime);
                    AnimationUtility.SetEditorCurve(allAnimationClip[j] ,newcurveBindings,newAnimationCurve );
                    
                    AnimationUtility.SetEditorCurve(allAnimationClip[j] ,curveBindings[i],null );
                    
                }
                else if (curveBindings[i].propertyName == "m_Center.y"){
                    AnimationCurve newAnimationCurve = AnimationUtility.GetEditorCurve(allAnimationClip[j], curveBindings[i]);
                    for (int indexKey = 0 ; indexKey< newAnimationCurve.keys.Length ; indexKey++){
                        AnimationUtility.SetKeyLeftTangentMode(newAnimationCurve ,indexKey,AnimationUtility.TangentMode.Constant);
                        AnimationUtility.SetKeyRightTangentMode(newAnimationCurve ,indexKey,AnimationUtility.TangentMode.Constant);
                    }
                    var newcurveBindings = curveBindings[i]; 
                    newcurveBindings.propertyName = "hitBox.center.y";
                    newcurveBindings.type = typeof(LF2.Client.HitBoxSizeRunTime);
                    AnimationUtility.SetEditorCurve(allAnimationClip[j] ,newcurveBindings,newAnimationCurve );

                    AnimationUtility.SetEditorCurve(allAnimationClip[j] ,curveBindings[i],null );
                }
                else if (curveBindings[i].propertyName == "m_Center.z"){
                    AnimationCurve newAnimationCurve = AnimationUtility.GetEditorCurve(allAnimationClip[j], curveBindings[i]);
                    for (int indexKey = 0 ; indexKey< newAnimationCurve.keys.Length ; indexKey++){
                        AnimationUtility.SetKeyLeftTangentMode(newAnimationCurve ,indexKey,AnimationUtility.TangentMode.Constant);
                        AnimationUtility.SetKeyRightTangentMode(newAnimationCurve ,indexKey,AnimationUtility.TangentMode.Constant);
                    }
                    var newcurveBindings = curveBindings[i]; 
                    newcurveBindings.propertyName = "hitBox.center.z";
                    newcurveBindings.type = typeof(LF2.Client.HitBoxSizeRunTime);
                    AnimationUtility.SetEditorCurve(allAnimationClip[j] ,newcurveBindings,newAnimationCurve );

                    AnimationUtility.SetEditorCurve(allAnimationClip[j] ,curveBindings[i],null );
                }
                else if (curveBindings[i].propertyName == "m_Enabled"){
                    AnimationCurve newAnimationCurve = AnimationUtility.GetEditorCurve(allAnimationClip[j], curveBindings[i]);
                    var newcurveBindings = curveBindings[i]; 
                    newcurveBindings.propertyName = "EnabledBox";
                    newcurveBindings.type = typeof(LF2.Client.HitBoxSizeRunTime);
                    AnimationUtility.SetEditorCurve(allAnimationClip[j] ,newcurveBindings,newAnimationCurve );

                    AnimationUtility.SetEditorCurve(allAnimationClip[j] ,curveBindings[i],null );
                    
                
                }
            }
        

            // if (curveBindings[i].propertyName == "hurtBox.size.x"){
            //     AnimationCurve newAnimationCurve = AnimationUtility.GetEditorCurve(animationClip, curveBindings[i]);
            //     var newcurveBindings = curveBindings[i]; 
            //     newcurveBindings.propertyName = "hitBox.size.x";
            //     newcurveBindings.type = typeof(LF2.Client.HitBoxSizeRunTime);
            //     // Add new HitBox 
            //     AnimationUtility.SetEditorCurve(animationClip ,newcurveBindings,newAnimationCurve );
            //     // Remove the old Hit Box
            //     // AnimationUtility.SetEditorCurve(animationClip ,curveBindings[i],null );

                
            // }else if (curveBindings[i].propertyName == "hurtBox.size.y"){
            //     AnimationCurve newAnimationCurve = AnimationUtility.GetEditorCurve(animationClip, curveBindings[i]);
            //     var newcurveBindings = curveBindings[i]; 
            //     newcurveBindings.propertyName = "hitBox.size.y";
            //     newcurveBindings.type = typeof(LF2.Client.HitBoxSizeRunTime);
            //     AnimationUtility.SetEditorCurve(animationClip ,newcurveBindings,newAnimationCurve );   
            //     // Remove the old Hit Box
                
            //     // AnimationUtility.SetEditorCurve(animationClip ,curveBindings[i],null );
                
            // }
            // else if (curveBindings[i].propertyName == "hurtBox.center.x"){
            //     AnimationCurve newAnimationCurve = AnimationUtility.GetEditorCurve(animationClip, curveBindings[i]);
            //     var newcurveBindings = curveBindings[i]; 
            //     newcurveBindings.propertyName = "hitBox.center.x";
            //     newcurveBindings.type = typeof(LF2.Client.HitBoxSizeRunTime);
            //     AnimationUtility.SetEditorCurve(animationClip ,newcurveBindings,newAnimationCurve );
                
            //     // AnimationUtility.SetEditorCurve(animationClip ,curveBindings[i],null );
                
            // }
            // else if (curveBindings[i].propertyName == "hurtBox.center.y"){
            //     AnimationCurve newAnimationCurve = AnimationUtility.GetEditorCurve(animationClip, curveBindings[i]);
            //     var newcurveBindings = curveBindings[i]; 
            //     newcurveBindings.propertyName = "hitBox.center.y";
            //     newcurveBindings.type = typeof(LF2.Client.HitBoxSizeRunTime);
            //     AnimationUtility.SetEditorCurve(animationClip ,newcurveBindings,newAnimationCurve );

            //     // AnimationUtility.SetEditorCurve(animationClip ,curveBindings[i],null );
                
            
            // }
        }


        
        

        // Debug.Log(curveBindings[0]);
        // Debug.Log(curveBindings[0].path);
        // Debug.Log(curveBindings[1].path);

        // Debug.Log(curveBindings[7].type);
        // Debug.Log(curveBindings[1].propertyName);
        // Debug.Log(curveBindings[2].propertyName);
        // Debug.Log(curveBindings[3].propertyName);
        // Debug.Log(curveBindings[4].propertyName);
        // Debug.Log(curveBindings[5].propertyName);
        // Debug.Log(curveBindings[6].propertyName);
        // Debug.Log(curveBindings[7].propertyName);
        // Debug.Log(curveBindings[8].propertyName);
        // foreach (AnimationEvent eventt in eventAnims){
        //     eventt.functionName = 
        // }
        // eventAnim.functionName = 
        // []animationClip.events


        // Debug.Log(eventAnim.objectReferenceParameter);
        Debug.Log("ok");
        // AnimationUtility.SetAnimationEvents(animationClip2 ,eventAnims );
        // animationClip.AddEvent(eventAnim);
    } 




    // Not use now 
    #region SomeFunction for Data frame by frame (old style) 
    public void SetHurtBoxFrameNowSame(int frameWant=0){
        
        // skillsDescription.frameChecker._frameStruct[frameNow].HitBox.size = skillsDescription.frameChecker._frameStruct[frameWant].HitBox.size; 
        // skillsDescription.frameChecker._frameStruct[frameNow].HitBox.center = skillsDescription.frameChecker._frameStruct[frameWant].HitBox.center; 
    
        
        skillsDescription.frameChecker._frameStruct[frameNow].HurtBox.size = skillsDescription.frameChecker._frameStruct[frameWant].HurtBox.size; 
        skillsDescription.frameChecker._frameStruct[frameNow].HurtBox.center = skillsDescription.frameChecker._frameStruct[frameWant].HurtBox.center;
    }

    public void SetHitBox(int index)
    {
        
        skillsDescription.frameChecker._frameStruct[index].HitBox.size = boxCollider.bounds.size; 
        skillsDescription.frameChecker._frameStruct[index].HitBox.center = boxCollider.bounds.center; 

        Tf_renderHitBox.position = boxCollider.bounds.center;
        Tf_renderHitBox.localScale = boxCollider.bounds.size;

    }

    public void SetHurtBox(int index)
    {
        skillsDescription.frameChecker._frameStruct[index].HurtBox.size = boxCollider.bounds.size; 
        skillsDescription.frameChecker._frameStruct[index].HurtBox.center = boxCollider.bounds.center; 

        
        Tf_renderHurtBox.position = boxCollider.bounds.center;
        Tf_renderHurtBox.localScale = boxCollider.bounds.size;
    }

    public void SetSpriteRender(Sprite sprite , AnimationBox hitbox , AnimationBox hurtbox)
    {
        spriteRenderer.sprite = sprite;
        Tf_renderHitBox.position = hitbox.center;
        Tf_renderHitBox.localScale = hitbox.size;

        Tf_renderHurtBox.position = hurtbox.size;
        Tf_renderHurtBox.localScale = hurtbox.center;

    }
    #endregion

}


