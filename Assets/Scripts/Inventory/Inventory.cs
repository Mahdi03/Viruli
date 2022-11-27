using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;


//Stolen from https://stackoverflow.com/questions/36239705/serialize-and-deserialize-json-and-json-array-in-unity
public static class JsonHelper {
	public static T[] FromJson<T>(string json) {
		Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
		return wrapper.Items;
	}

	public static string ToJson<T>(T[] array) {
		Wrapper<T> wrapper = new Wrapper<T>();
		wrapper.Items = array;
		return JsonUtility.ToJson(wrapper);
	}

	public static string ToJson<T>(T[] array, bool prettyPrint) {
		Wrapper<T> wrapper = new Wrapper<T>();
		wrapper.Items = array;
		return JsonUtility.ToJson(wrapper, prettyPrint);
	}

	[Serializable]
	private class Wrapper<T> {
		public T[] Items;
	}
}

/**
 * Use itemID = -1 to signify that the space is empty
 */
public class Inventory : IEnumerable {

	//This will store the itemID with the count
	[Serializable]
	private struct InternalInventoryItem {
		public InternalInventoryItem(int itemID = -1, int count = -1) {
			this.itemID = itemID;
			this.count = count;
		}
		public int itemID { get; set; }
		public int count { get; set; }
	}

	private List<InternalInventoryItem> inventory;
	private const string PlayerPrefsKeyName = "MahdiViruliStoredInventory";
	public Inventory(int initialElements = 0) {
		this.inventory = new List<InternalInventoryItem>();
		InternalInventoryItem item = new InternalInventoryItem(-1, -1);
		for (int i = 0; i < initialElements; i++) {
			this.inventory.Add(item);
		}
	}
	/*C++-fying the C# List*/
	public (int, int) at(int index) {
		if (index < 0 || index >= inventory.Count) {
			throw new System.Exception("Index " + index + " out of range. Length of array is " + this.length());
		}
		var itemInfo = inventory[index];

		return (itemInfo.itemID, itemInfo.count);
	}
	private void insertIntoNextOpenSpot(int itemID, int count = 1, bool stackable = true) {
		if (count < 1) {
			return; //Exit out rn to stop the recursion
		}
		for (int i = 0; i < this.inventory.Count; i++) {
			InternalInventoryItem item = this.inventory[i];
			if (item.itemID < 0) {
				//This is considered an empty spot let us place it here
				if (stackable) {
					InternalInventoryItem itemToAdd = new InternalInventoryItem(itemID, count);
					this.inventory[i] = itemToAdd;
					return; //We are done adding, we don't need to go any further
				}
				else {
					//We need to recursively add each item in the next open spot until we have no more items to insert
					InternalInventoryItem itemToAdd = new InternalInventoryItem(itemID, 1);
					this.inventory[i] = itemToAdd;
					this.insertIntoNextOpenSpot(itemID, count - 1, stackable); //At least O(n^2)
					return; //We are done adding, we don't need to go any further
				}
			}
		}
		//If we made it here that means no empty spots were found, let's just add it to the end
		if (stackable) {
			//We can add all of them at once
			InternalInventoryItem itemToAdd = new InternalInventoryItem(itemID, count);
			this.inventory.Add(itemToAdd); //Pushes it to the end (direct method of List)
		}
		else {
			//We should just add them consecutively instead of recursively calling and looping again each time unnecessarily
			for (int i = 0; i < count; i++) {
				InternalInventoryItem itemToAdd = new InternalInventoryItem(itemID, 1);
				this.inventory.Add(itemToAdd); //Pushes it to the end (direct method of List)
			}
		}
	}
	public void Add(int itemID, int count = 1) {
		IItem currentItem = InGameItemsDatabaseManager.Instance.getItemByID(itemID);
		if (!currentItem.Stackable) {
			//We can't stack the item anyways, add it to the end
			this.insertIntoNextOpenSpot(itemID, count, currentItem.Stackable);
			return; //We've already added it, we can stop here
		}
		else {
			//Search through vector to see if item already exists or not since we can stack it
			for (int i = 0; i < inventory.Count; i++) {
				InternalInventoryItem currentVal = this.inventory[i];
				//Use Item1 for first item of tuple, Item2, for 2nd, ItemN for Nth-element of tuple
				if (currentVal.itemID == currentItem.ID) {
					//These items are the same ID, we can add them
					currentVal.count++;
					this.inventory[i] = currentVal; //Save the modified value to the inventory so that changes are reflected
					return; //We don't need to go any further
				}
			}
			//If we made it this far then we still have not added it, add it to the end since it does not exist
			this.insertIntoNextOpenSpot(itemID, count, currentItem.Stackable);
		}
	}
	public void swap(int indexA, int indexB) {
		if ((indexA < 0 || indexA >= this.length()) || (indexB < 0 || indexB >= this.length())) {
			throw new System.IndexOutOfRangeException("Indices provided are out of the range of the inventory array");
		}

		//Swap the position of the two elements in the inventory array
		InternalInventoryItem itemA = this.inventory[indexA];
		this.inventory[indexA] = this.inventory[indexB];
		this.inventory[indexB] = itemA;
	}
	public void pop() {
		this.removeAt(this.length() - 1);
	}
	public void removeAt(int index) {
		inventory.RemoveAt(index);
	}
	public int length() {
		return this.inventory.Count;
	}
	/// <summary>
	/// Will check for the first index of an item in the inventory with the given itemID
	/// Returns the index if the element is found
	/// Returns -1 if there is no element with that itemID in the inventory
	/// </summary>
	/// <param name="itemID"></param>
	/// <returns></returns>
	public int indexOf(int itemID) {
		for (int i = 0; i < this.inventory.Count; i++) {
			InternalInventoryItem inventoryItem= this.inventory[i];
			if (itemID == inventoryItem.itemID) {
				return i;
			}
		}
		return -1;
	}
	/*Use in crafting system to judge whether the recipe can hold */
	public int getItemCountByID(int itemID) {
		int count = 0;
		foreach (InternalInventoryItem item in this.inventory) {
			if (item.itemID == itemID) {
				count += item.count;
			}
		}
		return count;
	}
	public int getCountOfRemainingOpenSpots() {
		int countOfFullInventorySlots = 0;
		foreach (InternalInventoryItem item in this.inventory) {
			//itemID's are -1 when empty
			if (item.itemID > -1) {
				countOfFullInventorySlots++;
			}
		}
		int remainingSpots = this.inventory.Count - countOfFullInventorySlots;
		//Debug.Log(remainingSpots);

        return remainingSpots;
	}
	public void removeByID(int itemIDToRemove, int amountToRemove = 1) {
		if (amountToRemove <= 0) { return; }
		if (this.getItemCountByID(itemIDToRemove) < amountToRemove) {
			throw new SystemException("Trying to remove more elements than currently exist in inventory");
		}
		for (int i = 0; i < this.inventory.Count; i++) {
			//Loop through all the items in the inventory until we find a matching itemID
			var currentItem = this.inventory[i];
			if (currentItem.itemID == itemIDToRemove) {
				//We need to remove as much as possible from here
				int originalCountOfItemsInInventory = currentItem.count; //Store original amount of how much of this item there was

				if (amountToRemove < originalCountOfItemsInInventory) {
					//We are removing less than the total amount that exists here, we can simply change it and then save it later
					currentItem.count -= amountToRemove;
				}
				else {
					//We are either removing the same amount or more than what is stored in this slot (if it weren't stackable)
					// so we need to empty this slot
					currentItem = new InternalInventoryItem(-1, -1);
					if (amountToRemove > originalCountOfItemsInInventory) {
						//We need to remove more since there are some removals left over
						removeByID(itemIDToRemove, amountToRemove - originalCountOfItemsInInventory); //Since we already removed the original number of items in this slot
					}
				}

				//Now that we made it here let us save the new values to the inventory
				this.inventory[i] = currentItem;
				return; //We could either break or return but let's just end the function here
						//Avoids looping and finding the next element and then removing extra elements
			}
		}
		//If we made it here then our code could not find the item
	}

	//Inherit from IEnumerable so that we can use a foreach loop on this container
	public IEnumerator GetEnumerator() {
		for (int i = 0; i < this.length(); i++) {
			yield return this.at(i);
		}
	}
	public void loadInventoryFromPlayerPrefs() {
		if (PlayerPrefs.HasKey(PlayerPrefsKeyName)) {
			string json = PlayerPrefs.GetString(PlayerPrefsKeyName);
			loadFromJSONString(json);
		}
	}
	private void loadFromJSONString(string json) {
		InternalInventoryItem[] items = JsonHelper.FromJson<InternalInventoryItem>(json);
		inventory.Clear();
		foreach (InternalInventoryItem currentInventoryItem in items) {
			this.inventory.Add(currentInventoryItem); //Push directly instead of using all that in-game add logic
		}
	}
	public void saveInventoryToPlayerPrefs() {
		PlayerPrefs.SetString(PlayerPrefsKeyName, saveToJSONString());
		PlayerPrefs.Save();
	}
	private string saveToJSONString() {
		return JsonHelper.ToJson(inventory.ToArray());

	}
	private void deletePlayerPrefKey() {
		PlayerPrefs.DeleteKey(PlayerPrefsKeyName); //Use this to test
	}

}
