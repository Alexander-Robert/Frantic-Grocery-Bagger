using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Generic Item of some size of cells correlating to a Grid</summary>
public class Item : MonoBehaviour
{
    //TODO: figure out what fields and game components an item actually needs to work with the grid.
    private static int count = 0;
    private int tilesX, tilesY, rotation, ID;
    private float tileSize;
    private GameObject obj;

    public Item(int tilesX = 1, int tilesY = 1, float tileSize = 10f, int rotation = 0, GameObject obj = null) {
        if (tilesX < 1 || tilesY < 1) {
            Debug.LogError("Item's can not have tile sizes smaller than 1x1: x: " + tilesX + " y: " + tilesY);
            Destroy(this);
            return;
        }
        this.tilesX = tilesX;
        this.tilesY = tilesY;
        this.tileSize = tileSize;
        this.rotation = rotation;
        this.ID = count++;
        if(obj != null) {
            this.obj = obj;
            //this.obj.Transform.rotation = rotation;
        }
    }
    
    public int getID() {return ID;}
    public void getSize(out int x, out int y) {
        //depending on the rotation, the size might be switched ex. 2x1 -> 1x2
        if (rotation == 90 || rotation == 270) {
            x = tilesY;
            y = tilesX;
        }
        else {
            x = tilesX;
            y = tilesY;
        }
    }
}