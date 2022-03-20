using System;
using UnityEngine;

public static class BoxMuller
{
	public static float Generate(float mu, float sigma)
    {
		const float two_pi = 2*Mathf.PI;

		//create two random numbers, make sure u1 is greater than epsilon
		float u1, u2;
		do
		{
			u1 = UnityEngine.Random.Range(0.0f, 1.0f);
			u2 = UnityEngine.Random.Range(0.0f, 1.0f);
		} while (u1 <= float.Epsilon);

		//compute z0 and z1
		var mag = sigma * Mathf.Sqrt(-2.0f * Mathf.Log(u1));
		var z0 = mag * Mathf.Cos(two_pi * u2) + mu;
		//var z1 = mag * Math.Sin(two_pi * u2) + mu;

		return z0;
	}

	public static Tuple<float, float> Generate2D(float mu, float sigma)
	{
		const float two_pi = 2f * Mathf.PI;

		//create two random numbers, make sure u1 is greater than epsilon
		float u1, u2;
		do
		{
			u1 = UnityEngine.Random.Range(0.0f, 1.0f);
			u2 = UnityEngine.Random.Range(0.0f, 1.0f);
		} while (u1 <= float.Epsilon);

		//compute z0 and z1
		var mag = sigma * Mathf.Sqrt(-2.0f * Mathf.Log(u1));
		var z0 = mag * Mathf.Cos(two_pi * u2) + mu;
		var z1 = mag * Mathf.Sin(two_pi * u2) + mu;

		return new Tuple<float, float>(z0,z1);
	}
}
