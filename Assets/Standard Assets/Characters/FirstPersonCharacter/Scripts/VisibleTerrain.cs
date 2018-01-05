using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainClass {
    using Data = Dictionary<string, string>;
    public struct CubeInfo
    {
       public Vector3 pos;
       public Material m;
    }

    public class VisibleTerrain {
		static int chunkSize = 24;
		static double maxHeight = 10.0;
		Chunk[,] chunks = new Chunk[3, 3];
		Queue<Chunk> inactiveChunks = new Queue<Chunk>();
		static int seedLength = 100;
		static int[,] seeds = new int[seedLength, seedLength];
		static double[,] perlinBiomes = new double[seedLength, seedLength];

		static Material snowMat = null;
		static Material grassMat = null;
		static Material sandMat= null;
		static Material rockMat = null;

		static Material whiteMat = null;
		static Material yellowMat = null;
		static Material greenMat= null;
		static Material blueMat = null;
		static Material purpleMat = null;
		int _heightSeed;
		int _biomeSeed;

		private static VisibleTerrain _instance;

		public static VisibleTerrain getInstance()
		{
			if(_instance !=null )
			{
				return _instance;
			} else
			{
				return null;
			}
		}


		public VisibleTerrain(int x, int z)
		{
			_heightSeed = UnityEngine.Random.Range (0,1000000);
			_biomeSeed = UnityEngine.Random.Range (0,1000000);
			generateSeeds();
			perlinBiomes = PerlinGenerator.generatePerlin(seedLength, seedLength, _biomeSeed);
			loadMaterials();
			createInactiveChunks(x, z);
			createChunks(x, z);
			_instance = this;
		}

		public VisibleTerrain(int x, int z, int heightSeed, int biomeSeed)
		{
			_heightSeed = heightSeed;
			_biomeSeed = biomeSeed;
			generateSeeds();
			perlinBiomes = PerlinGenerator.generatePerlin(seedLength, seedLength, _biomeSeed);
			loadMaterials();
			createInactiveChunks(x, z);
			createChunks(x, z);
			_instance = this;
		}

		void createInactiveChunks(int x, int z)
		{
			inactiveChunks.Enqueue(new Chunk(x - chunkSize, z - chunkSize));
			inactiveChunks.Enqueue(new Chunk(x, z - chunkSize));
			inactiveChunks.Enqueue(new Chunk(x + chunkSize, z - chunkSize));
			inactiveChunks.Enqueue(new Chunk(x - chunkSize, z));
			inactiveChunks.Enqueue(new Chunk(x, z));
			inactiveChunks.Enqueue(new Chunk(x + chunkSize, z));
			inactiveChunks.Enqueue(new Chunk(x - chunkSize, z + chunkSize));
			inactiveChunks.Enqueue(new Chunk(x, z + chunkSize));
			inactiveChunks.Enqueue(new Chunk(x + chunkSize, z + chunkSize));
			inactiveChunks.Enqueue(new Chunk(x - chunkSize * 2, z - chunkSize * 2));
			inactiveChunks.Enqueue(new Chunk(x, z - chunkSize * 2));
			inactiveChunks.Enqueue(new Chunk(x + chunkSize * 2, z - chunkSize * 2));
		}

		void createChunks(int x, int z)
		{
			chunks[0, 0] = inactiveChunks.Dequeue();
			chunks[0, 0].setActive(x - chunkSize, z - chunkSize);
			chunks[0, 1] = inactiveChunks.Dequeue();
			chunks[0, 1].setActive(x, z - chunkSize);
			chunks[0, 2] = inactiveChunks.Dequeue();
			chunks[0, 2].setActive(x + chunkSize, z - chunkSize);
			chunks[1, 0] = inactiveChunks.Dequeue();
			chunks[1, 0].setActive(x - chunkSize, z);
			chunks[1, 1] = inactiveChunks.Dequeue();
			chunks[1, 1].setActive(x, z);
			chunks[1, 2] = inactiveChunks.Dequeue();
			chunks[1, 2].setActive(x + chunkSize, z);
			chunks[2, 0] = inactiveChunks.Dequeue();
			chunks[2, 0].setActive(x - chunkSize, z + chunkSize);
			chunks[2, 1] = inactiveChunks.Dequeue();
			chunks[2, 1].setActive(x, z + chunkSize);
			chunks[2, 2] = inactiveChunks.Dequeue();
			chunks[2, 2].setActive(x + chunkSize, z + chunkSize);
		}


		void loadMaterials() {
			snowMat = (Material)Resources.Load ("SnowMat", typeof(Material));
			grassMat = (Material)Resources.Load ("GrassMat", typeof(Material));
			sandMat = (Material)Resources.Load ("SandMat", typeof(Material));
			rockMat = (Material)Resources.Load ("BlackMat", typeof(Material));

			whiteMat = (Material)Resources.Load ("WhiteMat", typeof(Material));
			yellowMat = (Material)Resources.Load ("YellowMat", typeof(Material));
			greenMat = (Material)Resources.Load ("GreenMat", typeof(Material));
			blueMat = (Material)Resources.Load ("CyanMat", typeof(Material));
			purpleMat = (Material)Resources.Load ("PinkMat", typeof(Material));

		}

		void generateSeeds() {
            UnityEngine.Random.InitState (_heightSeed);
			for (int i = 0; i < seedLength; i++) {
				for (int j = 0; j < seedLength; j++) {
					seeds[i,j] = ((int)UnityEngine.Random.Range(1,10000));
				}
			}
		}

		public void updateGrid(float x, float z)
		{
			Vector2 p = new Vector2(x, z);

			//check if we are still in current chunk
			if (chunks[1, 1].inBounds(p))
			{
				return;
			}

			//go through chunks to see which chunk we are in
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					if (!(i == 1 && j == 1))
					{
						if (chunks[i, j].inBounds(p))
						{
							//Debug.Log ("Moving to grid " + i + ", " + j);
							moveGrid(i, j);
							return;
						}
					}
				}
			}
		}

		void moveGrid(int x, int z)
		{
			Chunk[,] newChunks = new Chunk[3, 3];
			bool[,] keepChunks = new bool[3, 3];

			int newCenterX = chunks[x, z].getX();
			int newCenterZ = chunks[x, z].getZ();

			//move appropriate chunks to appropriate new spots
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					int newX = i + (1 - x);
					int newZ = j + (1 - z);
					if (newZ >= 0 && newZ <= 2 && newX >= 0 && newX <= 2)
					{
						newChunks[newX, newZ] = chunks[i, j];
						keepChunks[i, j] = true;
					}
					else
					{
						keepChunks[i, j] = false;
					}
				}
			}


			//delete old chunks
			//move appropriate chunks to appropriate new spots
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					if (keepChunks[i, j] == false)
					{
						chunks[i, j].setInactive();
						inactiveChunks.Enqueue(chunks[i, j]);
						chunks[i, j] = null;
					}
					if (newChunks[i, j] != null)
					{
						chunks[i, j] = newChunks[i, j];
					}
					else
					{
						chunks[i, j] = inactiveChunks.Dequeue();
						chunks[i, j].setActive(newCenterX + (j - 1) * chunkSize, newCenterZ + (i - 1) * chunkSize);
					}
				}
			}
		}


		public void deleteCube(Vector3 pos)
		{
			float chunkx = (pos.x - 0.5f) % chunkSize;
			float chunkz = (pos.z - 0.5f) % chunkSize;

			if(pos.x < 0)
			{
				chunkx = chunkSize - 0.5f + ((pos.x) % chunkSize);
			}

			if(pos.z < 0)
			{
				chunkz = chunkSize - 0.5f + ((pos.z) % chunkSize);
			}

			if (chunks[1,1].inBounds(new Vector2(pos.x, pos.z)))
			{
				Debug.Log("inbounds");
				chunks[1, 1].deleteCube(chunkx, pos.y - 0.5f,  chunkz);
			} 
			else
			{
				//go through chunks to see which chunk we are in
				for (int i = 0; i < 3; i++)
				{
					for (int j = 0; j < 3; j++)
					{
						if (!(i == 1 && j == 1))
						{
							if (chunks[i, j].inBounds(new Vector2(pos.x, pos.z)))
							{
								Debug.Log("Outbounds");
								chunks[i, j].deleteCube(chunkx, pos.y - 0.5f, chunkz);
								return;
							}
						}
					}
				}
			}
		}

        public void deleteCube(Vector3 pos, Action callback)
		{
			float chunkx = (pos.x - 0.5f) % chunkSize;
			float chunkz = (pos.z - 0.5f) % chunkSize;

			if (pos.x < 0) {
				chunkx = chunkSize - 0.5f + ((pos.x) % chunkSize);
			}

			if (pos.z < 0) {
				chunkz = chunkSize - 0.5f + ((pos.z) % chunkSize);
			}

			if (chunks [1, 1].inBounds (new Vector2 (pos.x, pos.z))) {
				Debug.Log ("inbounds");
				chunks [1, 1].deleteCube (chunkx, pos.y - 0.5f, chunkz);
			} else {
				//go through chunks to see which chunk we are in
				for (int i = 0; i < 3; i++) {
					for (int j = 0; j < 3; j++) {
						if (!(i == 1 && j == 1)) {
							if (chunks [i, j].inBounds (new Vector2 (pos.x, pos.z))) {
								Debug.Log ("Outbounds");
								chunks [i, j].deleteCube (chunkx, pos.y - 0.5f, chunkz);
								callback ();
								return;
							}
						}
					}
				}
			}
		}

		public void addCube(Vector3 pos, Material m)
		{
			Debug.Log("New pos: " + pos);
			float chunkx = (pos.x - 0.5f) % chunkSize;
			float chunkz = (pos.z - 0.5f) % chunkSize;

			if (pos.x < 0)
			{
				chunkx = chunkSize - 0.5f + ((pos.x) % chunkSize);
			}

			if (pos.z < 0)
			{
				chunkz = chunkSize - 0.5f + ((pos.z) % chunkSize);
			}

			if (chunks[1, 1].inBounds(new Vector2(pos.x, pos.z)))
			{
				Debug.Log("inbounds");
				chunks[1, 1].addCube(chunkx, pos.y - 0.5f, chunkz, pos.x, pos.y, pos.z, m);
			}
			else
			{
				//go through chunks to see which chunk we are in
				for (int i = 0; i < 3; i++)
				{
					for (int j = 0; j < 3; j++)
					{
						if (!(i == 1 && j == 1))
						{
							if (chunks[i, j].inBounds(new Vector2(pos.x, pos.z)))
							{
								Debug.Log("Outbounds");
								chunks[i, j].addCube(chunkx, pos.y - 0.5f, chunkz, pos.x, pos.y, pos.z, m);
								return;
							}
						}
					}
				}
			}
		}

		public void addCube(Vector3 pos, Material m, Action callback)
		{
			Debug.Log("New pos: " + pos);
			float chunkx = (pos.x - 0.5f) % chunkSize;
			float chunkz = (pos.z - 0.5f) % chunkSize;

			if (pos.x < 0)
			{
				chunkx = chunkSize - 0.5f + ((pos.x) % chunkSize);
			}

			if (pos.z < 0)
			{
				chunkz = chunkSize - 0.5f + ((pos.z) % chunkSize);
			}

			if (chunks[1, 1].inBounds(new Vector2(pos.x, pos.z)))
			{
				Debug.Log("inbounds");
				chunks[1, 1].addCube(chunkx, pos.y - 0.5f, chunkz, pos.x, pos.y, pos.z, m);
			}
			else
			{
				//go through chunks to see which chunk we are in
				for (int i = 0; i < 3; i++)
				{
					for (int j = 0; j < 3; j++)
					{
						if (!(i == 1 && j == 1))
						{
							if (chunks[i, j].inBounds(new Vector2(pos.x, pos.z)))
							{
								Debug.Log("Outbounds");
								chunks[i, j].addCube(chunkx, pos.y - 0.5f, chunkz, pos.x, pos.y, pos.z, m);
								callback();
								return;
							}
						}
					}
				}
			}
		}


        class Chunk {
			int _x;
			int _z;
			int edgeBlend = 6; //8
			int biomeBlend = 10; //12

			List<GameObject>[,] cubes = new List<GameObject>[chunkSize, chunkSize];
			public static List<Vector3>[,] deletedCubes = new List<Vector3>[seedLength, seedLength];
            public static List<CubeInfo>[,] addedCubes = new List<CubeInfo>[seedLength, seedLength];

            GameObject TerrainObj;

			public Chunk(int x, int z)
			{
				_x = x;
				_z = z;
				createTerrain();
				createInactives();
				TerrainObj.SetActive(false);

				for(int i = 0; i < seedLength; i++)
				{
					for (int j = 0; j < seedLength; j++)
					{
						deletedCubes[i, j] = new List<Vector3>();
                        addedCubes[i,j] = new List<CubeInfo>();
					}
				}
			}

			public void setActive(int x, int z)
			{
				_x = x;
				_z = z;
				TerrainObj.SetActive(true);
				TerrainObj.transform.position = new Vector3(_x, 0, _z);
				makeCubes();

			}

			void createInactives()
			{
				for (int i = _x; i < _x + chunkSize; i++)
				{
					for (int j = _z; j < _z + chunkSize; j++)
					{
						List<GameObject> c = new List<GameObject>();
						for (int k = 0; k < maxHeight; k++)
						{
							GameObject curr_cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
							curr_cube.AddComponent<MeshCollider>();
							curr_cube.transform.position = new Vector3(i + 0.5f, k + 0.5F, j + 0.5f);
							curr_cube.SetActive(false);
							c.Add(curr_cube);
							//Debug.Log ("Cube at: " + i + ", " + k+0.5F + ", " + j);
						}
						cubes[j - _z, i - _x] = c;
					}
				}
			}

			void makeCubes()
			{
				double b = getBiomeValue(_x, _z);

				double[,] zVals = PerlinGenerator.generatePerlin(chunkSize, chunkSize, getSeedValue(_x, _z));
				double[,] aDown = PerlinGenerator.generatePerlin(chunkSize, chunkSize, getSeedValue(_x - chunkSize, _z));
				double[,] aRight = PerlinGenerator.generatePerlin(chunkSize, chunkSize, getSeedValue(_x, _z - chunkSize));
				double[,] aLeft = PerlinGenerator.generatePerlin(chunkSize, chunkSize, getSeedValue(_x, _z + chunkSize));
				double[,] aUp = PerlinGenerator.generatePerlin(chunkSize, chunkSize, getSeedValue(_x + chunkSize, _z));

				double[,] bDown = PerlinGenerator.generatePerlin(chunkSize, chunkSize, getSeedValue(_x - chunkSize, _z) + getSeedValue(_x, _z));
				double[,] bLeft = PerlinGenerator.generatePerlin(chunkSize, chunkSize, getSeedValue(_x, _z + chunkSize) + getSeedValue(_x, _z));
				double[,] bRight = PerlinGenerator.generatePerlin(chunkSize, chunkSize, getSeedValue(_x, _z - chunkSize) + getSeedValue(_x, _z));
				double[,] bUp = PerlinGenerator.generatePerlin(chunkSize, chunkSize, getSeedValue(_x + chunkSize, _z) + getSeedValue(_x, _z));

				double[,] biomeVals = new double[chunkSize, chunkSize];
				for (int i = 0; i < chunkSize; i++)
				{
					for (int j = 0; j < chunkSize; j++)
					{
						biomeVals[i, j] = b;
					}
				}

				int buffSpace = chunkSize / 2 - edgeBlend;

				//				//Down terrain
				for (int i = 0; i < edgeBlend; i++)
				{
					for (int j = 0; j < chunkSize; j++)
					{
						zVals[i, j] = (aDown[chunkSize - 1 - i, j] + zVals[i, j]) / 2.0;
					}
				}
				//down Biome
				for (int i = 0; i < biomeBlend; i++)
				{
					for (int j = 0; j < chunkSize; j++)
					{
						double p = bDown[i + buffSpace, j];
						biomeVals[i, j] = (getBiomeValue(_x - chunkSize, _z) * p) + (biomeVals[i, j] * (1.0 - p));
					}
				}


				//Left Terrain
				for (int i = 0; i < chunkSize; i++)
				{
					for (int j = chunkSize - edgeBlend; j < chunkSize; j++)
					{
						zVals[i, j] = (aLeft[i, chunkSize - 1 - j] + zVals[i, j]) / 2.0;
					}
				}
				//left Biome
				for (int i = 0; i < chunkSize; i++)
				{
					for (int j = chunkSize - edgeBlend; j < chunkSize; j++)
					{
						double p = bLeft[i, j - buffSpace];
						biomeVals[i, j] = (getBiomeValue(_x, _z + chunkSize) * p) + (biomeVals[i, j] * (1.0 - p));
					}
				}


				//Right Terrain
				for (int i = 0; i < chunkSize; i++)
				{
					for (int j = 0; j < edgeBlend; j++)
					{
						zVals[i, j] = (aRight[i, chunkSize - 1 - j] + zVals[i, j]) / 2.0;
					}
				}
				//Right Biome
				for (int i = 0; i < chunkSize; i++)
				{
					for (int j = 0; j < edgeBlend; j++)
					{
						double p = bRight[i, j + buffSpace];
						biomeVals[i, j] = (getBiomeValue(_x, _z - chunkSize) * p) + (biomeVals[i, j] * (1.0 - p));
					}
				}


				//Up Terrain
				for (int i = chunkSize - edgeBlend; i < chunkSize; i++)
				{
					for (int j = 0; j < chunkSize; j++)
					{
						zVals[i, j] = (aUp[chunkSize - 1 - i, j] + zVals[i, j]) / 2.0;
					}
				}
				//Up Biome
				for (int i = chunkSize - edgeBlend; i < chunkSize; i++)
				{
					for (int j = 0; j < chunkSize; j++)
					{
						double p = bUp[i - buffSpace, j];
						biomeVals[i, j] = (getBiomeValue(_x + chunkSize, _z) * p) + (biomeVals[i, j] * (1.0 - p));
					}
				}

				for (int i = _x; i < _x + chunkSize; i++)
				{
					for (int j = _z; j < _z + chunkSize; j++)
					{
						int height = (int)(zVals[i - _x, j - _z] * maxHeight);
						for (int k = 0; k < height; k++)
						{
							GameObject curr_cube = cubes[i - _x, j - _z][k];
							curr_cube.transform.position = new Vector3(i + 0.5f, k + 0.5F, j + 0.5f);
							curr_cube.GetComponent<Renderer>().material = getBiome(biomeVals[i - _x, j - _z]);
							curr_cube.SetActive(true);
						}
					}
				}
                addCubes();

                deleteCubes();
			}

			void deleteCubes()
			{
				if (deletedCubes[getVal(_x), getVal(_z)].Count != 0)
				{
					List<Vector3> list = deletedCubes[getVal(_x), getVal(_z)];
					foreach (Vector3 v in list)
					{
						GameObject c = cubes[(int)v.x, (int)v.z][(int)v.y];
                        c.SetActive(false);
					}
				}
			}

            void addCubes()
            {
                if (addedCubes[getVal(_x), getVal(_z)].Count != 0)
                {
                     List<CubeInfo> list = addedCubes[getVal(_x), getVal(_z)];
                    foreach (CubeInfo v in list)
                    {
                        GameObject c = cubes[(int)v.pos.x, (int)v.pos.z][(int)v.pos.y];
                        c.transform.position = new Vector3(_x+v.pos.x + 0.5f, v.pos.y + 0.5f, _z+v.pos.z + 0.5f);
                        c.GetComponent<Renderer>().material = v.m;
                        c.SetActive(true);
                    }
                }
            }

			public void addCube(float x, float y, float z, float wx, float wy, float wz, Material m)
            {
                Debug.Log("adding at " + x + " y: " + y + " z: " + z);
                cubes[(int)x, (int)z][(int)y].transform.position = new Vector3(wx,wy,wz);

                cubes[(int)x, (int)z][(int)y].SetActive(true);
                cubes[(int)x, (int)z][(int)y].GetComponent<Renderer>().material = m;
                CubeInfo info = new CubeInfo();
                info.pos = new Vector3(x, y, z);
                info.m = cubes[(int)x, (int)z][(int)y].GetComponent<Renderer>().material;
                addedCubes[getVal(_x), getVal(_z)].Add(info);

            }

            public void deleteCube(float x, float y, float z)
			{
				if(cubes[(int)x, (int)z][(int)y].GetComponent<Renderer>().material == blueMat)
				{
					return;
				}
				Debug.Log("Delteting at " + x + " y: " + y + " z: " + z);
                cubes[(int)x, (int)z][(int)y].SetActive(false);
				deletedCubes[getVal(_x), getVal(_z)].Add(new Vector3(x,y,z));
			}

            public static int getVal(int x) {
				int val = (x / chunkSize) % seedLength;

				if (val < 0) {
					val = seedLength + val;
				}

				return val;
			}

			//			Material getBiome(double b) {
			//				if (b > 0.9) {
			//					return whiteMat;
			//				} else if (b > 0.7) {
			//					return yellowMat;
			//				} else if (b > 0.5) {
			//					return greenMat;
			//				} else if (b > 0.3) {
			//					return blueMat;
			//				}
			//				else {
			//					return purpleMat;
			//				}
			//			}
			//
			Material getBiome(double b) {
				if (b > 0.7) {
					return snowMat;
				} else if (b > 0.3) {
					return grassMat;
				} else {
					return sandMat;
				}
			}

			double getBiomeValue(int x, int z){
				int xval = (x / chunkSize) % seedLength;
				int yval = (z / chunkSize) % seedLength;

				if (xval < 0) {
					xval = seedLength + xval;
				}
				if (yval < 0) {
					yval = seedLength + yval;
				}

				return perlinBiomes [xval, yval];
			}

			int getSeedValue(int x, int z){
				int xval = (x / chunkSize) % seedLength;
				int yval = (z / chunkSize) % seedLength;

				if (xval < 0) {
					xval = seedLength + xval;
				}
				if (yval < 0) {
					yval = seedLength + yval;
				}

				return seeds [xval, yval];
			}

			public void setInactive()
			{
				for (int i = 0; i < chunkSize; i++)
				{
					for (int j = 0; j < chunkSize; j++)
					{
						foreach (GameObject c in cubes[i, j])
						{
							c.SetActive(false);
						}
						TerrainObj.SetActive(false);
					}
				}
			}


			public void destroyChunk(){
				for (int i = 0; i < chunkSize; i++) {
					for (int j = 0; j < chunkSize; j++) {
						foreach(GameObject c in cubes[i,j]){
							GameObject.Destroy(c.gameObject);
						}
						GameObject.Destroy (TerrainObj.gameObject);
					}
				}
			}

			public void changeMat(Material m){
				for (int i = 0; i < chunkSize; i++) {
					for (int j = 0; j < chunkSize; j++) {
						foreach(GameObject c in cubes[i,j]){
							c.GetComponent<Renderer> ().material = m;
						}
					}
				}
			}

			void createTerrain(){
				TerrainData tData = new TerrainData();
				tData.size = new Vector3(chunkSize,0.5f,chunkSize);
				TerrainObj = Terrain.CreateTerrainGameObject(tData);
				TerrainObj.transform.position = new Vector3(_x, 0, _z);
				//				TerrainObj.AddComponent<Renderer> ();
				//TerrainObj.GetComponent<Renderer>().material = grassMat;
			}

            public bool inBounds(Vector2 p)
            {
                //splti into trinagle
                Vector2 a = new Vector2(_x, _z);
                Vector2 b = new Vector2(_x + chunkSize, _z);
                Vector2 c = new Vector2(_x + chunkSize, _z + chunkSize);
                Vector2 d = new Vector2(_x, _z + chunkSize);

                return (inOutTest(a, b, c, p) || inOutTest(a, d, c, p));
            }

            void printTerrain() {
				//Debug.Log ("Terrain at: " + _x + ", " + _z);
			}

			public int getX(){
				return _x;
			}
			public int getZ(){
				return _z;
			}
		}

        public static bool inOutTest(Vector2 a, Vector2 b, Vector2 c, Vector2 p)
        {
            float ap_x = p.x - a.x;
            float ap_y = p.y - a.y;

            bool p_ab = (b.x - a.x) * ap_y - (b.y - a.y) * ap_x > 0;

            if ((c.x - a.x) * ap_y - (c.y - a.y) * ap_x > 0 == p_ab) return false;

            if ((c.x - b.x) * (p.y - b.y) - (c.y - b.y) * (p.x - b.x) > 0 != p_ab) return false;

            return true;
        }



    }
}

