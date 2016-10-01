using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerInteractionController : MonoBehaviour {

	public float useRange;

	private Camera fpsCam;
	private WaitForSeconds flyToPlayerDuration = new WaitForSeconds(0.2f);
	private PlayerInventory playerInventory;
	private GameObject interactionText;

	void Awake() {
		playerInventory = GetComponent<PlayerInventory> ();
		interactionText = GameObject.FindWithTag ("Interaction Text");
	}

	void Start () {
		fpsCam = GetComponentInChildren<Camera> ();
		interactionText.GetComponent<Text>().text = ""; //At the beginning, our text is an empty string.
	}

	void Update () {


		Debug.DrawRay(fpsCam.ViewportToWorldPoint(new Vector3(0.5f,0.5f,0.5f)),fpsCam.transform.forward * useRange, Color.red);//TODO remove
		Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f,0.5f,0.5f));
		RaycastHit hit;

		//TODO make sure this is the proper button... maybe change it to the mouse key.
		if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, useRange )) {

			//Determine what kind of item we're interacting with.
			//Takable items are items that can be added to inventory (such as weapons, money, etc.).
			if (hit.collider.CompareTag ("Takable")) {
				interactionText.GetComponent<Text>().text = "Press E to take";
					
				if (Input.GetKeyDown (KeyCode.E)) {
					TakeItem (hit);
				}
					//Grabbable items can be held and moved, but not added to inventory (e.g. large objects, crates).
			} else if (hit.collider.CompareTag ("Usable")) {
				interactionText.GetComponent<Text>().text = "Press E to use";

				if (Input.GetKeyDown (KeyCode.E)) {
					UseItem (hit);
				}
				//Usable objects are items such as buttons and doors.
			} else if (hit.collider.CompareTag ("Grabbable")) {
				interactionText.GetComponent<Text>().text = "Press E to grab";
				if (Input.GetKeyDown (KeyCode.E)) {
					Debug.Log ("GRAB!!");
				}

			} else if (hit.collider.CompareTag("Player")) {
				Debug.Log ("error, reading self");
			} else {
				interactionText.GetComponent<Text>().text = "";
				if (Input.GetKeyDown (KeyCode.E)) {
					Debug.Log (hit.collider.name + " is not Takable, Usable, or Grabbable.");
				}

			}


			
		}
	}

	private void TakeItem(RaycastHit item) {

		AddToInventory (item);

		item.rigidbody.useGravity = false;

		//Found next two lines thanks to http://forum.unity3d.com/threads/physics-move-towards-another-object.120194/
		Vector3 relativePos = transform.position - item.transform.position;
		item.rigidbody.AddForce (relativePos * 300f);
		item.collider.enabled = false;

		StartCoroutine(DestroyItem (item));


	}

	private void UseItem(RaycastHit item) {
		item.transform.SendMessage ("UseObject");
	}

	private IEnumerator DestroyItem(RaycastHit item) {

		yield return flyToPlayerDuration;
		Destroy (item.collider.gameObject);
	}

	private void AddToInventory(RaycastHit item) {
		GameObject itemToAdd = item.collider.gameObject;
		playerInventory.AddItem(itemToAdd);
	}
}
