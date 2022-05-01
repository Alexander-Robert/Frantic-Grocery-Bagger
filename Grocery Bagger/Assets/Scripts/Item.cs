using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private int tilesX, tilesY; 
    private float tileSize;
    private GameObject obj;
    public Item(int tilesX = 1, int tilesY = 1, float tileSize = 10f, GameObject obj = null) {
        if (tilesX < 1 || tilesY < 1) {
            Debug.LogError("Item's can not have tile sizes smaller than 1x1: x: " + tilesX + " y: " + tilesY);
            Destroy(this);
            return;
        }
        this.tilesX = tilesX;
        this.tilesY = tilesY;
        this.tileSize = tileSize;
        this.obj = obj;
    }
    
}
