using UnityEngine;
using System.Collections;

public class CheckForPlayer : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			//print ("Player has entered restricted area");
			GameController.inRestrictedArea = true;
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.tag == "Player") {
			//print ("Player has exited restricted area");
			GameController.inRestrictedArea = false;
		}
	}

	void OnTriggerStay(Collider other) {
		if (other.tag == "Player") {
			GameController.inRestrictedArea = true;
		}
	}
}
