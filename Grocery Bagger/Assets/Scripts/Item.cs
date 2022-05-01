using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private static int count = 0;
    private int tilesX, tilesY, rotation, ID = 0; 
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