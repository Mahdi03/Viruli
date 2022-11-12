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
		public InternalInventoryItem(int itemID, int count) {
			this.itemID = itemID;
			this.count = count;
		}
		public int itemID { get; set; }
		public int count { get; set; }
    }
	
	private List<InternalInventoryItem> inventory; 
	private const string PlayerPrefsKeyName = "MahdiViruliStoredInventory";
	public Inventory(int initialElements = 0) {
		this.inventory = new List<InternalInventoryItem>(initialElements);
	}
	/*C++-fying the C+ List*/
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
        if ((indexA < 0 || indexA > this.length()) || (indexB < 0 || indexB > this.length())) {
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
	/*
	public int indexOf(IItem item) {
		//Search through inventory to find the first index of an item with a matching ID
		for (int index = 0; index < this.length(); index++) {
            (IItem, int) currentVal = this.at(index);
			if (currentVal.Item1.ID == item.ID) {
				return index;
			}
        }
		return -1; //Return -1 when not found
	}
	public int indexOf((IItem, int) inventoryItem) {
		for (int index = 0; index < this.length(); index++) {
            (IItem, int) currentVal = this.at(index);
			if (currentVal == inventoryItem) {
				return index;
			}
        }
		return -1;
	}
	*/
	public int length() {
		return this.inventory.Count;
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
