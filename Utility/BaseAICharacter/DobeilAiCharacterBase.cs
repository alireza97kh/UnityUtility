using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dobeil;
using System.Linq;

public abstract class DobeilAiCharacterBase : MonoBehaviour
{
    public DobeilAiCharacterBaseDataModel aiStatesData;
	public AiState currentState;

	private void Start()
	{
		if (aiStatesData.allStates.Count > 0)
			currentState = aiStatesData.allStates.First().Value;
	}

	private void Update()
	{
		CalculateNextState();
	}

	private void CalculateNextState()
	{
		int bestScore = -1;
		AiAction bestAction = null;
		foreach (var actionScore in currentState.stateActions)
		{
			if (isActionAvaiable(currentState, actionScore) && bestScore < actionScore.actionScore)
			{
				bestAction = actionScore;
				bestScore = actionScore.actionScore;
			}
		}
		if (bestScore != -1 && bestAction != null && aiStatesData.allStates.ContainsKey(bestAction.nextStateName))
		{
			currentState = aiStatesData.allStates[bestAction.nextStateName];
			Debug.LogError(currentState);
			currentState.executeState?.Invoke();
		}
	}

	public abstract bool isActionAvaiable(AiState currentState, AiAction nextAction);
}
