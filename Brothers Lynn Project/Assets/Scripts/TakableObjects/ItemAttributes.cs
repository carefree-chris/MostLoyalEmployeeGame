using UnityEngine;
using System.Collections;

public class ItemAttributes : MonoBehaviour {


	[SerializeField] private Camera fpsCam; 			//Reference to the player camera.
	[SerializeField] private string objectName;			//The name of the object.
	[SerializeField] private float throwForce;			//How much force with which to throw the object.
	[SerializeField] private float soundVolume;
	[SerializeField] private GameObject soundSphere;

	//These variables will determine where the item instantiates in front of the camera.
	[SerializeField] private float xViewportOffset;
	[SerializeField] private float yViewportOffset;
	[SerializeField] private float zViewportOffset;

	//How the item will be rotated (towards the screen).
	[SerializeField] private float xRotationOffset;
	[SerializeField] private float yRotationOffset;
	[SerializeField] private float zRotationOffset;

	//This will keep track of whether or not the object has recently been thrown by the player.
	private bool isThrown;

	void Awake() {
		isThrown = false; //Because at the start of the game, no objects have been thrown yet.
		gameObject.name = objectName;
		fpsCam = GameObject.FindWithTag ("Player").GetComponentInChildren<Camera> ();

		if (soundSphere != null) {
			soundSphere.GetComponent<SoundSphereController> ().setSize (soundVolume);
		} else {
			Debug.Log ("Item does not have a sound sphere... it should.");

		}

	}

	public void UseItemFunction() {
		SendMessage ("UseItem", SendMessageOptions.DontRequireReceiver);
	}

	public Vector3 GetItemViewportLocation() {
		return new Vector3 (xViewportOffset, yViewportOffset, zViewportOffset);
	}

	public float GetXRotationOffset() {
		return xRotationOffset;
	}
	public float GetYRotationOffset() {
		return yRotationOffset;
	}
	public float GetZRotationOffset() {
		return zRotationOffset;
	}

	public float GetThrowForce() {
		return throwForce;
	}

	public bool getIsThrown() {
		return isThrown;
	}

	public void setIsThrown(bool isThrownOrNot) {
		isThrown = isThrownOrNot;
	}

	void OnCollisionEnter(Collision collision) {
		
		if(isThrown && !collision.collider.CompareTag("Player")) {
			
			Instantiate (soundSphere, GetComponent<Transform>().position, GetComponent<Transform>().rotation);

			setIsThrown (false); //TODO Make sure that the item can bounce, and still make sound.
		}

	}
}
