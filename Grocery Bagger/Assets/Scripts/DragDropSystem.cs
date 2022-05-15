using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDropSystem : MonoBehaviour {

    public static DragDropSystem Instance { get; private set; }



    [SerializeField] private List<Inventory> inventoryList;

    private Inventory draggingInventory;
    private PlacedItem draggingPlacedItem;
    private Vector2Int mouseDragGridPositionOffset;
    private Vector2 mouseDragAnchoredPositionOffset;
    private Item.Dir dir;


    private void Awake() {
        Instance = this;
    }

    private void Start() {
        foreach (Inventory inventory in inventoryList) {
            inventory.OnObjectPlaced += (object sender, PlacedItem placedItem) => {

            };
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            dir = Item.GetNextDirClockWise(dir);
        }

        if (draggingPlacedItem != null) {
            // Calculate target position to move the dragged item
            RectTransformUtility.ScreenPointToLocalPointInRectangle(draggingInventory.GetItemContainer(), Input.mousePosition, null, out Vector2 targetPosition);
            targetPosition += new Vector2(-mouseDragAnchoredPositionOffset.x, -mouseDragAnchoredPositionOffset.y);

            // Apply rotation offset to target position
            Vector2Int rotationOffset = draggingPlacedItem.GetItem().GetRotationOffset(dir);
            targetPosition += new Vector2(rotationOffset.x, rotationOffset.y) * draggingInventory.GetGrid().TileSize;

            // Snap position
            targetPosition /= 10f;// draggingInventory.GetGrid().GetCellSize();
            targetPosition = new Vector2(Mathf.Floor(targetPosition.x), Mathf.Floor(targetPosition.y));
            targetPosition *= 10f;

            // Move and rotate dragged Item
            draggingPlacedItem.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(draggingPlacedItem.GetComponent<RectTransform>().anchoredPosition, targetPosition, Time.deltaTime * 20f);
            draggingPlacedItem.transform.rotation = Quaternion.Lerp(draggingPlacedItem.transform.rotation, Quaternion.Euler(0, 0, -draggingPlacedItem.GetItem().GetRotationAngle(dir)), Time.deltaTime * 15f);
        }
    }

    public void StartedDragging(Inventory inventory, PlacedItem placedItem) {
        // Started Dragging
        draggingInventory = inventory;
        draggingPlacedItem = placedItem;

        Cursor.visible = false;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(inventory.GetItemContainer(), Input.mousePosition, null, out Vector2 anchoredPosition);
        Vector2Int mouseGridPosition = inventory.GetGridPosition(anchoredPosition);

        // Calculate Grid Position offset from the placedItem origin to the mouseGridPosition
        mouseDragGridPositionOffset = mouseGridPosition - placedItem.GetGridPosition();

        // Calculate the anchored poisiton offset, where exactly on the image the player clicked
        mouseDragAnchoredPositionOffset = anchoredPosition - placedItem.GetComponent<RectTransform>().anchoredPosition;

        // Save initial direction when started draggign
        dir = placedItem.GetDir();

        // Apply rotation offset to drag anchored position offset
        Vector2Int rotationOffset = draggingPlacedItem.GetItem().GetRotationOffset(dir);
        mouseDragAnchoredPositionOffset += new Vector2(rotationOffset.x, rotationOffset.y) * draggingInventory.GetGrid().TileSize;
    }

    public void StoppedDragging(Inventory fromInventory, PlacedItem placedItem) {
        draggingInventory = null;
        draggingPlacedItem = null;

        Cursor.visible = true;

        // Remove item from its current inventory
        fromInventory.RemoveItemAt(placedItem.GetGridPosition());

        Inventory toInventory = null;

        // Find out which Inventory is under the mouse position
        foreach (Inventory inventory in inventoryList) {
            Vector3 screenPoint = Input.mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(inventory.GetItemContainer(), screenPoint, null, out Vector2 anchoredPosition);
            Vector2Int placedItemOrigin = inventory.GetGridPosition(anchoredPosition);
            placedItemOrigin = placedItemOrigin - mouseDragGridPositionOffset;

            if (inventory.IsValidGridPosition(placedItemOrigin)) {
                toInventory = inventory;
                break;
            }
        }

        // Check if it's on top of a Inventory
        if (toInventory != null) {
            Vector3 screenPoint = Input.mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(toInventory.GetItemContainer(), screenPoint, null, out Vector2 anchoredPosition);
            Vector2Int placedItemOrigin = toInventory.GetGridPosition(anchoredPosition);
            placedItemOrigin = placedItemOrigin - mouseDragGridPositionOffset;

            bool tryPlaceItem = toInventory.TryPlaceItem(placedItem.GetItem() as Item, placedItemOrigin, dir);

            if (tryPlaceItem) {
                // Item placed!
            } else {
                // Cannot drop item here!

                // Drop on original position
                fromInventory.TryPlaceItem(placedItem.GetItem() as Item, placedItem.GetGridPosition(), placedItem.GetDir());
            }
        } else {
            // Not on top of any Inventory !

            // Cannot drop item here!

            // Drop on original position
            fromInventory.TryPlaceItem(placedItem.GetItem() as Item, placedItem.GetGridPosition(), placedItem.GetDir());
        }
    }


}