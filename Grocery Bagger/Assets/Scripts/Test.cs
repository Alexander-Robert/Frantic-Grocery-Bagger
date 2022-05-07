using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private Grid<int> gridInt;
    private Grid<bool> gridBool;
    private ItemGrid<ItemGridObject> itemGrid;
    private const float tileSize = 10f;
    public List<Item> items; 
    
    private void Start()
    {
        // items.Add(new Item(2, 1, tileSize));
        // items.Add(new Item(2, 1, tileSize));
        // Debug.Log("items: " + items.Count);
        // for(int i = 0; i < items.Count; ++i) {
        //     Debug.Log("ID: " + items[i].getID());
        // }
        itemGrid = new ItemGrid<ItemGridObject>(10, 10, tileSize, new Vector3(-50,-50), 
                                                (Grid<ItemGridObject> g, int x, int y) => new ItemGridObject(g,x,y), true);
    }
    private void Update() {
        if(Input.GetMouseButtonDown(0)) {           
            ItemGridObject itemGridObject = itemGrid.GetData(GetMouseWorldPosition());
            if(itemGridObject != null) {
                itemGridObject.setState((itemGridObject.getState() == State.Empty) ? State.Used : State.Empty);
            }
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