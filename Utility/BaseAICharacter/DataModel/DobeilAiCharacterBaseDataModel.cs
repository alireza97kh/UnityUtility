using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dobeil
{
	[System.Serializable]
	[CreateAssetMenu(fileName = "DobeilAiCharacterBaseDataModel", menuName = "DobeilData/DobeilAiCharacterBaseDataModel")]

	public class DobeilAiCharacterBaseDataModel : ScriptableObject
	{
		public Dictionary<string, AiState> allStates;
		public DobeilAiCharacterBaseDataModel()
		{
			allStates = new Dictionary<string, AiState>();
		}
	}
	[Serializable]
	public class AiState
	{
		public string stateName;
		public List<AiAction> stateActions;
		public Action executeState;
	}
	[Serializable]
	public class AiAction
	{
		public string actionName;
		public string nextStateName;
		public int actionScore;
	}
}