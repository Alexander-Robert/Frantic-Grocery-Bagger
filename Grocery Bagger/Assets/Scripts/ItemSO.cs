using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//Create and manage the visual components of the scriptable object
[CreateAssetMenu()]
public class ItemSO : PlacedObjectTypeSO {
    public static void CreateVisualGrid(Transform visualParentTransform, ItemSO itemSO, float cellSize) {
        Transform visualTransform = Instantiate(InventoryAssets.Instance.gridVisual, visualParentTransform);

        // Create background
        Transform template = visualTransform.Find("Template");
        template.gameObject.SetActive(false);

        for (int x = 0; x < itemSO.width; x++) {
            for (int y = 0; y < itemSO.height; y++) {
                Transform backgroundSingleTransform = Instantiate(template, visualTransform);
                backgroundSingleTransform.gameObject.SetActive(true);
            }
        }

        visualTransform.GetComponent<GridLayoutGroup>().cellSize = Vector2.one * cellSize;

        visualTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(itemSO.width, itemSO.height) * cellSize;

        visualTransform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        visualTransform.SetAsFirstSibling();
    }
}
