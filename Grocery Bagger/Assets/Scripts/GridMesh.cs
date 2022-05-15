// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// //Ensures that the GridMesh object cannot be instantiated without a MeshFilter.
// //because a Mesh is a property of the MeshFilter. NOTE: Mesh Filters also require a MeshRenderer for them to function in game.
// /// <summary>An object containing a grid which maps custom mesh to it.</summary>
// [RequireComponent(typeof(MeshFilter))] 
// public class GridMesh<DataType> {
//     //TODO finish implmenting this
//         //current goal of this class is to use it as a way to visually show each tile's state via a color.
//     private Grid<DataType> grid;
//     private UnityEngine.Mesh mesh;
//     private Vector3[] vertices;
//     private Vector2[] uv;
//     private int[] triangles;

//     public GridMesh(Grid<DataType> grid, MeshFilter meshFilter) {
//         this.grid = grid;
//         mesh = new UnityEngine.Mesh();
//         meshFilter.mesh = mesh;

//         UpdateGridMesh();
//         grid.OnGridObjectChanged += Grid_OnGridValueChanged;
//     }

//     private void Grid_OnGridValueChanged(object sender, System.EventArgs e) { UpdateGridMesh(); }

//     public void UpdateGridMesh () {
//         CreateEmptyMeshArrays(grid.Width, grid.Height, grid.TileSize, out vertices, out uv, out triangles);
//         mesh.vertices = vertices;
//         mesh.uv = uv;
//         mesh.triangles = triangles;

//         for (int x = 0; x < grid.Width; x++)
//         {
//             for (int y = 0; y < grid.Height; y++)
//             {
//                 int index = x * grid.Height + y;
//                 Vector3 squareSize = new Vector3(1,1) * grid.TileSize;
//                 DataType gridObject = grid.GetData(x,y);
//                 float gridValueNormalized = 0f;
//                 //float gridValueNormalized = gridObject.GetValueNormalized();
//                 Vector2 gridValueUV = new Vector2(1f,0f);

//                 AddToMeshArrays(vertices, uv, triangles, index, 
//                                 grid.GetWorldPosition(x,y) + squareSize * 0.5f, 0f, squareSize, gridValueUV, gridValueNormalized);
//             }
//         }

//         //TODO finish this function to update the mesh array instead of assigning it empty ones.

//         //Have Unity automatically recalculate lighting on this mesh
//         //TODO see if this is inefficient (i.e. is updating the lighting for the entire mesh unnecessary when making small updates?)
//         mesh.RecalculateNormals();
//     }

//     public void CreateEmptyMeshArrays(int width, int height, float tileSize, out Vector3[] vertices, out Vector2[] uv, out int[] triangles) {
//         vertices = new Vector3[4 * (width * height)];
//         uv = new Vector2[4 * (width * height)];
//         triangles = new int[6 * (width * height)];

//         for (int i = 0; i < width; i++) {
//             for (int j = 0; j < height; j++) {
//                 int index = i * height + j;

//                 //define the four vertices for our square
//                 vertices[0 + index * 4] = new Vector3(tileSize * i, tileSize * j);
//                 vertices[1 + index * 4] = new Vector3(tileSize * i, (tileSize + 1) * j);
//                 vertices[2 + index * 4] = new Vector3((tileSize + 1) * i, (tileSize + 1) * j);
//                 vertices[3 + index * 4] = new Vector3((tileSize + 1) * i, tileSize * j);

//                 //uv map of the square in order of vertex definition
//                 uv[0 + index * 4] = new Vector2(0, 0);
//                 uv[1 + index * 4] = new Vector2(0, 1);
//                 uv[2 + index * 4] = new Vector2(1, 1);
//                 uv[3 + index * 4] = new Vector2(1, 0);

//                 //Define the two triangles that form the square
//                 triangles[0 + index * 6] = 0 + index * 4;
//                 triangles[1 + index * 6] = 1 + index * 4;
//                 triangles[2 + index * 6] = 2 + index * 4;
//                 //Order is (1,3,2) for the second triangle of the square so that it is drawn clockwise (the way unity draws triangles)
//                 triangles[3 + index * 6] = 1 + index * 4;
//                 triangles[4 + index * 6] = 3 + index * 4;
//                 triangles[5 + index * 6] = 2 + index * 4;
//             }
//         }
//     }

//     public void AddToMeshArrays(Vector3[] vertices, Vector2[] uv, int[] triangles, int index, 
//                                 Vector3 position, float opacity, Vector3 squareSize, Vector2 gridValueUV, float gridValueNormalized) {
//         //TODO finish this function
//     }

// }