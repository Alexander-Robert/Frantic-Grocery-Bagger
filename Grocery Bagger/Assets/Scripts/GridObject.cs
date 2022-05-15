using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject {
        public PlacedObject placedObject;
        //References the grid in each object so that it can trigger events each time we change the state
        private Grid<GridObject> grid;
        private int x,y; //additional required parameters for triggering the events
        //Therefore, we need to be able to construct an ItemGridObject with these references and required parameters
        public GridObject(Grid<GridObject> grid, int x, int y) {
            this.grid = grid;
            this.x = x;
            this.y = y;
            placedObject = null;
        }

        public override string ToString() {
            return x + ", " + y + "\n" + placedObject;
        }

        public void TriggerGridObjectChanged() {
            grid.TriggerGridObjectChanged(x,y);
        }
        public void SetPlacedObject(PlacedObject placedObject) {
            this.placedObject = placedObject;
            grid.TriggerGridObjectChanged(x, y);
        }

        public void ClearPlacedObject() {
            placedObject = null;
            grid.TriggerGridObjectChanged(x, y);
        }

        public PlacedObject GetPlacedObject() {
            return placedObject;
        }

        public bool CanBuild() {
            return placedObject == null;
        }

        public bool HasPlacedObject() {
            return placedObject != null;
        }

    }