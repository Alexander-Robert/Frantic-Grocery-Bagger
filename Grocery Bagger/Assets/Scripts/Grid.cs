using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>A generic template class for creating any sized grids at any offset on the screen.</summary>
public class Grid<TGridObject> {
    //c# {get; set;} "auto property"
    //https://stackoverflow.com/questions/5096926/what-is-the-get-set-syntax-in-c
    public static Grid<TGridObject> Instance {get; private set;}
    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    //explicit definition of the event handler requiring the x and y of the object that was changed.
    public class OnGridObjectChangedEventArgs : EventArgs {
        public int x;
        public int y;
    }

    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private TGridObject[,] gridArray;

    /// <summary>Constructing a grid with any number of rows and columns.</summary>
    /// <param name="width">(int) The number of rows</param>
    /// <param name="height">(int) The number of columns</param>
    /// <param name="cellSize">(float) The scale of each tile</param>
    /// <param name="originPosition">(Vector3) The offset for the grid position</param>
    /// <param name="createGridObject">(Lambda) Object-type grids require passing a lambda operator (the arrow functions) to creat a new object of that type</param>
    public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<Grid<TGridObject>, int, int, TGridObject> createGridObject) {
        Instance = this;
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new TGridObject[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int y = 0; y < gridArray.GetLength(1); y++) {
                //if this instance was given a lambda expression, 
                    //then assign each grid to that lambda expression object definition
                gridArray[x, y] = createGridObject(this, x, y);
            }
        }

        bool showDebug = false;
        if (showDebug) {
            TextMesh[,] debugTextArray = new TextMesh[width, height];

            for (int x = 0; x < gridArray.GetLength(0); x++) {
                for (int y = 0; y < gridArray.GetLength(1); y++) {
                    debugTextArray[x, y] = CreateWorldText(gridArray[x, y]?.ToString(), null, GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f, 30, Color.white, TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);

            //OnGridObjectChanged += denotes adding delegates to the event handler
            OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) => {
                debugTextArray[eventArgs.x, eventArgs.y].text = gridArray[eventArgs.x, eventArgs.y]?.ToString();
            };
        }
    }

    public int GetWidth() {
        return width;
    }

    public int GetHeight() {
        return height;
    }

    public float GetCellSize() {
        return cellSize;
    }

    public Vector3 GetWorldPosition(int x, int y) {
        return new Vector3(x, y) * cellSize + originPosition;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y) {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    public void SetGridObject(int x, int y, TGridObject value) {
        if (x >= 0 && y >= 0 && x < width && y < height) {
            gridArray[x, y] = value;
            TriggerGridObjectChanged(x, y);
        }
    }

    //Public function to update the event handler given an x, y cell coordinate
    public void TriggerGridObjectChanged(int x, int y) {
        //quicker shorthand and thread safe version of (if OnGridObjectChanged != null) -> make new event given the args
        //https://stackoverflow.com/questions/14703698/invokedelegate
        //Note OnGridObjectChanged? is checking if it's null or not (aka it's a nullable type):
        //https://stackoverflow.com/questions/2690866/what-is-the-purpose-of-a-question-mark-after-a-type-for-example-int-myvariabl
        OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs { x = x, y = y });
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject value) {
        GetXY(worldPosition, out int x, out int y);
        SetGridObject(x, y, value);
    }

    public TGridObject GetGridObject(int x, int y) {
        if (x >= 0 && y >= 0 && x < width && y < height) {
            return gridArray[x, y];
        } else {
            return default(TGridObject);
        }
    }

    public TGridObject GetGridObject(Vector2Int gridPosition) {
        return GetGridObject(gridPosition.x, gridPosition.y);
    }
    public TGridObject GetGridObject(Vector3 worldPosition) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetGridObject(x, y);
    }

    public bool IsValidGridPosition(Vector2Int gridPosition) {
        int x = gridPosition.x;
        int y = gridPosition.y;

        if (x >= 0 && y >= 0 && x < width && y < height) {
            return true;
        } else {
            return false;
        }
    }

    //only used for the showDebug of text to provide programmers with an easy means to see how the data in each tile is being updated.
    //calls the overloaded CreateWorldText() function given default parameter options
    public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), 
                                          int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, 
                                          TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = 5000) {
        if (color == null) color = Color.white;
        return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment, sortingOrder);
    }

    //Sets all the required properties of a TextMesh to be able to display text in the game world.
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