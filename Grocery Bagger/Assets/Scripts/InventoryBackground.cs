using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryBackground : MonoBehaviour {
    [SerializeField] private Inventory inventory;
    private void Start() {
        // Create background
        Transform template = transform.Find("Template");
        template.gameObject.SetActive(false);

        for (int x = 0; x < inventory.GetGrid().Width; x++) {
            for (int y = 0; y < inventory.GetGrid().Height; y++) {
                Transform backgroundTransform = Instantiate(template, transform);
                backgroundTransform.gameObject.SetActive(true);
            }
        }
        GetComponent<GridLayoutGroup>().cellSize = new Vector2(inventory.GetGrid().TileSize, inventory.GetGrid().TileSize);
        GetComponent<RectTransform>().sizeDelta = new Vector2(inventory.GetGrid().Width, inventory.GetGrid().Height) * inventory.GetGrid().TileSize;
        GetComponent<RectTransform>().anchoredPosition = inventory.GetComponent<RectTransform>().anchoredPosition;
    }
}