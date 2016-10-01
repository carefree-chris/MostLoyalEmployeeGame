using UnityEngine;
using System.Collections;

public class PhoneUse : MonoBehaviour {

	//TODO EVERYTHING SUBJECT TO CHANGE

	[SerializeField] private GameObject mystery;
	[SerializeField] private Material[] mysteryColors;
	private Material currentColor;

	public void UseItem() {

		currentColor = mysteryColors [Random.Range (0, mysteryColors.Length)];

		GameObject myBox = (GameObject)Instantiate (mystery, (this.transform.position + transform.forward*4f), this.transform.rotation);
		myBox.GetComponent<MeshRenderer>().material = currentColor;

		if (this.GetComponent<Transform> ().GetChild (1).name == "Quad") {
			this.GetComponent<Transform> ().GetChild (1).GetComponent<MeshRenderer> ().material = currentColor;
		}


	}

}
