using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance {get; private set;}
    public event EventHandler<PlacedItem> OnObjectPlaced;
    private ItemGrid<ItemGridObject> grid;
    private RectTransform itemContainer;

    private void Awake() {
        Instance = this;
        int gridWidth = 10;
        int gridHeight = 10;
        float tileSize = 10f;
        grid = new ItemGrid<ItemGridObject>(gridWidth, gridHeight, tileSize, new Vector3(0,0,0), 
                                            (Grid<ItemGridObject> g, int x, int y) => new ItemGridObject(g,x,y), true);
        itemContainer = transform.Find("ItemContainer").GetComponent<RectTransform>();
        transform.Find("BackgroundTempVisual").gameObject.SetActive(false);
    }

    public ItemGrid<ItemGridObject> GetGrid() {return grid;}

    public Vector2Int GetGridPosition(Vector3 worldPosition) {
        grid.GetGridPosition(worldPosition, out int x, out int y);
        return new Vector2Int(x, y);
    }

    public bool IsValidGridPosition(Vector2Int gridPosition) {
        return grid.IsValidGridPosition(gridPosition);
    }

     public bool TryPlaceItem(ItemSO item, Vector2Int itemOrigin, Item.Dir dir) {
        // Test Can Build
        List<Vector2Int> gridPositionList = item.GetGridPositionList(itemOrigin, dir);
        bool canPlace = true;

        //For each tile that the item takes up, check if it's possible to place the item there.
        foreach (Vector2Int gridPosition in gridPositionList) {
            bool isValidPosition = grid.IsValidGridPosition(gridPosition);
            if (!isValidPosition) {
                // Not valid
                canPlace = false;
                break;
            }
            if (!grid.GetData(gridPosition.x, gridPosition.y).canPlace()) {
                canPlace = false;
                break;
            }
        }

        if (canPlace) {
            foreach (Vector2Int gridPosition in gridPositionList) {
                if (!grid.GetData(gridPosition.x, gridPosition.y).canPlace()) {
                    canPlace = false;
                    break;
                }
            }
        }

        if (canPlace) {
            Vector2Int rotationOffset = item.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(itemOrigin.x, itemOrigin.y) + new Vector3(rotationOffset.x, rotationOffset.y) * grid.TileSize;

            PlacedItem placedObject = PlacedItem.CreateCanvas(itemContainer, placedObjectWorldPosition, itemOrigin, dir, item);
            placedObject.transform.rotation = Quaternion.Euler(0, 0, -item.GetRotationAngle(dir));

            placedObject.GetComponent<DragDrop>().Setup(this);

            foreach (Vector2Int gridPosition in gridPositionList) {
                grid.GetData(gridPosition.x, gridPosition.y).setState(State.Used);
            }

            //TODO see if this implementation works or if I have to configure it using the placedObject
            //OnObjectPlaced?.Invoke(this, placedObject);
            OnObjectPlaced?.Invoke(this, placedObject);

            // Object Placed!
            return true;
        } 
        else {
            // Object CANNOT be placed!
            return false;
        }
    }

    public void RemoveItemAt(Vector2Int removeGridPosition) {
        PlacedItem placedObject = grid.GetData(removeGridPosition.x, removeGridPosition.y).GetItem();

        if (placedObject != null) {
            // Demolish
            placedObject.DestroySelf();

            List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();
            foreach (Vector2Int gridPosition in gridPositionList) {
                grid.GetData(gridPosition.x, gridPosition.y).ClearItem();
            }
        }
    }

    public RectTransform GetItemContainer() {
        return itemContainer;
    }



    [Serializable]
    public struct AddItemTetris {
        public string itemTetrisSOName;
        public Vector2Int gridPosition;
        public Item.Dir dir;
    }

    [Serializable]
    public struct ListAddItemTetris {
        public List<AddItemTetris> addItemTetrisList;
    }

    public string Save() {
        List<PlacedItem> placedObjectList = new List<PlacedItem>();
        for (int x = 0; x < grid.Width; x++) {
            for (int y = 0; y < grid.Height; y++) {
                if (grid.GetData(x, y).getState() == State.Used) {
                    //TODO redefine placedItem so that it handles everything for Items (AKA change grid to a grid of placedItems) 
                    placedObjectList.Remove(grid.GetData(x, y).GetItem()); 
                    placedObjectList.Add(grid.GetData(x, y).GetItem());
                }
            }
        }

        List<AddItemTetris> addItemTetrisList = new List<AddItemTetris>();
        foreach (PlacedItem placedObject in placedObjectList) {
            addItemTetrisList.Add(new AddItemTetris {
                dir = placedObject.GetDir(),
                gridPosition = placedObject.GetGridPosition(),
                itemTetrisSOName = (placedObject.GetItem() as Item).name,
            });

        }

        return JsonUtility.ToJson(new ListAddItemTetris { addItemTetrisList = addItemTetrisList });
    }

    public void Load(string loadString) {
        ListAddItemTetris listAddItemTetris = JsonUtility.FromJson<ListAddItemTetris>(loadString);

        foreach (AddItemTetris addItemTetris in listAddItemTetris.addItemTetrisList) {
            TryPlaceItem(InventoryAssets.Instance.GetItemSOFromName(addItemTetris.itemTetrisSOName), addItemTetris.gridPosition, addItemTetris.dir);
        }
    }
}
