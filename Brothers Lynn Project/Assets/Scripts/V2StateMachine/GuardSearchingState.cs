using UnityEngine;
using System.Collections;

public class GuardSearchingState : IGuardState {

	private StatePatternGuard guard;
	private bool playerVisible;



	public GuardSearchingState (StatePatternGuard statePatternGuard) {
		guard = statePatternGuard;
	}

	public void UpdateState() {
		//We increment the timer, then act depending on how long we've been searching.
		guard.searchingTimer += Time.deltaTime;
		Look ();
		Listen ();
		Search();
	}

	public void ToGuardPatrolState() {
		guard.agent.speed = guard.normalSpeed;
		guard.searchingTimer = 0f;
		guard.currentState = guard.guardPatrolState;
	}

	public void ToGuardPursueState() {
		guard.agent.speed = guard.pursueSpeed;
		guard.searchingTimer = 0f;
		guard.currentState = guard.guardPursueState;
	}

	public void ToGuardSuspicionState() {
		Debug.Log ("Can't go from searching state to suspicion state.");
	}

	public void ToGuardSearchingState() {
		Debug.Log ("Can't go to searching state. Already in searching state. Idiot.");
	}
		
	//Watch for the player.
	private void Look() {


		//The direction from the eyes of the guard to the player
		Vector3 directionToPlayer = guard.target.position - guard.eyes.transform.position;
		RaycastHit hit;

		playerVisible = (Physics.Raycast (guard.eyes.transform.position, directionToPlayer, out hit, guard.sightRange) && hit.collider.CompareTag ("Player"));

		//Debug.Log (playerVisible);//TODO remove

		//The angle of the line from the guard to the player
		float angleToPlayer = Vector3.Angle(directionToPlayer , guard.transform.forward);
		//First, we check if the player is within our Field Of View, before we cast a ray. Divided by two because we want the angle from
		//the front of the guard to the player to be half the fov, otherwise an FOV of 90 turns into 180. Just... trust me.
		if ((angleToPlayer < (guard.FOV / 2f)) && playerVisible && GameController.inRestrictedArea) {

			//Debug.Log ("PLAYER DISCOVERED!");

			ToGuardPursueState ();


		} else if (playerVisible && GameController.inRestrictedArea == false) {
			//TODO add audio line "And you stay out" or something. But, y'know, better.
			ToGuardPatrolState();

		}

	}

	private void Listen() {
		//TODO implement once needed.
	}

	private void Search() {

		if (guard.searchingTimer > guard.searchingDuration) {
			//TODO insert line about "must've lost 'em" or something.
			ToGuardPatrolState();
			return;

		}

		guard.agent.destination = guard.playerLastPosition.position;
		guard.agent.Resume ();

		if (guard.agent.remainingDistance < guard.agent.stoppingDistance && !guard.agent.pathPending) {

			NextSearchPoint ();
			//TODO improve this (polish?) if needed

			/*
			if (guard.agent.hasPath) {
				Debug.Log ("YES PATH");
			} else {
				Debug.Log ("NO PATH");
			}*/

		}


	}

	private void NextSearchPoint() {
		guard.playerLastPosition.position = (Random.insideUnitSphere + guard.transform.position) + guard.searchOffset[Random.Range(0,guard.searchOffset.Length)];
	}
}
