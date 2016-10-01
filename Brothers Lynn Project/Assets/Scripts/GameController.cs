using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	//Whether the player is in a restricted area or not.
	public static bool inRestrictedArea;
	public GameObject player;
	//This keeps track of the players rotation, x coordinate, and z coordinate (y remains zero).
	public static Transform playerTransform;

	// Use this for initialization
	void Start () {
		player = GameObject.FindWithTag ("Player");
		playerTransform = GameObject.FindWithTag ("Player Location").GetComponent<Transform>();
		//inRestrictedArea = false; //The player will start in an unrestricted area.



	}
	
	// Update is called once per frame
	void Update () {
		UpdatePlayerLocation ();
	}

	void UpdatePlayerLocation() {
		playerTransform.position = new Vector3 (player.GetComponent<Transform> ().position.x, 0f, player.GetComponent<Transform> ().position.z);
		playerTransform.rotation = player.GetComponent<Transform> ().rotation;
	}

}
