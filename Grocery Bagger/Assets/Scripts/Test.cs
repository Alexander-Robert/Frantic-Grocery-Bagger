using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private Grid<int> gridInt;
    private Grid<bool> gridBool;
    private Grid<ItemGridObject> grid;
    private const float tileSize = 10f;
    public List<Item> items; 
    
    private void Start()
    {
        //gridInt = new Grid<int>(5,2, tileSize, new Vector3(20, 0));
        //gridBool = new Grid<bool>(5,2, tileSize, new Vector3(-20, 0));
        items.Add(new Item(2, 1, tileSize));
        items.Add(new Item(2, 1, tileSize));
        Debug.Log("items: " + items.Count);
        for(int i = 0; i < items.Count; ++i) {
            Debug.Log("ID: " + items[i].getID());
        }
        grid = new Grid<ItemGridObject>(10, 10, tileSize, new Vector3(-10,-10), () => new ItemGridObject());
    }
    private void Update() {
        if(Input.GetMouseButtonDown(0)) {
            //gridInt.SetValue(GetMouseWorldPosition(), gridInt.GetValue(GetMouseWorldPosition()) + 1);
            //gridBool.SetValue(GetMouseWorldPosition(), !gridBool.GetValue(GetMouseWorldPosition()));
            grid.SetValue(GetMouseWorldPosition(), (grid.GetValue(GetMouseWorldPosition()).state == State.Empty) ? new ItemGridObject(State.Used) : new ItemGridObject(State.Empty));
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