using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualPlacement : MonoBehaviour {

    public static ManualPlacement Instance { get; private set; }

    public event EventHandler OnSelectedChanged;
    public event EventHandler OnObjectPlaced;

    [SerializeField] private Canvas canvas = null;
    [SerializeField] private List<PlacedItem> placedItemList = null;

    private PlacedItem placedItem;
    private Item.Dir dir;
    private Inventory inventory;
    private RectTransform canvasRectTransform;
    private RectTransform itemContainer;



    private void Awake() {
        Instance = this;

        inventory = GetComponent<Inventory>();

        placedItem = null;

        if (canvas == null) {
            canvas = GetComponentInParent<Canvas>();
        }

        if (canvas != null) {
            canvasRectTransform = canvas.GetComponent<RectTransform>();
        }

        itemContainer = transform.Find("ItemContainer").GetComponent<RectTransform>();
    }

    private void Update() {
        // Try to place
        if (Input.GetMouseButtonDown(0) && placedItem != null) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(itemContainer, Input.mousePosition, null, out Vector2 anchoredPosition);
            
            Vector2Int placedObjectOrigin = inventory.GetGridPosition(anchoredPosition);

            bool tryPlaceItem = inventory.TryPlaceItem(placedItem.GetItem() as Item, placedObjectOrigin, dir);

            if (tryPlaceItem) {
                OnObjectPlaced?.Invoke(this, EventArgs.Empty);
            } else {
                // Cannot build here
            }
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            dir = Item.GetNextDirClockWise(dir);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) { placedItem = placedItemList[0]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { placedItem = placedItemList[1]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { placedItem = placedItemList[2]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { placedItem = placedItemList[3]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { placedItem = placedItemList[4]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha6)) { placedItem = placedItemList[5]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha7)) { placedItem = placedItemList[6]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha8)) { placedItem = placedItemList[7]; RefreshSelectedObjectType(); }

        if (Input.GetKeyDown(KeyCode.Alpha0)) { DeselectObjectType(); }

        // Demolish
        /*
        if (Input.GetMouseButtonDown(1)) {
            Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
            PlacedObject placedObject = grid.GetGridObject(mousePosition).GetPlacedObject();
            if (placedObject != null) {
                // Demolish
                placedObject.DestroySelf();

                List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();
                foreach (Vector2Int gridPosition in gridPositionList) {
                    grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
                }
            }
        }
        */
    }
    private void DeselectObjectType() {
        placedItem = null; RefreshSelectedObjectType();
    }
    private void RefreshSelectedObjectType() {
        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
    }
    public Vector2 GetCanvasSnappedPosition() {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(itemContainer, Input.mousePosition, null, out Vector2 anchoredPosition);
        inventory.GetGrid().GetGridPosition(anchoredPosition, out int x, out int y);

        if (placedItem != null) {
            Vector2Int rotationOffset = placedItem.GetItem().GetRotationOffset(dir);
            Vector2 placedObjectCanvas = inventory.GetGrid().GetWorldPosition(x, y) + new Vector3(rotationOffset.x, rotationOffset.y) * inventory.GetGrid().TileSize;
            return placedObjectCanvas;
        } else {
            return anchoredPosition;
        }
    }
    public Quaternion GetPlacedItemRotation() {
        if (placedItem != null) {
            return Quaternion.Euler(0, 0, -placedItem.GetItem().GetRotationAngle(dir));
        } else {
            return Quaternion.identity;
        }
    }
    public PlacedItem GetPlacedItem() {
        return placedItem;
    }
}
