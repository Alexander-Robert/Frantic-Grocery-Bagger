using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//State types need to be public and accessible across many classes to check if types are equal
public enum State {Empty, Used, HoverEmpty, HoverUsed}

/// <summary>Object to handle the current state of each grid tile in relation to item placement</summary>
public class ItemGridObject {
    public PlacedItem placedItem;
    private State state; //default state is empty
    private Color tileColor,
                 empty = Color.white, 
                 used = Color.clear, 
                 hoverEmpty = Color.yellow, 
                 hoverUsed = Color.red;

    //References the grid in each object so that it can trigger events each time we change the state
    private Grid<ItemGridObject> grid;
    private int x,y; //additional required parameters for triggering the events

    //Therefore, we need to be able to construct an ItemGridObject with these references and required parameters
    public ItemGridObject(Grid<ItemGridObject> grid, int x, int y) {
        this.grid = grid;
        this.x = x;
        this.y = y;
    }

    public ItemGridObject(State state) {this.state = state; setColor();}
    public ItemGridObject() : this(State.Empty) {}

    public PlacedItem GetItem() {return placedItem;}
    public void SetItem(PlacedItem placedItem) {
        this.placedItem = placedItem;
        this.state = State.Used;
    }
    public void ClearItem() {
        this.placedItem = null;
        this.state = State.Empty;
        if (grid != null) {
            grid.TriggerGridObjectChanged(x,y);
        }    
    }

    public void TriggerGridObjectChanged() {
            grid.TriggerGridObjectChanged(x, y);
    }

    public void setState(State state) {
        this.state = state; 
        //every time the state of the object is changed, trigger the grid event to update the object's state
        if (grid != null)  {
            grid.TriggerGridObjectChanged(x,y); //this is why we need the reference to the grid from within each object on the grid
        }
    } 
    public State getState() {return this.state;}

    public bool canPlace() {return this.state == State.Empty;}

    public Color getColor() {return tileColor;}

    //overloaded SetColor function overrides the color of the current state
    private void SetColor(Color color) {tileColor = color;}

    ///<summary>sets the color according to the current state</summary>
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

    public override string ToString()
    {
        //accessing the explicit name as a string for the enumeration type State given this object's current state
        return Enum.GetName(typeof(State), state);
    }
}