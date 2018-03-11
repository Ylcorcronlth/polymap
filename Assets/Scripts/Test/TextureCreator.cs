using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureCreator : MonoBehaviour {

	public int Resolution = 64;
	Texture2D Texture;
	public float SearchRadius = 0.05f;
	public float Exponent = 2.0f;
	public float FalloffWeight = 1.0f;

	private InverseDistanceWeighting Interpolator;
	public Vector2[] Points;
	public float[] Values;
	public Vector2[] Gradients;

	private void CreateInterpolator() {
		Interpolator = new InverseDistanceWeighting(Points, SearchRadius, Exponent, FalloffWeight);
	}

	private void CreateTexture() {
		Texture = new Texture2D(Resolution, Resolution, TextureFormat.RGB24, true);
		Texture.name = "Procedural texture";
		Texture.wrapMode = TextureWrapMode.Clamp;
		GetComponent<MeshRenderer>().material.mainTexture = Texture;
	}

	private void FillTexture() {
		float t0 = Time.realtimeSinceStartup;
		for (int x = 0; x < Resolution; x++) {
			for (int y = 0; y < Resolution; y++) {
				float value = Interpolator.Evaluate(new Vector2((float)x/(Resolution - 1.0f), (float)y/(Resolution - 1.0f)), Values, Gradients);
				//float value = 4.0f*(x/(Resolution - 1.0f) - 0.5f)*(x/(Resolution - 1.0f) - 0.5f);
				//Debug.Log(x + ", " + y + ": " + value);
				if (value > 1.0f) {
					value = 1.0f;
				} else if (value < -1.0f) {
					value = -1.0f;
				}
				if (value > 0.0f) {
					Texture.SetPixel(x, y, new Color(value, value, value));
				} else {
					Texture.SetPixel(x, y, new Color(-value, 0.0f, 0.0f));
				}
			}
		}
		Debug.Log("Time to generate texture: " + (Time.realtimeSinceStartup - t0));
		Texture.Apply();
	}

	// Use this for initialization
	void Start()
	{
		CreateInterpolator();
		CreateTexture();
		FillTexture();
	}

	// Update is called once per frame
	void Update()
	{

	}
}
