using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handle the logic for placing items in different positions and inventories
public class ManualPlacement : MonoBehaviour {

    public static ManualPlacement Instance { get; private set; }

    public event EventHandler OnSelectedChanged;
    public event EventHandler OnObjectPlaced;

    [SerializeField] private Canvas canvas = null;
    [SerializeField] private List<PlacedObjectTypeSO> placedObjectTypeSOList = null;

    private PlacedObjectTypeSO placedObjectTypeSO;
    private PlacedObjectTypeSO.Dir dir;
    private Inventory inventory;
    private RectTransform canvasRectTransform;
    private RectTransform itemContainer;



    private void Awake() {
        Instance = this;

        inventory = GetComponent<Inventory>();

        placedObjectTypeSO = null;

        if (canvas == null) {
            canvas = GetComponentInParent<Canvas>();
        }

        if (canvas != null) {
            canvasRectTransform = canvas.GetComponent<RectTransform>();
        }
        //use the empty child "ItemContainer" to hold each cloned item
        itemContainer = transform.Find("ItemContainer").GetComponent<RectTransform>();
    }

    private void Update() {
        // Try to place
        if (Input.GetMouseButtonDown(0) && placedObjectTypeSO != null) {
            //get the cell corresponding to the current mouse position to anchor the item to
            RectTransformUtility.ScreenPointToLocalPointInRectangle(itemContainer, Input.mousePosition, null, out Vector2 anchoredPosition);
            
            Vector2Int placedObjectOrigin = inventory.GetGridPosition(anchoredPosition);

            bool tryPlaceItem = inventory.TryPlaceItem(placedObjectTypeSO as ItemSO, placedObjectOrigin, dir);

            if (tryPlaceItem) {
                OnObjectPlaced?.Invoke(this, EventArgs.Empty);
            } else {
                // Cannot build here
            }
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            dir = PlacedObjectTypeSO.GetNextDir(dir);
        }

        //selecting which item you want to spawn from the given list.
        if (Input.GetKeyDown(KeyCode.Alpha1)) { placedObjectTypeSO = placedObjectTypeSOList[0]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { placedObjectTypeSO = placedObjectTypeSOList[1]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { placedObjectTypeSO = placedObjectTypeSOList[2]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { placedObjectTypeSO = placedObjectTypeSOList[3]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { placedObjectTypeSO = placedObjectTypeSOList[4]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha6)) { placedObjectTypeSO = placedObjectTypeSOList[5]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha7)) { placedObjectTypeSO = placedObjectTypeSOList[6]; RefreshSelectedObjectType(); }

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
        placedObjectTypeSO = null; RefreshSelectedObjectType();
    }

    private void RefreshSelectedObjectType() {
        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetCanvasSnappedPosition() {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(itemContainer, Input.mousePosition, null, out Vector2 anchoredPosition);
        inventory.GetGrid().GetXY(anchoredPosition, out int x, out int y);

        if (placedObjectTypeSO != null) {
            Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
            Vector2 placedObjectCanvas = inventory.GetGrid().GetWorldPosition(x, y) + new Vector3(rotationOffset.x, rotationOffset.y) * inventory.GetGrid().GetCellSize();
            return placedObjectCanvas;
        } else {
            return anchoredPosition;
        }
    }

    public Quaternion GetPlacedObjectRotation() {
        if (placedObjectTypeSO != null) {
            return Quaternion.Euler(0, 0, -placedObjectTypeSO.GetRotationAngle(dir));
        } else {
            return Quaternion.identity;
        }
    }

    public PlacedObjectTypeSO GetPlacedObjectTypeSO() {
        return placedObjectTypeSO;
    }



}
