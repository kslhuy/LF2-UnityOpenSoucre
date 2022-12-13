using System.Collections.Generic;
using UnityEngine;

namespace LF2.Client
{
	public abstract class StateLogicSO : SkillsDescription
	{
		/// <summary>
		/// Will create a new custom <see cref="StateAction"/> or return an existing one inside <paramref name="createdInstances"/>
		/// </summary>
        // public SkillsDescription stateData;
		public StateActionLogic GetAction(StateMachineNew stateMachine)
		{

			var action = CreateAction();
			action.stateData = this;
			action.Awake(stateMachine);
			return action;
		}
		protected abstract StateActionLogic CreateAction();
	}

	public abstract class StateLogicSO<T> : StateLogicSO where T : StateActionLogic, new()
	{
		protected override StateActionLogic CreateAction() => new T();
	}
}
