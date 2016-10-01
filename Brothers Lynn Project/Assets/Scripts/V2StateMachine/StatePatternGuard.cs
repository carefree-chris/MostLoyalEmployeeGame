using UnityEngine;
using System.Collections;

public class StatePatternGuard : MonoBehaviour {



	//The pursuitSearchDuration determines how long the guard can follow the player after losing sight of them.
	//This is in case, say, the player sidesteps their vision cone for half a second, but they should be able
	//to turn to see the player. Should, ideally, be unnoticable to the player.
	[HideInInspector] public float pursuitSearchTimer = 0f;
	[HideInInspector] public float searchingTimer = 0f;
	public float pursuitSearchDuration;
	public float searchingDuration;

	//How far the guard can see.
	public float sightRange = 20f;
	//The distance at which the guard will instantly pursue the player
	public float sightInstantPursuit = 3f;
	public Transform[] wayPoints;
	public Transform eyes;
	public float FOV = 90f;
	//The amount of time the player can try to hide, before the guard investigates
	public float gracePeriod = 3;

	//The various speeds of the guard.
	[HideInInspector] public float normalSpeed;
	[HideInInspector] public float pursueSpeed;
	[HideInInspector] public float searchSpeed;

	[HideInInspector] public Vector3[] searchOffset;

	[HideInInspector] public NavMeshAgent agent;
	//We must know where the player was last.
	public Transform playerLastPosition;
	[HideInInspector] public Transform target;
	[HideInInspector] public IGuardState currentState;

	//All guard states
	[HideInInspector] public GuardPatrolState guardPatrolState;
	[HideInInspector] public GuardSuspicionState guardSuspicionState;
	[HideInInspector] public GuardPursueState guardPursueState;
	[HideInInspector] public GuardSearchingState guardSearchingState;

	void Awake() {
		guardPatrolState = new GuardPatrolState (this);
		guardSuspicionState = new GuardSuspicionState (this);
		guardPursueState = new GuardPursueState (this);
		guardSearchingState = new GuardSearchingState (this);
		agent = GetComponent<NavMeshAgent> ();
	}

	void Start () {
		//Start with patrolling
		currentState = guardPatrolState;
		target = GameObject.FindWithTag ("Player").GetComponent<Transform> ();
		playerLastPosition.position = new Vector3(target.position.x,target.position.y,target.position.z);
		normalSpeed = agent.speed;
		pursueSpeed = 3 * normalSpeed;
		searchSpeed = normalSpeed;// / 2;

		//We'll use this in search mode, later.
		searchOffset = new Vector3[4];
		searchOffset [0] = new Vector3 (3f, 0f, 3f);
		searchOffset [1] = new Vector3 (-3f, 0f, -3f);
		searchOffset [2] = new Vector3 (3f, 0f, -3f);
		searchOffset [3] = new Vector3 (-3f, 0f, 3f);


	}
	

	void Update () {
		currentState.UpdateState ();
		//Debug.Log ("Current state = " + currentState);
	}
}
