using UnityEngine;
using System.Collections;

public class GuardPursueState : IGuardState {

	private StatePatternGuard guard;
	private bool playerVisible;
	private bool playerSightLost = false;

	public GuardPursueState (StatePatternGuard statePatternGuard) {
		guard = statePatternGuard;
	}

	public void UpdateState() {
		LookAndPatrol ();
		if (playerSightLost) {
			guard.pursuitSearchTimer += Time.deltaTime;
		} else {
			guard.pursuitSearchTimer = 0f;
		}
	}

	public void ToGuardPatrolState() {
		guard.pursuitSearchTimer = 0f;
		guard.agent.speed = guard.normalSpeed;
		guard.currentState = guard.guardPatrolState;
	}

	public void ToGuardPursueState() {
		Debug.Log ("Cannot go to pursue state. Already in pursue state.");
	}

	public void ToGuardSuspicionState() {
		Debug.Log ("Cannot go from pursue state to suspicion state.");
	}

	public void ToGuardSearchingState() {
		guard.pursuitSearchTimer = 0f;
		guard.agent.speed = guard.searchSpeed;
		guard.currentState = guard.guardSearchingState;
	}

	//Watch for the player.
	private void LookAndPatrol() {

		//The direction from the eyes of the guard to the player
		Vector3 directionToPlayer = guard.target.position - guard.eyes.transform.position;
		RaycastHit hit;

		playerVisible = (Physics.Raycast (guard.eyes.transform.position, directionToPlayer, out hit, guard.sightRange) && hit.collider.CompareTag ("Player"));
		//The angle of the line from the guard to the player
		float angleToPlayer = Vector3.Angle(directionToPlayer, guard.transform.forward);

		//First, we check if the player is within our Field Of View, before we cast a ray. Divided by two because we want the angle from
		//the front of the guard to the player to be half the fov, otherwise an FOV of 90 turns into 180. Just... trust me.
		if (angleToPlayer < (guard.FOV / 2f) && playerVisible && GameController.inRestrictedArea) {

			guard.playerLastPosition.position = new Vector3(guard.target.position.x,guard.target.position.y,guard.target.position.z);
			playerSightLost = false;

			//TODO: Remove this once you've figured out the correct FOV.
			Debug.DrawRay (guard.eyes.transform.position, directionToPlayer, Color.red);
			//Debug.Log (playerVisible);

			Pursue ();

		} else if (angleToPlayer < (guard.FOV / 2f) && playerVisible && GameController.inRestrictedArea == false) {
			//TODO insert audio line "And don't come back!" or something. Maybe have them attack.
			ToGuardPatrolState();
			playerSightLost = false;


			//if we lose track of the player for more than a second,, we go to search state.
		} else if (!playerVisible || !(angleToPlayer < (guard.FOV / 2f))) {
			playerSightLost = true;

			if (guard.pursuitSearchTimer < guard.pursuitSearchDuration) {
				Pursue ();
			} else {
				ToGuardSearchingState ();
			}



		} else {
			Debug.Log ("WHAT HAPPENED!? This line wasn't supposed to be reached! Did someone forget a possible case scenario?");
		}

	}

	private void Pursue() {
		Vector3 myQuarry = guard.target.position;
		//Otherwise, the given transform position is hovering too high.
		myQuarry.y -= 1f;
		guard.agent.destination = myQuarry;
		guard.agent.Resume ();
	}
}
