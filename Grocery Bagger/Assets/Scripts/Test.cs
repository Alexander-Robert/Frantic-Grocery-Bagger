using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private Transform outerInventoryBackground;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Inventory outerInventory;
    [SerializeField] private List<string> addItemSaveList;

    private int addItemSaveListIndex;

    private void Start() {
        outerInventoryBackground.gameObject.SetActive(false);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            outerInventoryBackground.gameObject.SetActive(true);
            outerInventory.Load(addItemSaveList[addItemSaveListIndex]);

            addItemSaveListIndex = (addItemSaveListIndex + 1) % addItemSaveList.Count;
        }

        if (Input.GetKeyDown(KeyCode.P)) {
            Debug.Log(inventory.Save());
        }
    }

    public static Vector3 GetMouseWorldPosition() {
        Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        vec.z = 0f;
        return vec;
    }
    public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera) {
        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }
}