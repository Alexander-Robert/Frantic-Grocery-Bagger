using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>A generic template class for creating any sized grids at any offset on the screen.</summary>
public class Grid<DataType> {

    //c# {get; set;} "auto property"
    //https://stackoverflow.com/questions/5096926/what-is-the-get-set-syntax-in-c
    protected int width;
    public int Width { get; set; }
    protected int height;
    public int Height { get; set; }
    protected float tileSize;
    public float TileSize { get; set; }
    protected Vector3 originPosition;
    protected DataType[,] gridArray; //the underlying data structure for the given type: 2D array
    
    //displays the primitive data type or the name of the object type. 
    //Or whatever you return in an override ToString() in the class definition for your objects.
    protected TextMesh[,] debugTextArray; 
    protected bool showDebug;
    public bool ShowDebug { get;set; }

    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    //explicit definition of the event handler requiring the x and y of the object that was changed.
    public class OnGridObjectChangedEventArgs : EventArgs {public int x,y;}

    /// <summary>Constructing a grid only requires the number of rows and columns. constant tileSize recommended.</summary>
    /// <param name="width">(int) The number of rows</param>
    /// <param name="height">(int) The number of columns</param>
    /// <param name="tileSize">(float) The scale of each tile</param>
    /// <param name="originPosition">(Vector3) The offset for the grid position</param>
    /// <param name="createGridObject">(Lambda) Object-type grids require passing a lambda operator (the arrow functions) to creat a new object of that type</param>
    /// <param name="showDebug">(Bool) Set to true if you want to see the data value displayed in the center of each tile. For objects, display whatever you want by overridding the ToString() method in the class definition of your object type</param>
    public Grid(int width, int height, float tileSize = 10f, Vector3? originPosition = null, Func<DataType> createGridObject = null, bool showDebug = false) {
        this.width = width;
        this.height = height;
        this.tileSize = tileSize;
        this.showDebug = showDebug;
        if (originPosition == null) {
            originPosition = new Vector3(0,0);
        }
        //Note the Vector3? type definition in the constructor parameters is a nullable type:
        //https://stackoverflow.com/questions/2690866/what-is-the-purpose-of-a-question-mark-after-a-type-for-example-int-myvariabl
        //because originPosition is the nullable type for a Vector3, it has to be explicitly type casted to Vector3
        this.originPosition = (Vector3)originPosition; 

        gridArray = new DataType[width, height];
        if(showDebug) 
            debugTextArray = new TextMesh[width, height];

        for (int x = 0; x < gridArray.GetLength(0); ++x) {
            for (int y = 0; y < gridArray.GetLength(1); ++y) {
                if(createGridObject != null) {
                    //if this instance was given a lambda expression (implying object data type), 
                    //then assign each grid to that lambda expression object definition
                    gridArray[x,y] = createGridObject();
                }

                if (showDebug) {
                    //here gridArray[x, y]?.ToString() checks if the value is null or defined before trying to call the ToString() method on it.
                    //Avoids throwing exceptions by checking first. Much nicer than regular try catch exception handling.
                    debugTextArray[x, y] = CreateWorldText(gridArray[x, y]?.ToString(), null, GetWorldPosition(x, y) + new Vector3(tileSize / 2, tileSize / 2), 
                                                           20, Color.white, TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);

                    if (createGridObject != null) {
                        //OnGridObjectChanged += denotes adding delegates to the event handler
                        //TODO see if I worded that correctly or if I'm misunderstanding addivite event handling
                        OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) 
                        => { debugTextArray[eventArgs.x, eventArgs.y].text = gridArray[eventArgs.x, eventArgs.y]?.ToString(); };
                        }
                    }
                }
            }

        if(showDebug) {
            Debug.DrawLine(GetWorldPosition(0,height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
        }
    }

    /*
    constructing this class using a primitive type while needing to see debug values 
    requires annoying pointless lambda expressions for primitive data type initialization.
    Which is why the constructor is overloaded to avoid this issue.
    */
    //TODO see how c# determines constructor calls given ambiguous parameters (i.e. only giving width, height has both constructor calls be valid)
    //      Note: this TODO isn't super important as either constructor call will correctly define the object properties

    /// <summary>Constructing a grid only requires the number of rows and columns. constant tileSize recommended.</summary>
    /// <param name="width">(int) The number of rows</param>
    /// <param name="height">(int) The number of columns</param>
    /// <param name="tileSize">(float) The scale of each tile</param>
    /// <param name="originPosition">(Vector3) The offset for the grid position</param>
    /// <param name="showDebug">(Bool) Set to true if you want to see the data value displayed in the center of each tile. For objects, display whatever you want by overridding the ToString() method in the class definition of your object type</param>
    public Grid(int width, int height, float tileSize = 10f, Vector3? originPosition = null, bool showDebug = false) : this(width, height, tileSize, originPosition, null, showDebug){}

    protected Vector3 GetWorldPosition(int x, int y) {
        return new Vector3(x,y) * tileSize + originPosition;
    }

    //out keyword denotes returning multiple values. Much nicer than passing by reference in parameter arguements in C++
    //TODO see if this is the same as c++ pass by reference or if it's copying *from* the arguement AND *to* the argument.
    protected void GetGridPosition(Vector3 worldPosition, out int x, out int y) {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / tileSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / tileSize);
    }

    public bool SetData(int x, int y, DataType data) {
        if (x >= 0 && y >= 0 && x < width && y < height) {
            gridArray[x,y] = data;
            //have the event handler trigger on successful assignment of new data
            if (OnGridObjectChanged != null) OnGridObjectChanged(this, new OnGridObjectChangedEventArgs {x = x, y = y});
            if (showDebug)
                debugTextArray[x,y].text = gridArray[x,y]?.ToString();
            return true;
        }
        else if (showDebug)
            Debug.LogWarning("trying to set " + data + " to invalid position: (" + x + ", " + y + ")");
        
        return false;
    }

    public void SetData(Vector3 worldPosition, DataType data) {
        int x, y;
        //assign x and y to the values returned by GetGridPosition()
        GetGridPosition(worldPosition, out x, out y);
        //Not as obvious but the if statement performs and checks if setting data was successful
        if (SetData(x,y,data) == false && showDebug) {
            Debug.LogWarning("trying to set " + data + " to invalid world position: (" 
            + worldPosition.x + ", " + worldPosition.y + ")");
        }
    }

    //Public function to update the event handler given an x, y cell coordinate
    //See ItemGridObject setState() to see where this is used and understand better why we have weird referencing properties in each object.
    public void TriggerGridObjectChanged(int x, int y) {
        if(OnGridObjectChanged != null) OnGridObjectChanged(this, new OnGridObjectChangedEventArgs {x = x, y = y});
    }
    public DataType GetData(int x, int y) {
        if (x >= 0 && y >= 0 && x < width && y < height)
            return gridArray[x,y];
        else {
            if (showDebug) 
                Debug.LogWarning("trying to get data at invalid position: (" + x + ", " + y + ")");
            return default(DataType);
        }
    }

    public DataType GetData(Vector3 worldPosition) {
        int x, y;
        GetGridPosition(worldPosition, out x, out y);
        return GetData(x, y);
    }


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