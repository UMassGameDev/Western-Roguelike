using UnityEngine;

public class ToggleInventory : MonoBehaviour
{
    public GameObject Inventory;

    // This runs automatically once when the game starts
    private void Start()
    {
        if (Inventory != null)
        {
            // Force the inventory to start invisible
            Inventory.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            //Debug.Log("E key was pressed!");
            ToggleInventoryUI();
        }
    }

    public void ToggleInventoryUI()
    {
        if (Inventory != null)
        {
            //Debug.Log("should toggle");

            // Check the current active state and flip it (Invert it using !)
            bool isActive = Inventory.activeSelf;
            Inventory.SetActive(!isActive);
        }
    }
}
