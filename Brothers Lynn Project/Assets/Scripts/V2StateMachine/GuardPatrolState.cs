using UnityEngine;
using System.Collections;

public class GuardPatrolState : IGuardState {

	private StatePatternGuard guard;
	private int nextWayPoint;

	public GuardPatrolState (StatePatternGuard statePatternGuard) {
		guard = statePatternGuard;
	}

	public void UpdateState() {
		Look ();
		Listen ();
		Patrol ();

	}

	public void ToGuardPatrolState() {
		Debug.Log ("Cannot switch to patrol state. Already in GuardPatrolState");
	}

	public void ToGuardPursueState() {
		Debug.Log ("Cannot switch to pursue state. Must first go through suscpicion state.");
	}

	public void ToGuardSuspicionState() {
		
		guard.currentState = guard.guardSuspicionState;
		//TODO add audio and visual cue
	}

	public void ToGuardSearchingState() {
		Debug.Log ("Cannot switch to searching state. Must be in suspicion or pursuit state first.");
	}

	//Watch for the player.
	private void Look() {

		//The direction from the eyes of the guard to the player
		Vector3 directionToPlayer = guard.target.position - guard.eyes.transform.position;

		//The angle of the line from the guard to the player
		float angleToPlayer = Vector3.Angle(directionToPlayer , guard.transform.forward);

		//First, we check if the player is within our Field Of View, before we cast a ray. Divided by two because we want the angle from
		//the front of the guard to the player to be half the fov, otherwise an FOV of 90 turns into 180. Just... trust me.
		if (angleToPlayer < (guard.FOV / 2f)) {

			RaycastHit hit;



			//Now that we know the player is in our FOV, we check if any objects are in between the guard and the player by raycasting.
			//We don't use one if statement so we aren't always raycasting. I don't know how expensive it is to raycast, but I'd
			//rather be on the safe side.
			if (Physics.Raycast(guard.eyes.transform.position, directionToPlayer, out hit, guard.sightRange) && GameController.inRestrictedArea && hit.collider.CompareTag("Player")) {
			//if (Physics.Raycast(guard.eyes.transform.position, guard.eyes.transform.forward, out hit, guard.sightRange) && GameController.inRestrictedArea && hit.collider.CompareTag("Player")) {
				//Debug.Log ("Player seen IN RESTRICTED AREA");

				guard.playerLastPosition.position = new Vector3(guard.target.position.x,guard.target.position.y,guard.target.position.z);

				//TODO: Remove this once you've figured out the correct FOV.
				Debug.DrawRay (guard.eyes.transform.position, directionToPlayer, Color.red);




				//Debug.DrawRay (guard.eyes.transform.position, guard.eyes.transform.forward * guard.sightRange, Color.red);
				//If the player is seen, we go immediately to Suspicion State, and figure out our reaction based on time and distance.
				ToGuardSuspicionState();

			}
		}

	}


	//Listen for the player.
	private void Listen() {
		//TODO implement when needed
	}

	//The actual patrolling from waypoint to waypoint.
	private void Patrol() {

		guard.agent.destination = guard.wayPoints[nextWayPoint].position;
		guard.agent.Resume ();
		if (guard.agent.remainingDistance < guard.agent.stoppingDistance && !guard.agent.pathPending) {
			nextWayPoint = (nextWayPoint + 1) % guard.wayPoints.Length;
		}
	}
}
