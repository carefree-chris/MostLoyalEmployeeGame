using UnityEngine;
using System.Collections;

public interface IGuardState {
	
	void UpdateState();
	void ToGuardPatrolState();
	void ToGuardPursueState();
	void ToGuardSuspicionState();
	void ToGuardSearchingState();
}
