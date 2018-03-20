using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexLinesTest : MonoBehaviour {
	public int Width, Height;
	public float LineWidth = 0.05f;
	public float BorderLineWidthInternal = 0.25f;
	public float BorderLineWidthExternal = 0.25f;
	public Color LineColor = Color.black;
	public Color BorderColor = Color.black;
	public Material material;

	private GameObject lines;
	private GameObject border;

	// Use this for initialization
	void Start()
	{
		border = new GameObject("Hex Grid Border", typeof(HexGridBorder));
		border.transform.parent = transform;
		border.transform.localPosition = new Vector3(0.0f, 1.0f, 0.0f);
		border.GetComponent<MeshRenderer>().material = material;
		border.GetComponent<HexGridBorder>().LineWidthInternal = BorderLineWidthInternal;
		border.GetComponent<HexGridBorder>().LineWidthExternal = BorderLineWidthExternal;
		border.GetComponent<HexGridBorder>().LineColor = BorderColor;
		border.GetComponent<HexGridBorder>().Region = Hex.Region.FlatRectangle(Width, Height);
		border.GetComponent<HexGridBorder>().Chunk = Hex.Region.FlatRectangle(Width + 2, Height + 2);
		border.GetComponent<HexGridBorder>().Apply();

		border = new GameObject("Hex Grid Lines", typeof(HexGridLines));
		border.transform.parent = transform;
		border.GetComponent<MeshRenderer>().material = material;
		border.GetComponent<HexGridLines>().LineWidth = LineWidth;
		border.GetComponent<HexGridLines>().LineColor = LineColor;
		border.GetComponent<HexGridLines>().Region = Hex.Region.FlatRectangle(Width, Height);
		border.GetComponent<HexGridLines>().Chunk = Hex.Region.FlatRectangle(Width + 2, Height + 2);
		border.GetComponent<HexGridLines>().Apply();
	}

	// Update is called once per frame
	void Update()
	{

	}
}
