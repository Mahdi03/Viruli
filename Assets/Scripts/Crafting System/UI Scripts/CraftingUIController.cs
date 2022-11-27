
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
public class CraftingUIController : MonoBehaviour {
    //OpenCraftingMenu is declared in a different script since it requires both options for 2D and 3D handling
    public void CloseCraftingMenu() {
        gameObject.SetActive(false);
        MainDoorManager.Instance.UnglowAllDoors(); //Since we are closing out of it, stop the door selection
    }
}
