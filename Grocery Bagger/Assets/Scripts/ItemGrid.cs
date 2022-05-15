//easier to handle everything within the grid class itself. While it was nice to abstract the specific use case functionality, I'm not using the
//grid for any other purpose for this project so this is just an unnecessary layer of abstraction.

// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// /// <summary>A class for handling item interactions on a grid</summary>
// public class ItemGrid<ItemGridObject> : Grid<ItemGridObject> {    
//     //The only difference between the base constructor and this constructor is the different parameter types for creating a grid object
//     //because items are being updated automatically using event handlers which requires this awkward reference handling
//     public ItemGrid(int width, int height, float tileSize = 10f, Vector3? originPosition = null, 
//     Func<Grid<ItemGridObject>, int, int, ItemGridObject> createGridObject = null, bool showDebug = false) 
//     : base(width, height, tileSize, originPosition, null, showDebug) {
//         for (int x = 0; x < gridArray.GetLength(0); ++x) {
//             for (int y = 0; y < gridArray.GetLength(1); ++y) {
//                 if(createGridObject != null)
//                     gridArray[x,y] = createGridObject(this, x, y);

//                 if (showDebug) {
//                     debugTextArray[x, y] = CreateWorldText(gridArray[x, y]?.ToString(), null, GetWorldPosition(x, y) + new Vector3(tileSize / 2, tileSize / 2), 
//                                                            20, Color.white, TextAnchor.MiddleCenter);
//                     Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
//                     Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);

//                     if (createGridObject != null) {
//                         OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs)
//                         => { debugTextArray[eventArgs.x, eventArgs.y].text = gridArray[eventArgs.x, eventArgs.y]?.ToString(); };
//                     }
//                 }
//             }
//         }
//     }
// }