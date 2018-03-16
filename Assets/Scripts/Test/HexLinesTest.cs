using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HexLinesGenerator))]
public class HexLinesTest : MonoBehaviour {
	public int Width, Height;
	private HexLinesGenerator generator;

	// Use this for initialization
	void Start()
	{
		generator = GetComponent<HexLinesGenerator>();
		generator.Region = Hex.Region.FlatRectangle(Width, Height);
		generator.Chunk = generator.Region;
		generator.Apply();
	}

	// Update is called once per frame
	void Update()
	{

	}
}
