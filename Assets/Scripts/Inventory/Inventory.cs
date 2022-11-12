using System;
using System.Collections;
using System.Collections.Generic;
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


public class Inventory : IEnumerable {
	private struct InternalInventoryItem {

	}
	
	private List<(IItem, int)> inventory; //This will store an item instance from the game with the count
	private const string PlayerPrefsKeyName = "MahdiViruliStoredInventory";
	public Inventory(int initialElements = 0) {
		this.inventory = new List<(IItem, int)>(initialElements);
	}
	/*C++-fying the C+ List*/
	public (IItem, int) at(int index) {
		if (index < 0 || index >= inventory.Count) {
			throw new System.Exception("Index " + index + " out of range. Length of array is " + this.length());
		}
		return inventory[index];
	}
	public void push(IItem item) {
		if (!item.stackable) {
			//We can't stack the item anyways, add it to the end
			inventory.Add((item, 1));
			return; //We've already added it, we can stop here
		}
		else {
			//Search through vector to see if item already exists or not since we can stack it
			for (int i = 0; i < inventory.Count; i++) {
				(IItem, int) currentVal = this.at(i);
				//Use Item1 for first item of tuple, Item2, for 2nd, ItemN for Nth-element of tuple
				if (currentVal.Item1.ID == item.ID) {
					//These items are the same ID, we can add them
					currentVal.Item2++;
					return; //We don't need to go any further
				}
			}
			//If we made it this far then we still have not added it, add it to the end since it does not exist
			inventory.Add((item, 1));
		}
	}
	private void push((IItem, int) inventoryItem) {
		inventory.Add(inventoryItem);
	}
	public void push((IItem, int) inventoryItem, int index) { //Change the element at a given index
		inventory[index] = inventoryItem; //Replace the element here with a new element
	}
	public void swap((IItem, int) a, (IItem, int) b) {
		//Swap the position of the two elements in the inventory array
		int indexA = this.indexOf(a), indexB = this.indexOf(b);
		if (indexA < 0 || indexB < 0) {
			throw new System.IndexOutOfRangeException("Item not found within array to swap");
		}
		else { }

	}
	public void pop() {
		this.removeAt(this.length() - 1);
	}
	public void removeAt(int index) {
		inventory.RemoveAt(index);
	}
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
	public int length() {
		return inventory.Count;
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
		(IItem, int)[] items = JsonHelper.FromJson<(IItem, int)>(json);
		inventory.Clear();
		foreach ((IItem, int) currentInventoryItem in items) {
			this.push(currentInventoryItem);
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
