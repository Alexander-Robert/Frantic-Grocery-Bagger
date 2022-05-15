using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Generic Item of some size of cells correlating to a Grid</summary>
public class Item : MonoBehaviour
{
    //TODO: figure out what fields and game components an item actually needs to work with the grid.
    public static int count = 0;
    public int width, height, tileSize, ID;
    public string stringName;
    public Transform prefab;
    public Transform visual;

    public Item(int width, int height, int tileSize) {
        if (width < 1 || height < 1) {
            Debug.LogError("Item's can not have tile sizes smaller than 1x1: x: " + width + " y: " + height);
            Destroy(this);
            return;
        }
        this.width = width;
        this.height = height;
        this.ID = count++;
    }

    ///<summary>Given a parentTransform, create a visual of a prefab for this item</summary>
    public static void CreateVisualGrid(Transform visualParentTransform, Item item, float tileSize) {
        Transform visualTransform = Instantiate(InventoryAssets.Instance.gridVisual, visualParentTransform);

        // Create background
        Transform template = visualTransform.Find("Template");
        template.gameObject.SetActive(false);

        for (int x = 0; x < item.width; x++) {
            for (int y = 0; y < item.height; y++) {
                Transform backgroundSingleTransform = Instantiate(template, visualTransform);
                backgroundSingleTransform.gameObject.SetActive(true);
            }
        }

        visualTransform.GetComponent<GridLayoutGroup>().cellSize = Vector2.one * tileSize;

        visualTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(item.width, item.height) * tileSize;

        visualTransform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        visualTransform.SetAsFirstSibling();
    }
    
    public int getID() {return ID;}

    //Direction needed for item orientation
    public enum Dir { Down, Left, Up, Right}

    public static Dir GetNextDirClockWise(Dir dir) {
        switch (dir) {
            default:
            case Dir.Down: return Dir.Left;
            case Dir.Left: return Dir.Up;
            case Dir.Up: return Dir.Right;
            case Dir.Right: return Dir.Down;
        }
    }

    public static Dir GetNextDirCounterClockWise(Dir dir) {
        switch (dir) {
            default:
            case Dir.Down: return Dir.Right;
            case Dir.Right: return Dir.Up;
            case Dir.Up: return Dir.Left;
            case Dir.Left: return Dir.Down;
        }
    }

    public static Dir GetDir(Vector2Int from, Vector2Int to) {
        if (from.x < to.x) {
            return Dir.Right;
        } else {
            if (from.x > to.x) {
                return Dir.Left;
            } 
            else {
                if (from.y < to.y) {
                    return Dir.Up;
                } else {
                    return Dir.Down;
                }
            }
        }
    }

    public static Vector2Int GetDirForwardVector(Dir dir) {
        switch (dir) {
            default:
            case Dir.Down: return new Vector2Int(0, -1);
            case Dir.Left: return new Vector2Int(-1, 0);
            case Dir.Up: return new Vector2Int(0, +1);
            case Dir.Right: return new Vector2Int(+1, 0);
        }
    }

    public int GetRotationAngle(Dir dir) {
        switch (dir) {
            default:
            case Dir.Down:  return 0;
            case Dir.Left:  return 90;
            case Dir.Up:    return 180;
            case Dir.Right: return 270;
        }
    }

    //Once an item is rotated, it does so about it's origin coordinates,
    //therefore, we must offset it so that it rotates around it's center.
    public Vector2Int GetRotationOffset(Dir dir) {
        switch (dir) {
            default:
            case Dir.Down:  return new Vector2Int(0, 0);
            case Dir.Left:  return new Vector2Int(0, width);
            case Dir.Up:    return new Vector2Int(width, height);
            case Dir.Right: return new Vector2Int(height, 0);
        }
    }

    ///<summary>Given the direction and offset, return which grid positions the item is currently occupying</summary>
    public List<Vector2Int> GetGridPositionList(Vector2Int offset, Dir dir) {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();
        switch (dir) {
            default:
            case Dir.Down:
            case Dir.Up:
                for (int x = 0; x < width; x++) {
                    for (int y = 0; y < height; y++) {
                        gridPositionList.Add(offset + new Vector2Int(x, y));
                    }
                }
                break;
            case Dir.Left:
            case Dir.Right:
                for (int x = 0; x < height; x++) {
                    for (int y = 0; y < width; y++) {
                        gridPositionList.Add(offset + new Vector2Int(x, y));
                    }
                }
                break;
        }
        return gridPositionList;
    }
}