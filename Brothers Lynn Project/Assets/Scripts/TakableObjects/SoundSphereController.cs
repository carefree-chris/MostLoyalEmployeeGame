using UnityEngine;
using System.Collections;

public class SoundSphereController : MonoBehaviour {

	private float soundDuration;
	private float soundSize;

	// Use this for initialization
	void Start () {
		soundDuration = 0f;
	}
	
	// Update is called once per frame
	void Update () {

		soundDuration += Time.deltaTime;
	

		if (soundDuration >= 1f) {
			Destroy (this.gameObject);
		}

	}

	public void setSize(float size) {
		GetComponent<Transform> ().localScale = new Vector3(size, size, size);
	}

	void OnTriggerEnter(Collider listener) {

		Debug.Log (listener.name + " heard.");

		if (listener.name == "Guard") {
			
			Debug.Log("A GUARD HAS HEARD THE CALL!");
		}
	}
}
