using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//the logic to ensure the player can move items between cells and inventories and rotate objects without their positioning changing
public class DragDropSystem : MonoBehaviour {

    public static DragDropSystem Instance { get; private set; }



    [SerializeField] private List<Inventory> inventoryList;

    private Inventory draggingInventory;
    private PlacedObject draggingPlacedObject;
    private Vector2Int mouseDragGridPositionOffset;
    private Vector2 mouseDragAnchoredPositionOffset;
    private PlacedObjectTypeSO.Dir dir;


    private void Awake() {
        Instance = this;
    }

    private void Start() {
        foreach (Inventory inventory in inventoryList) {
            inventory.OnObjectPlaced += (object sender, PlacedObject placedObject) => {

            };
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            dir = PlacedObjectTypeSO.GetNextDir(dir);
        }

        if (draggingPlacedObject != null) {
            // Calculate target position to move the dragged item
            RectTransformUtility.ScreenPointToLocalPointInRectangle(draggingInventory.GetItemContainer(), Input.mousePosition, null, out Vector2 targetPosition);
            targetPosition += new Vector2(-mouseDragAnchoredPositionOffset.x, -mouseDragAnchoredPositionOffset.y);

            // Apply rotation offset to target position
            Vector2Int rotationOffset = draggingPlacedObject.GetPlacedObjectTypeSO().GetRotationOffset(dir);
            targetPosition += new Vector2(rotationOffset.x, rotationOffset.y) * draggingInventory.GetGrid().GetCellSize();

            // Snap position
            targetPosition /= 10f;// draggingInventory.GetGrid().GetCellSize();
            targetPosition = new Vector2(Mathf.Floor(targetPosition.x), Mathf.Floor(targetPosition.y));
            targetPosition *= 10f;

            // Move and rotate dragged Item
            draggingPlacedObject.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(draggingPlacedObject.GetComponent<RectTransform>().anchoredPosition, targetPosition, Time.deltaTime * 20f);
            draggingPlacedObject.transform.rotation = Quaternion.Lerp(draggingPlacedObject.transform.rotation, Quaternion.Euler(0, 0, -draggingPlacedObject.GetPlacedObjectTypeSO().GetRotationAngle(dir)), Time.deltaTime * 15f);
        }
    }

    public void StartedDragging(Inventory inventory, PlacedObject placedObject) {
        // Started Dragging
        draggingInventory = inventory;
        draggingPlacedObject = placedObject;

        Cursor.visible = false;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(inventory.GetItemContainer(), Input.mousePosition, null, out Vector2 anchoredPosition);
        Vector2Int mouseGridPosition = inventory.GetGridPosition(anchoredPosition);

        // Calculate Grid Position offset from the placedObject origin to the mouseGridPosition
        mouseDragGridPositionOffset = mouseGridPosition - placedObject.GetGridPosition();

        // Calculate the anchored poisiton offset, where exactly on the image the player clicked
        mouseDragAnchoredPositionOffset = anchoredPosition - placedObject.GetComponent<RectTransform>().anchoredPosition;

        // Save initial direction when started draggign
        dir = placedObject.GetDir();

        // Apply rotation offset to drag anchored position offset
        Vector2Int rotationOffset = draggingPlacedObject.GetPlacedObjectTypeSO().GetRotationOffset(dir);
        mouseDragAnchoredPositionOffset += new Vector2(rotationOffset.x, rotationOffset.y) * draggingInventory.GetGrid().GetCellSize();
    }

    public void StoppedDragging(Inventory fromInventory, PlacedObject placedObject) {
        draggingInventory = null;
        draggingPlacedObject = null;

        Cursor.visible = true;

        // Remove item from its current inventory
        fromInventory.RemoveItemAt(placedObject.GetGridPosition());

        Inventory toInventory = null;

        // Find out which Inventory is under the mouse position
        foreach (Inventory inventory in inventoryList) {
            Vector3 screenPoint = Input.mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(inventory.GetItemContainer(), screenPoint, null, out Vector2 anchoredPosition);
            Vector2Int placedObjectOrigin = inventory.GetGridPosition(anchoredPosition);
            placedObjectOrigin = placedObjectOrigin - mouseDragGridPositionOffset;

            if (inventory.IsValidGridPosition(placedObjectOrigin)) {
                toInventory = inventory;
                break;
            }
        }

        // Check if it's on top of a Inventory
        if (toInventory != null) {
            Vector3 screenPoint = Input.mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(toInventory.GetItemContainer(), screenPoint, null, out Vector2 anchoredPosition);
            Vector2Int placedObjectOrigin = toInventory.GetGridPosition(anchoredPosition);
            placedObjectOrigin = placedObjectOrigin - mouseDragGridPositionOffset;

            bool tryPlaceItem = toInventory.TryPlaceItem(placedObject.GetPlacedObjectTypeSO() as ItemSO, placedObjectOrigin, dir);

            if (tryPlaceItem) {
                // Item placed!
            } else {
                // Cannot drop item here!

                // Drop on original position
                fromInventory.TryPlaceItem(placedObject.GetPlacedObjectTypeSO() as ItemSO, placedObject.GetGridPosition(), placedObject.GetDir());
            }
        } else {
            // Not on top of any Inventory !

            // Cannot drop item here!

            // Drop on original position
            fromInventory.TryPlaceItem(placedObject.GetPlacedObjectTypeSO() as ItemSO, placedObject.GetGridPosition(), placedObject.GetDir());
        }
    }


}