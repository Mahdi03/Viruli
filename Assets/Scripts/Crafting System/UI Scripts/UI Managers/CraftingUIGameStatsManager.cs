using UnityEngine;

public class CraftingUIGameStatsManager : MonoBehaviour {

    private static CraftingUIGameStatsManager instance;
    public static CraftingUIGameStatsManager Instance => instance;

    [SerializeField]
    private GameObject gameStatsTabContent; //Used to measure whether the game stats tab is being shown
    private RectTransform gameStatsTabRectTransform;

    private void Awake() {
        if (instance != this && instance != null) {
            Destroy(gameObject);
        }
        else {
            instance = this;
            //Initialize anything else here
            gameStatsTabRectTransform = gameStatsTabContent.GetComponent<RectTransform>();
        }
    }

    // Update is called once per frame
    void Update() {
        //Use activeInHierarchy to measure that not only is the tab open but that the crafting UI is also open (if a parent is inactive, the child is still shown as active in the hierarchy)
        if (gameStatsTabContent.activeInHierarchy) {
            //Then Crafting UI and Game Stats tab are both set to open, we run our render logic


            /* I think use a prefab with a controller script to assign all of these???
             * Enemies Killed:
             *     - 700 zombies
             *     - 235 trolls
             *     - 13 minotaurs
             *     
             * Number of potions crafted: 567
             * Number of doors repaired: 345
             * 
             * Time Elasped:
             *     100:100:45:75.789             
             */
        }
    }
}
