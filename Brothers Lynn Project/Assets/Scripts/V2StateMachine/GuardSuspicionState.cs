using UnityEngine;
using System.Collections;

public class GuardSuspicionState : IGuardState {

	private StatePatternGuard guard;
	//How long the guard has been in suspicion mode
	private float suspicionLength = 0f;
	bool playerVisible;

	public GuardSuspicionState (StatePatternGuard statePatternGuard) {
		guard = statePatternGuard;
	}

	public void UpdateState() {

		if (guard.currentState == guard.guardSuspicionState) {
			suspicionLength += (1 * Time.deltaTime);
		}


		Look ();
		Listen ();
	}

	public void ToGuardPatrolState() {
		guard.agent.Resume ();
		guard.currentState = guard.guardPatrolState;
		suspicionLength = 0f;
	}

	public void ToGuardPursueState() {
		guard.agent.Resume ();
		guard.currentState = guard.guardPursueState;
		guard.agent.speed = guard.pursueSpeed;
		suspicionLength = 0f;
	}

	public void ToGuardSuspicionState() {
		Debug.Log ("Cannot go to suspicion state. Already in suspicion state.");
	}

	public void ToGuardSearchingState() {
		guard.currentState = guard.guardSearchingState;
		guard.agent.speed = guard.searchSpeed;
		suspicionLength = 0f;
	}

	//Watch for the player.
	private void Look() {
		

		//The direction from the eyes of the guard to the player
		Vector3 directionToPlayer = guard.target.position - guard.eyes.transform.position;
		RaycastHit hit;

		playerVisible = (Physics.Raycast (guard.eyes.transform.position, directionToPlayer, out hit, guard.sightRange) && hit.collider.CompareTag ("Player"));

		//The angle of the line from the guard to the player
		float angleToPlayer = Vector3.Angle(directionToPlayer , guard.transform.forward);
		//First, we check if the player is within our Field Of View, before we cast a ray. Divided by two because we want the angle from
		//the front of the guard to the player to be half the fov, otherwise an FOV of 90 turns into 180. Just... trust me.
		if ((angleToPlayer < (guard.FOV / 2f)) && playerVisible && GameController.inRestrictedArea) {

			//Update the position in which we last saw the player.
			guard.playerLastPosition.position = new Vector3(guard.target.position.x,guard.target.position.y,guard.target.position.z);

			//Debug.Log ("Player seen IN RESTRICTED AREA while in SUSPICION MODE");
			//TODO: Remove this once you've figured out the correct FOV.
			Debug.DrawRay (guard.eyes.transform.position, directionToPlayer, Color.red);

			//When the guard is suspicious, they should face the player, eyeing them with said suspicion.
			Transform targetToRotateTowards = GameController.playerTransform;
			guard.transform.LookAt (targetToRotateTowards);
			//Stop the guard from going to the next waypoint, by making that waypoint where he/she/it is standing.
			guard.agent.destination = guard.GetComponent<Transform> ().position;

			//If while patrolling, the player appears directly in front of the guard, instant pursuit.
			if (hit.distance < guard.sightInstantPursuit) {
				ToGuardPursueState ();
				return;
			}
			if (suspicionLength > guard.gracePeriod) {
				ToGuardPursueState ();
				return;
			}




			//If the player has moved to an unrestricted area within 3 seconds, we let them be.
		} else if (playerVisible && GameController.inRestrictedArea == false) {
			//TODO add audio line "And you stay out" or something. But, y'know, better.
			ToGuardPatrolState();

			//If the guard cannot see the player, and it's been less than two seconds, we continue patrolling (no punishment);
		} else if (!playerVisible && suspicionLength <= guard.gracePeriod) {
			//Debug.Log ("LOST SIGHT OF PLAYER - Resuming patrol");
			ToGuardPatrolState ();

			//If the guard cannot see the player, but it's been more than two seconds, they investigate.
		} else if (!playerVisible && suspicionLength > guard.gracePeriod) {
			//Debug.Log ("LOST SIGHT OF PLAYER - More than " + guard.gracePeriod + " seconds - Proceding to Search");
			ToGuardSearchingState ();
		} else {
			Debug.Log ("Hear ye hear ye, this is the final else, which must never be reached.");
		}

	}
		


	//Listen for the player.
	private void Listen() {
		//TODO implement when needed
	}
}
