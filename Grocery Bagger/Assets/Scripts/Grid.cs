using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private int width;
    private int height;
    private float tileSize;
    private Vector3 originPosition;
    private int[,] gridArray;
    private TextMesh[,] debugTextArray;

    public Grid(int width, int height, float tileSize = 10f, Vector3? originPosition = null) {
        this.width = width;
        this.height = height;
        this.tileSize = tileSize;
        if (originPosition == null) {
            originPosition = new Vector3(0,0);
        }
        this.originPosition = (Vector3)originPosition;

        gridArray = new int[width, height];
        debugTextArray = new TextMesh[width, height];

        Debug.Log(width + " " + height);

        for (int x = 0; x < gridArray.GetLength(0); ++x) {
            for (int y = 0; y < gridArray.GetLength(1); ++y) {
                debugTextArray[x,y] = CreateWorldText(gridArray[x,y].ToString(), null, GetWorldPosition(x,y) + new Vector3(tileSize/2, tileSize/2), 
                20, Color.white, TextAnchor.MiddleCenter);
                //Debug.Log(x + ", " + y);
                Debug.DrawLine(GetWorldPosition(x,y), GetWorldPosition(x, y+1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x,y), GetWorldPosition(x+1, y), Color.white, 100f);
                }
        }
        Debug.DrawLine(GetWorldPosition(0,height), GetWorldPosition(width, height), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
    }
    private Vector3 GetWorldPosition(int x, int y) {
        return new Vector3(x,y) * tileSize + originPosition;
    }

    private void GetGridPosition(Vector3 worldPosition, out int x, out int y) {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / tileSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / tileSize);
    }

    public void SetValue(int x, int y, int value) {
        if (x >= 0 && y >= 0 && x < width && y < height) {
            gridArray[x,y] = value;
            debugTextArray[x,y].text = gridArray[x,y].ToString();
        }
        else
            Debug.LogWarning("trying to set " + value + " to invalid position: (" + x + ", " + y + ")");
    }

    public void SetValue(Vector3 worldPosition, int value) {
        int x, y;
        GetGridPosition(worldPosition, out x, out y);
        SetValue(x,y,value);
    }

    public int GetValue(int x, int y) {
        if (x >= 0 && y >= 0 && x < width && y < height)
            return gridArray[x,y];
        else {
            Debug.LogWarning("trying to get a value at invalid position: (" + x + ", " + y + ")");
            return -1;
        }
    }

    public int GetValue(Vector3 worldPosition) {
        int x, y;
        GetGridPosition(worldPosition, out x, out y);
        return GetValue(x, y);
    }


    public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), 
                                          int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, 
                                          TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = 5000) {
        if (color == null) color = Color.white;
        return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment, sortingOrder);
    }
    public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, 
                                           TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
    {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }
}
