using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Use the template for the inventory background to create each tile given the size, position and cell size
public class InventoryBackground : MonoBehaviour {
    [SerializeField] private Inventory inventory;
    private void Start() {
        // Create background
        Transform template = transform.Find("Template");
        template.gameObject.SetActive(false);

        for (int x = 0; x < inventory.GetGrid().GetWidth(); x++) {
            for (int y = 0; y < inventory.GetGrid().GetHeight(); y++) {
                Transform backgroundTransform = Instantiate(template, transform);
                backgroundTransform.gameObject.SetActive(true);
            }
        }
        GetComponent<GridLayoutGroup>().cellSize = new Vector2(inventory.GetGrid().GetCellSize(), inventory.GetGrid().GetCellSize());
        GetComponent<RectTransform>().sizeDelta = new Vector2(inventory.GetGrid().GetWidth(), inventory.GetGrid().GetHeight()) * inventory.GetGrid().GetCellSize();
        GetComponent<RectTransform>().anchoredPosition = inventory.GetComponent<RectTransform>().anchoredPosition;
    }
}