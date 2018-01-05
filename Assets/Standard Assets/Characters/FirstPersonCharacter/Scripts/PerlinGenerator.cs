using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinGenerator : MonoBehaviour {
	static int r = 0;
	static int c = 0;
	static int zoomFactor = 5;
	static int _seed = 0;

	public static double[,] generatePerlin(int rows, int cols, int seed) {
		double[,] noise = new double[rows,cols];
		r = rows;
		c = cols;
		_seed = seed;

		generateNoise(noise);

		return smoothPerlin(noise);
	}

	static void generateNoise(double[,] noise) {
		Random.InitState (_seed);
		for (int y = 0; y < r; y++)
			for (int x = 0; x < c; x++)
			{
				float f = Random.Range (0f, 1f);
				noise[y,x] = (double) f;
			}
	}

	static double[,] smoothPerlin(double[,] noise) {
		double[,] finalNoise = new double[r,c];

		int denom = 0;
		for (int i = 0; i < zoomFactor; i++) {
			int z = (int)Mathf.Pow(2,i);
			for (int x = 0; x < r; x++) {
				for (int y = 0; y < c; y++) {
					finalNoise[x,y] += smoothNoise(noise,x/z,y/z) * i;
				}
			}
			denom += i;
		}
		for (int x = 0; x < r; x++) {
			for (int y = 0; y < c; y++) {
				finalNoise[x,y] = finalNoise[x,y]/denom;
			}
		}
		return finalNoise;
	}

	static double smoothNoise(double[,] noise, double x, double y)
	{
		//get fractional part of x and y
		double fractX = x - (int)x;
		double fractY = y - (int)y;

		//wrap around
		int x1 = ((int)x + r) % r;
		int y1 = ((int)y + c) % c;

		//neighbor values
		int x2 = (x1 + r - 1) % r;
		int y2 = (y1 + c - 1) % c;

		//smooth the noise with bilinear interpolation
		double value = 0.0f;
		value += fractX     * fractY     * noise[y1,x1];
		value += (1 - fractX) * fractY     * noise[y1,x2];
		value += fractX     * (1 - fractY) * noise[y2,x1];
		value += (1 - fractX) * (1 - fractY) * noise[y2,x2];

		return value;
	}

}
