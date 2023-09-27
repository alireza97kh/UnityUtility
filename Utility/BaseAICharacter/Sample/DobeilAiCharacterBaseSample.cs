using Dobeil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DobeilAiCharacterBaseSample : DobeilAiCharacterBase
{
	public override bool isActionAvaiable(AiState currentState, AiAction nextAction)
	{
        return true;
	}
}
