using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class for creating sized grids at any offset on the screen holding a generic template type that you give it. 
/// </summary>
public class Grid<TGridObject>{
    protected int width;
    protected int height;
    protected float tileSize;
    protected Vector3 originPosition;
    protected TGridObject[,] gridArray;
    protected TextMesh[,] debugTextArray;

    /// <summary>
    /// Constructing a grid only requires the number of rows and columns. consistant tileSize recommended.  
    /// </summary>
    /// <param name="tileSize"> (float) the scale of each tile</param>
    /// <param name="originPosition"> (Vector3) type offset for the grid position</param>
    /// <param name="createGridObject"> object-type grids require passing an arrow function creating a new object of that type </param>
    public Grid(int width, int height, float tileSize = 10f, Vector3? originPosition = null, Func<TGridObject> createGridObject = null) {
        this.width = width;
        this.height = height;
        this.tileSize = tileSize;
        if (originPosition == null) {
            originPosition = new Vector3(0,0);
        }
        this.originPosition = (Vector3)originPosition;

        gridArray = new TGridObject[width, height];
        debugTextArray = new TextMesh[width, height];

        Debug.Log(width + " " + height);

        for (int x = 0; x < gridArray.GetLength(0); ++x) {
            for (int y = 0; y < gridArray.GetLength(1); ++y) {
                if(createGridObject != null) {
                    gridArray[x,y] = createGridObject();
                }
                
                //debugTextArray[x,y] = CreateWorldText(gridArray[x,y]?.ToString(), null, GetWorldPosition(x,y) + new Vector3(tileSize/2, tileSize/2), 20, Color.white, TextAnchor.MiddleCenter);
                //Debug.Log(x + ", " + y);
                Debug.DrawLine(GetWorldPosition(x,y), GetWorldPosition(x, y+1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x,y), GetWorldPosition(x+1, y), Color.white, 100f);
                }
        }
        Debug.DrawLine(GetWorldPosition(0,height), GetWorldPosition(width, height), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
    }
    protected Vector3 GetWorldPosition(int x, int y) {
        return new Vector3(x,y) * tileSize + originPosition;
    }

    protected void GetGridPosition(Vector3 worldPosition, out int x, out int y) {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / tileSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / tileSize);
    }

    public void SetValue(int x, int y, TGridObject value) {
        if (x >= 0 && y >= 0 && x < width && y < height) {
            gridArray[x,y] = value;
            debugTextArray[x,y].text = gridArray[x,y]?.ToString();
        }
        else {
            //Debug.LogWarning("trying to set " + value + " to invalid position: (" + x + ", " + y + ")");
        }
    }

    public void SetValue(Vector3 worldPosition, TGridObject value) {
        int x, y;
        GetGridPosition(worldPosition, out x, out y);
        SetValue(x,y,value);
    }

    public TGridObject GetValue(int x, int y) {
        if (x >= 0 && y >= 0 && x < width && y < height)
            return gridArray[x,y];
        else {
            //Debug.LogWarning("trying to get a value at invalid position: (" + x + ", " + y + ")");
            return default(TGridObject);
        }
    }

    public TGridObject GetValue(Vector3 worldPosition) {
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

public enum State {Empty, Used, HoverEmpty, HoverUsed}

/// <summary>
/// Object to handle the current state of each tile in relation to item placement  
/// </summary>
public class ItemGridObject {
    public State state;
    private Color tileColor,
                 empty = Color.white, 
                 used = Color.clear, 
                 hoverEmpty = Color.yellow, 
                 hoverUsed = Color.red;

    public ItemGridObject(State state) {this.state = state; setColor();}
    public ItemGridObject() : this(State.Empty) {}

    public Color getColor() {return tileColor;}
    private void SetColor(Color color) {tileColor = color;}
    /// <summary>
    /// sets the color according to the current state  
    /// </summary>
    private void setColor() {
        switch (state)
        {
            case State.Empty:
            tileColor = empty;
            break;
            case State.Used:
            tileColor = used;
            break;
            case State.HoverEmpty:
            tileColor = hoverEmpty;
            break;
            case State.HoverUsed:
            tileColor = hoverUsed;
            break;
            default:
            Debug.LogWarning("State: " + state + " not found.");
            break;
        }
    }
}

/// <summary>
/// A class for handling item interactions on a grid 
/// </summary>
public class ItemGrid<ItemGridObject> : Grid<ItemGridObject> {
    private SpriteRenderer[,] SpriteArray;
    
    public ItemGrid(int width, int height, float tileSize = 10f, Vector3? originPosition = null, Func<ItemGridObject> createGridObject = null) 
    : base(width, height, tileSize, originPosition, createGridObject) {
        SpriteArray = new SpriteRenderer[width, height];
        //TODO have each tile display the color corresponding to its state.
        for (int x = 0; x < gridArray.GetLength(0); ++x) {
            for (int y = 0; y < gridArray.GetLength(1); ++y) {
                //TODO: figure out default instatiation of Sprite class.
                //Sprite sprite = new Sprite();
                //SpriteArray[x,y] = CreateWorldSprite("", sprite, GetWorldPosition(x,y), tileSize, 1, gridArray[x,y].getColor());
            }
        }
    }

    public void SetValue(int x, int y, State value) {
        if (x >= 0 && y >= 0 && x < width && y < height) {
            //TODO figure out accessing ItemGridObject properties and methods as seen below.
            //gridArray[x,y].state = value;
            //gridArray[x,y].setColor();

            //SpriteArray[x,y].color = gridArray[x,y].getColor();
        }
        else {
            //Debug.LogWarning("trying to set " + value + " to invalid position: (" + x + ", " + y + ")");
        }
    }

    public void SetValue(Vector3 worldPosition, State value) {
        int x, y;
        GetGridPosition(worldPosition, out x, out y);
        SetValue(x,y,value);
    }

    // Create a Sprite in the World, no parent
    public static GameObject CreateWorldSprite(string name, Sprite sprite, Vector3 position, Vector3 localScale, int sortingOrder, Color color) {
        return CreateWorldSprite(null, name, sprite, position, localScale, sortingOrder, color);
    }
        
    // Create a Sprite in the World
    public static GameObject CreateWorldSprite(Transform parent, string name, Sprite sprite, Vector3 localPosition, Vector3 localScale, int sortingOrder, Color color) {
        GameObject gameObject = new GameObject(name, typeof(SpriteRenderer));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        transform.localScale = localScale;
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = sortingOrder;
        spriteRenderer.color = color;
        return gameObject;
    }
}