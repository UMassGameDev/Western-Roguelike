using UnityEngine;

public class ToggleInventory : MonoBehaviour
{
    public GameObject Inventory;

    // This runs automatically once when the game starts
    private void Start() // Leave it blank for future edits, since the game already starts without it being active.
    {
        /*if (Inventory != null)
        {
            // Force the inventory to start invisible
            Inventory.SetActive(false);
        }*/
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleInventoryUI();
        }
    }

    public void ToggleInventoryUI()
    {
        if (Inventory != null)
        {
            // Check the current active state and flip it (Invert it using !)
            bool isActive = Inventory.activeSelf;
            Inventory.SetActive(!isActive);
        }
    }
}
