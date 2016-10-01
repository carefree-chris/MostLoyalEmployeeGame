using UnityEngine;
using System.Collections;

public class ButtonController : MonoBehaviour {

	[SerializeField] private Material onColor;
	[SerializeField] private Material offColor;
	private bool turnedOn;
	[SerializeField] private GameObject connectedObject;


	void Awake () {
		turnedOn = false;
		GetComponent<MeshRenderer> ().material = offColor;
	}

	void UseObject() {

		if (turnedOn) {
			turnedOn = false;
			GetComponent<MeshRenderer> ().material = offColor;
			connectedObject.SendMessage ("TurnOff");
		} else if (!turnedOn) {
			turnedOn = true;
			GetComponent<MeshRenderer> ().material = onColor;
			connectedObject.SendMessage ("TurnOn");
		}

	}
}
