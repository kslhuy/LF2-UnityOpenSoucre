using System.Collections.Generic;
using UnityEngine;
using LF2.Client;
using System;

namespace LF2.Test{

  public class PlayerSetting : MonoBehaviour {
    public static PlayerSetting Instance;
    public Dictionary<CharacterTypeEnum, Config> map = new Dictionary<CharacterTypeEnum, Config>();
    private void Awake()
    {
        // Ensure only one instance of GlobalVariables exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If another instance already exists, destroy this one
            Destroy(gameObject);
        }
    }
  }
  [Serializable]
  public class Config {
    public StateType DDA = StateType.NONE;
    public StateType DUA = StateType.NONE ;
    public StateType DDJ = StateType.NONE ;
    public StateType DUJ = StateType.NONE ;

    [SerializeField]
    public Dictionary<StateType,StateType> map = new Dictionary<StateType, StateType>();

    public StateType state(StateType stateType) {
      switch (stateType)
      {
        case StateType.DDA1 :
          return DDA;
        case StateType.DUA1 :
          return DUA;
        case StateType.DDJ1 :
          return DDJ;
        case StateType.DUJ1 :
          return DUJ;
        default:
        return StateType.NONE;
      }
    }
    public void set(StateType stateType, StateType newState) {
      switch (stateType)
      {
        case StateType.DDA1 :
          DDA = newState;
          break;
        case StateType.DUA1 :
          DUA = newState;
          break;
        case StateType.DDJ1 :
          DDJ = newState;
          break;
        case StateType.DUJ1 :
          DUJ = newState;
          break;
      }
    }
  }
}