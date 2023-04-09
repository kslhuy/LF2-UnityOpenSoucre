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

    public void SetCollider(int index)
    {
        
        skillsDescription.frameChecker._frameStruct[index].center = boxCollider.bounds.size; 
        skillsDescription.frameChecker._frameStruct[index].offset = boxCollider.bounds.center; 

    }

    public void SetSpriteRender(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }
}


