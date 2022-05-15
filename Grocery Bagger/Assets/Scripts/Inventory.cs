using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    public static Inventory Instance { get; private set; }
    public event EventHandler<PlacedObject> OnObjectPlaced;
    public int gridWidth, gridHeight;
    public float cellSize;
    private Grid<GridObject> grid;
    private RectTransform itemContainer;

    private void Awake() {
        Instance = this;
        //passing a llambda function to the Grid so that each GridObject gets constructed such that it has a reference to the grid and coordinates.
        grid = new Grid<GridObject>(gridWidth, gridHeight, cellSize, new Vector3(0, 0, 0), (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y));
        itemContainer = transform.Find("ItemContainer").GetComponent<RectTransform>();
        transform.Find("BackgroundTempVisual").gameObject.SetActive(false);
    }

    public Grid<GridObject> GetGrid() {
        return grid;
    }

    public Vector2Int GetGridPosition(Vector3 worldPosition) {
        grid.GetXY(worldPosition, out int x, out int z);
        return new Vector2Int(x, z);
    }

    public bool IsValidGridPosition(Vector2Int gridPosition) {
        return grid.IsValidGridPosition(gridPosition);
    }

    public bool TryPlaceItem(ItemSO itemSO, Vector2Int placedObjectOrigin, PlacedObjectTypeSO.Dir dir) {
        // Test Can Build
        List<Vector2Int> gridPositionList = itemSO.GetGridPositionList(placedObjectOrigin, dir);
        bool canPlace = true;
        foreach (Vector2Int gridPosition in gridPositionList) {
            bool isValidPosition = grid.IsValidGridPosition(gridPosition);
            if (!isValidPosition) {
                // Not valid
                canPlace = false;
                break;
            }
            if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild()) {
                canPlace = false;
                break;
            }
        }

        if (canPlace) {
            foreach (Vector2Int gridPosition in gridPositionList) {
                if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild()) {
                    canPlace = false;
                    break;
                }
            }
        }

        if (canPlace) {
            //if we can place the item, get it's properties, add it to the grid and list tracking placed items.
            Vector2Int rotationOffset = itemSO.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(placedObjectOrigin.x, placedObjectOrigin.y) + new Vector3(rotationOffset.x, rotationOffset.y) * grid.GetCellSize();

            PlacedObject placedObject = PlacedObject.CreateCanvas(itemContainer, placedObjectWorldPosition, placedObjectOrigin, dir, itemSO);
            placedObject.transform.rotation = Quaternion.Euler(0, 0, -itemSO.GetRotationAngle(dir));

            placedObject.GetComponent<DragDrop>().Setup(this);

            foreach (Vector2Int gridPosition in gridPositionList) {
                grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
            }

            OnObjectPlaced?.Invoke(this, placedObject);

            // Object Placed!
            return true;
        } else {
            // Object CANNOT be placed!
            return false;
        }
    }

    public void RemoveItemAt(Vector2Int removeGridPosition) {
        PlacedObject placedObject = grid.GetGridObject(removeGridPosition.x, removeGridPosition.y).GetPlacedObject();

        if (placedObject != null) {
            // Demolish
            placedObject.DestroySelf();

            List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();
            foreach (Vector2Int gridPosition in gridPositionList) {
                grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
            }
        }
    }

    public RectTransform GetItemContainer() {
        return itemContainer;
    }



    [Serializable]
    public struct AddItem {
        public string itemSOName;
        public Vector2Int gridPosition;
        public PlacedObjectTypeSO.Dir dir;
    }

    [Serializable]
    public struct ListAddItem {
        public List<AddItem> addItemList;
    }

    public string Save() {
        //Get all items placed in the grid and stringify it as a JSON so that we can load it easily in another instance
        List<PlacedObject> placedObjectList = new List<PlacedObject>();
        for (int x = 0; x < grid.GetWidth(); x++) {
            for (int y = 0; y < grid.GetHeight(); y++) {
                if (grid.GetGridObject(x, y).HasPlacedObject()) {
                    placedObjectList.Remove(grid.GetGridObject(x, y).GetPlacedObject());
                    placedObjectList.Add(grid.GetGridObject(x, y).GetPlacedObject());
                }
            }
        }

        List<AddItem> addItemList = new List<AddItem>();
        foreach (PlacedObject placedObject in placedObjectList) {
            addItemList.Add(new AddItem {
                dir = placedObject.GetDir(),
                gridPosition = placedObject.GetGridPosition(),
                itemSOName = (placedObject.GetPlacedObjectTypeSO() as ItemSO).name,
            });

        }

        return JsonUtility.ToJson(new ListAddItem { addItemList = addItemList });
    }

    public void Load(string loadString) {
        //unpack the Json info and try to place each item back where it was.
        ListAddItem listAddItem = JsonUtility.FromJson<ListAddItem>(loadString);

        foreach (AddItem addItem in listAddItem.addItemList) {
            TryPlaceItem(InventoryAssets.Instance.GetItemSOFromName(addItem.itemSOName), addItem.gridPosition, addItem.dir);
        }
    }

}
