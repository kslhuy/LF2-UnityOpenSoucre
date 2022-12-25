using System;
using System.Collections;
using System.Collections.Generic;
using LF2.Client;
using UnityEngine;

public class SummaryUI : MonoBehaviour
{
    [SerializeField]
    ClientPlayerAvatarRuntimeCollection m_PlayerAvatars;
    [SerializeField] 
    RectTransform templateCloneScores;
    [SerializeField] 
    RectTransform BaseCanvas;
    
    

    void Awake()
    {

        m_PlayerAvatars.ItemAdded += PlayerAvatarAdded;
        m_PlayerAvatars.ItemRemoved += PlayerAvatarRemoved;
    }

    void PlayerAvatarAdded(ClientPlayerAvatar clientPlayerAvatar)
    {
        SpawnTemplateClone();
    }

    private void SpawnTemplateClone()
    {
        Instantiate(templateCloneScores, BaseCanvas);
    }

    void PlayerAvatarRemoved(ClientPlayerAvatar clientPlayerAvatar)
    {

    }
}
