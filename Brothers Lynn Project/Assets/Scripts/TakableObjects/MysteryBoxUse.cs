using UnityEngine;
using System.Collections;

public class MysteryBoxUse : MonoBehaviour {

	[SerializeField] private Material color1;
	[SerializeField] private Material color2;
	private Material currentColor;


	//We want to store the attributes attached to this object in a script.
	private ItemAttributes myAttributes;

	void Awake() {
		myAttributes = GetComponent<ItemAttributes> ();
	}

	public void UseItem() {
		if (currentColor == color1) {
			currentColor = color2;
			GetComponent<MeshRenderer> ().material = color2;
		} else if (currentColor == color2) {
			currentColor = color1;
			GetComponent<MeshRenderer> ().material = color1;
		} else {
			currentColor = color1;
			GetComponent<MeshRenderer> ().material = color1;
		}
	}


}
