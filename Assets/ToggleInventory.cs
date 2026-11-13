using UnityEngine;

public class ToggleInventory : MonoBehaviour
{
    public GameObject Inventory;

    // This runs automatically once when the game starts
    
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
