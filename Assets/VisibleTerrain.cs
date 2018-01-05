//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//
//namespace TerrainClass {
//	public class VisibleTerrain : MonoBehaviour {
//		int chunkSize = 8;
//		Chunk[,] chunks = new Chunk[3, 3];
//
//		public VisibleTerrain() {
//			updateChunks ();
//		}
//
//		void updateChunks(){
//			int x = 0;
//			int y = 0;
//
//			chunks [0, 0] = new Chunk (x - chunkSize, y - chunkSize);
//			chunks[0,1] = new Chunk (x, y - chunkSize);
//			chunks[0,2] = new Chunk (x + chunkSize, y - chunkSize);
//			chunks[1,0] = new Chunk (x - chunkSize, y);
//			chunks[1,1] = new Chunk (x, y);
//			chunks[1,2] = new Chunk (x + chunkSize, y);
//			chunks[2,0] = new Chunk (x - chunkSize, y + chunkSize);
//			chunks[2,1] = new Chunk (x, y + chunkSize);
//			chunks[2,2] = new Chunk (x + chunkSize, y + chunkSize);
//
//		}
//
//		class Chunk: MonoBehaviour {
//			int _x;
//			int _y;
//			int chunkSize = 8;
//
//			List<GameObject> cubes = new List<GameObject> ();
//			GameObject TerrainObj;
//
//			public Chunk(int x, int y) {
//				_x = x;
//				_y = y;
//				createTerrain();
//				//makeCubes();
//			}
//
//			void makeCubes() {
//				Material newMat = (Material)Resources.Load ("RedMat", typeof(Material));
//
//				int[,] yVals = PerlinGenerator.generatePerlin (chunkSize, chunkSize);
//
//				for (int i = _x; i < _x + chunkSize; i++) {
//					for (int j = _y; j < _y + chunkSize; j++) {
//						for (int k = 0; k < yVals [i, j]; k++) {
//							GameObject curr_cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
//							Rigidbody rb = curr_cube.AddComponent<Rigidbody> ();
//							rb.mass = 10000;
//							rb.drag = 1000;
//							rb.angularDrag = 1000;
//							//curr_cube.AddComponent<BoxCollider>();
//							curr_cube.transform.position = new Vector3 (i, k + 0.5F, j);
//							curr_cube.GetComponent<Renderer> ().material = newMat;
//							cubes.Add (curr_cube);
//						}
//					}
//				}
//			}
//
//			void createTerrain(){
//				TerrainObj = new GameObject("TerrainObj");
//
//				TerrainData _TerrainData = new TerrainData();
//
//				_TerrainData.size = new Vector3(_x, 0, _y);
//				_TerrainData.heightmapResolution = chunkSize;
//				_TerrainData.baseMapResolution = chunkSize;
//				//_TerrainData.SetDetailResolution(1024, 16);
//
//				int _heightmapWidth = _TerrainData.heightmapWidth;
//				int _heightmapHeight = _TerrainData.heightmapHeight;
//
//				TerrainCollider _TerrainCollider = TerrainObj.AddComponent<TerrainCollider>();
//				Terrain _Terrain2 = TerrainObj.AddComponent<Terrain>();
//
//				_TerrainCollider.terrainData = _TerrainData;
//				_Terrain2.terrainData = _TerrainData;
//			}
//		}
//
//	}
//}
//
