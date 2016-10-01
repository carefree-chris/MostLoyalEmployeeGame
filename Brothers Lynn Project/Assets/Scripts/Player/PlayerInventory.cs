using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerInventory : MonoBehaviour {

	private GameObject[] inventoryItems;
	private int[] numberOfInventoryItems;
	private const int MAX_NUMBER_OF_ITEMS = 9;
	public Image[] inventoryItemImage; //To be filled with sprite images of inventory.
	public Text[] inventoryItemText; //To be filled with text images of inventory.
	private Transform inventoryLocation; //Where the stuff in our pockets go
	private WaitForSeconds selectAfterThrowTime = new WaitForSeconds(0.4f);

	private bool itemIsSelected; //Has the user selected an item from their inventory?
	private int selectedItem; //What item has the user selected from their inventory?
	private Camera fpsCam; //The camera through which the player views our world.
	private GameObject heldItem; //The item that the player is holding.

	void Awake () {
		fpsCam = GameObject.FindWithTag ("Player").GetComponentInChildren<Camera> ();
		inventoryLocation = GameObject.FindWithTag ("Player Inventory").GetComponent<Transform>();
		inventoryItems = new GameObject[MAX_NUMBER_OF_ITEMS];
		numberOfInventoryItems = new int[MAX_NUMBER_OF_ITEMS];
		selectedItem = -1; //Arbitrary number. In the beginning, it cannot be 0-8.

		//When no item is selected, this variable is false, and we hold an empty GameObject.
		itemIsSelected = false;

	}



	void Update() {

		if (Input.GetKeyDown (KeyCode.Mouse0) && itemIsSelected) {
			ItemAttributes itemScript = heldItem.GetComponent<ItemAttributes> ();
			itemScript.UseItemFunction ();

		}
		if (Input.GetKeyDown (KeyCode.Mouse1) && itemIsSelected) {
			ThrowHeldItem ();
		}

		//If the player presses the relevant key, we check the inventory slot.
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			DealWithInput (0);
		}
		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			DealWithInput (1);
		}
		if (Input.GetKeyDown (KeyCode.Alpha3)) {
			DealWithInput (2);
		}
		if (Input.GetKeyDown (KeyCode.Alpha4)) {
			DealWithInput (3);
		}
		if (Input.GetKeyDown (KeyCode.Alpha5)) {
			DealWithInput (4);
		}
		if (Input.GetKeyDown (KeyCode.Alpha6)) {
			DealWithInput (5);
		}
		if (Input.GetKeyDown (KeyCode.Alpha7)) {
			DealWithInput (6);
		}
		if (Input.GetKeyDown (KeyCode.Alpha8)) {
			DealWithInput (7);
		}
		if (Input.GetKeyDown (KeyCode.Alpha9)) {
			DealWithInput (8);
		}

	}


	//Honestly, I just wanted to make the Update function look shorter.
	private void DealWithInput(int inputNum) {
		//First, we make sure the slot isn't empty.
		if (CheckSlot (inputNum)) {
			return;
		}

		//Make sure this item isn't already selected..
		if (selectedItem != inputNum) {


			//If we have a different object selected, we have to destroy that first.
			if (itemIsSelected) {
				UnHoldItem (heldItem);
			}

			//We want to keep track of whether an item is selected, and
			//which item of our inventory that is. To do this, we store
			//whether or not we're holding an object with itemIsSelected,
			//and use selectedItem to store which item we're using.
			itemIsSelected = true;
			selectedItem = inputNum;

			//heldItem is the item we're currently holding. We get it
			//using the HoldItem function to instantiate it (at the correct position).
			heldItem = HoldItem (selectedItem);

		} else { //If it is selected, we deselect it.

			//To seemingly put the item back into our inventory, we use
			//the UnHoldItem function to destroy the object (in our hand), and
			//then keep track of this by setting itemIsSelected to false and changing
			//the selectedItem number to -1 (to indicate not item is selected).
			UnHoldItem(heldItem);

		}
	}

	public void AddItem( GameObject item) {

		int inventorySlot = CheckInventoryForItem (item);
		if (inventorySlot == -1) {

			//Check for the first empty slot
			int freeSlot = CheckForFreeSlot();

			if (freeSlot == -1) {
				Debug.Log ("Inventory Full.");
				return;
			}

			GameObject newInventoryItem = (GameObject)Instantiate(item, (inventoryLocation.position + new Vector3(Random.Range(-5,5),0,Random.Range(-5,5))), inventoryLocation.rotation);

			//Add the item to our inventory.
			inventoryItems [freeSlot] = newInventoryItem;

			//Since this is the first of this item we've acquired, we update our number of this type to 1.
			numberOfInventoryItems[freeSlot] = 1;

			UpdateInventorySlotText(freeSlot, item);

		} else {
			numberOfInventoryItems [inventorySlot]++;
			UpdateInventorySlotText (inventorySlot, item);
		}

	}

	private void RemoveItem(int itemToRemove) {
		//First, make sure that the item hasn't been removed yet.
		if (inventoryItems[itemToRemove] == null || numberOfInventoryItems[itemToRemove] == 0) {
			Debug.Log ("Error: Item already removed.");
			return;
		}

		//Second, check how many of that item we have, and act accordingly.
		if (numberOfInventoryItems[itemToRemove] > 1) {
			//If we have others, we simply remove one for every item we throw.
			numberOfInventoryItems [itemToRemove] -= 1;

			//We update the GUI text.
			UpdateInventorySlotText(itemToRemove, inventoryItems[itemToRemove]);

			//However, if we only have one of that item, we remove it from the array.
		} else if (numberOfInventoryItems[itemToRemove] == 1) {



			numberOfInventoryItems [itemToRemove] = 0;
			GameObject itemToDestroy = inventoryItems [itemToRemove];
			Destroy (itemToDestroy);


			//This line might be unnccessary, since we already destroyed the item. 
			//Still, setting it to null is the only way to be sure.
			inventoryItems [itemToRemove] = null; 

			//We update the UI
			UpdateInventorySlotText (selectedItem, inventoryItems[itemToRemove]);

			//If we've destroyed all of these objects, then we have no item currently selected.
			itemIsSelected = false;
			selectedItem = -1;


		}



	}

	//We toss the item that we have in our hand away!
	private void ThrowHeldItem() {



		//First, we make sure that an item is, in fact, in our hand(s).
		if (!itemIsSelected) {
			return; //If we are holding no item, we return.
		}

		//First, we unparent the object. Then, we make it solid and affected by physics.
		heldItem.GetComponent<Transform> ().parent = null;
		heldItem.GetComponent<Collider> ().enabled = true;
		heldItem.GetComponent<Rigidbody> ().isKinematic = false;

		ItemAttributes thrownItemAttributes = heldItem.GetComponent<ItemAttributes> ();
		thrownItemAttributes.setIsThrown (true);

		//Get the direction we want to throw it towards.
		Vector3 throwDirection = fpsCam.transform.forward * heldItem.GetComponent<ItemAttributes>().GetThrowForce();

		//..And add some force, to send it flying.
		heldItem.GetComponent<Rigidbody>().AddForce(throwDirection);

		int tempItemNum = selectedItem;

		//And finally, remove that item from our inventory.
		RemoveItem(selectedItem);

		itemIsSelected = false;
		selectedItem = -1;

		StartCoroutine(WaitThenSelect (tempItemNum));

	}

	private IEnumerator WaitThenSelect(int itemToSelectPostThrow) {
		yield return selectAfterThrowTime;
		if (selectedItem == -1 && !itemIsSelected) {
			DealWithInput (itemToSelectPostThrow);
		}
	}

	//Hold item in front of player, to be used or thrown away. Each item will have a different transform and
	//means of interaction.
	public GameObject HoldItem(int itemToHold) {

		float camOffset = .5f;

		ItemAttributes itemAttributes = inventoryItems [itemToHold].GetComponent<ItemAttributes> ();

		Vector3 itemPosition = fpsCam.ViewportToWorldPoint(itemAttributes.GetItemViewportLocation());


		//The rotation x,y, and z values that the object will use.
		float rx = itemAttributes.GetXRotationOffset();
		float ry = itemAttributes.GetYRotationOffset();
		float rz = itemAttributes.GetZRotationOffset();

		//Set up our position relative to the camera.
		GameObject newItem = (GameObject)Instantiate (inventoryItems [itemToHold], itemPosition , fpsCam.transform.rotation * Quaternion.Euler (rx,ry,rz),fpsCam.transform);

		//We must disable certain physical properties to make it usable by the player.
		newItem.GetComponent<Collider> ().enabled = false;
		newItem.GetComponent<Rigidbody> ().isKinematic = true;

		return newItem;
	}

	//Not to be confused with dropping an item, this will remove it from the player's hand, but not the inventory.
	public void UnHoldItem(GameObject itemToUnHold) {
		Destroy(itemToUnHold);
		selectedItem = -1;
		itemIsSelected = false;
	}


	//Check whether or not we have an inventory item. If we do, return its place. If not, return -1.
	private int CheckInventoryForItem(GameObject item) {

		for(int i = 0; i < inventoryItems.Length; i++) {
			if (inventoryItems [i] != null) {
				if (item.name == inventoryItems [i].name) {
					return i;
				}
			}
		}
			
		return -1;

	}

	//Searches our inventory for the first open slot.
	private int CheckForFreeSlot() {
		for (int i = 0; i < inventoryItems.Length; i++ ) {
			if (inventoryItems[i] == null) {
				return i;
			}
		}
		return -1;
	}

	//Checks if a specific slot is empty. Returns true if it is.
	private bool CheckSlot(int slotToCheck) {
		if (inventoryItems [slotToCheck] == null) {
			return true;
		} else {
			return false;
		}
	}

	private void UpdateInventorySlotText(int inventorySlot, GameObject item) {

		if (numberOfInventoryItems [inventorySlot] == 0 || item == null) {
			inventoryItemText [inventorySlot].text = "Empty";
			return;
		}

		if (numberOfInventoryItems[inventorySlot] == 1) {
			inventoryItemText [inventorySlot].text = item.name;
		} else {
			inventoryItemText [inventorySlot].text = item.name + " (" + numberOfInventoryItems[inventorySlot] + ")";
		}

	}
}
