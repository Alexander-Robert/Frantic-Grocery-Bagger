using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private Grid<int> grid;
    private Grid<bool> gridB;
    private const float tileSize = 10f;
    private void Start()
    {
        grid = new Grid<int>(5,2, tileSize, new Vector3(20, 0));
        gridB = new Grid<bool>(5,2, tileSize, new Vector3(-20, 0));
    }
    private void Update() {
        if(Input.GetMouseButtonDown(0)) {
            grid.SetValue(GetMouseWorldPosition(), grid.GetValue(GetMouseWorldPosition()) + 1);
            gridB.SetValue(GetMouseWorldPosition(), !gridB.GetValue(GetMouseWorldPosition()));
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
