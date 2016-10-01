using UnityEngine;
using System.Collections;

public class WeirdCube : MonoBehaviour {

	[SerializeField] float rotateX;
	[SerializeField] float rotateY;
	[SerializeField] float rotateZ;



	void Awake () {
		GetComponent<MeshRenderer>().enabled = false;
	}
		
	void Update () {
		transform.Rotate (new Vector3(rotateX,rotateY,rotateZ) * Time.deltaTime);

	}



	public void TurnOn() {
		GetComponent<MeshRenderer>().enabled = true;
	}

	public void TurnOff() {
		GetComponent<MeshRenderer>().enabled = false;
	}
}
