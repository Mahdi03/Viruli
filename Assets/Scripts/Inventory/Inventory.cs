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
	private List<Item> inventory;
	private const string PlayerPrefsKeyName = "MahdiViruliStoredInventory";

	/*C++-fying the C+ List*/
	public Item at(int index) {
		if (index < 0 || index >= inventory.Count) {
			throw new System.Exception("Index " + index + " out of range. Length of array is " + this.length());
		}
		return inventory[index];
	}
	public void push(Item item) {
		inventory.Add(item);
	}
	public void pop() {
		inventory.RemoveAt(this.length() - 1);
	}
	public int indexOf(Item item) {
		return inventory.IndexOf(item);
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
		Item[] items = JsonHelper.FromJson<Item>(json);
		inventory.Clear();
		foreach (Item item in items) {
			this.push(item);
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
