using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexGridCreator : MonoBehaviour {
	public int Width=5, Height=5;
	public Vector2 Scale = new Vector2(1.0f, 1.0f);
	public float LineWidth=0.05f;
	public Color LineColor = Color.black;
	public bool Pointy = false;

	private Mesh mesh;

	private void Generate() {
		mesh.Clear();

		var vertices = new List<Vector3>();
		var triangles = new List<int>();
		var colors = new List<Color>();
		var lookup = new Dictionary<Hex.Vertex, int>();

		float t0 = Time.realtimeSinceStartup;
		var map = Hex.Region.FlatRectangle(Width, Height);
		Debug.Log("Grid creation: " + (Time.realtimeSinceStartup - t0) + " (" + map.PolygonCount + ", " + map.VertexCount + ")");
		var layout = new Hex.Layout(Scale, (Pointy ? Hex.Orientation.Pointy : Hex.Orientation.Flat));

		t0 = Time.realtimeSinceStartup;
		Vector2 disp1 = layout.GetScreenPosition(new Hex.Polygon(2, -1));
		Vector2 disp2 = layout.GetScreenPosition(new Hex.Polygon(1, 1));
		disp1 = (0.5f*LineWidth/disp1.magnitude) * disp1;
		disp2 = (0.5f*LineWidth/disp2.magnitude) * disp2;

		foreach (Hex.Vertex vertex in map.Vertices) {
			Vector2 pt = layout.GetScreenPosition(vertex);
			lookup[vertex] = vertices.Count;
			vertices.Add(new Vector3(pt.x, 0.0f, pt.y));
			colors.Add(LineColor);
			colors.Add(LineColor);
			colors.Add(LineColor);
			colors.Add(LineColor);
			if (vertex.side == Hex.Vertex.Type.E) {
				vertices.Add(new Vector3(pt.x - disp1.x, 0.0f, pt.y + disp1.y));
				vertices.Add(new Vector3(pt.x + disp2.x, 0.0f, pt.y + disp2.y));
				vertices.Add(new Vector3(pt.x + disp2.x, 0.0f, pt.y - disp2.y));
			} else {
				vertices.Add(new Vector3(pt.x + disp1.x, 0.0f, pt.y + disp1.y));
				vertices.Add(new Vector3(pt.x - disp2.x, 0.0f, pt.y + disp2.y));
				vertices.Add(new Vector3(pt.x - disp2.x, 0.0f, pt.y - disp2.y));
			}
		}

		foreach (Hex.Polygon polygon in map.Polygons) {
			Vector2 pt = layout.GetScreenPosition(polygon);
			int[] k = new int[6];
			for (int i = 0; i < 6; i++) {
				k[i] = lookup[polygon.GetCorner(i)];
			}
			triangles.Add(k[0]);
			triangles.Add(k[0] + 1);
			triangles.Add(k[1]);

			triangles.Add(k[0] + 1);
			triangles.Add(k[1] + 3);
			triangles.Add(k[1]);

			triangles.Add(k[1]);
			triangles.Add(k[1] + 3);
			triangles.Add(k[2]);

			triangles.Add(k[1] + 3);
			triangles.Add(k[2] + 3);
			triangles.Add(k[2]);

			triangles.Add(k[2]);
			triangles.Add(k[2] + 3);
			triangles.Add(k[3]);

			triangles.Add(k[2] + 3);
			triangles.Add(k[3] + 1);
			triangles.Add(k[3]);

			triangles.Add(k[3]);
			triangles.Add(k[3] + 1);
			triangles.Add(k[4]);

			triangles.Add(k[3] + 1);
			triangles.Add(k[4] + 2);
			triangles.Add(k[4]);

			triangles.Add(k[4]);
			triangles.Add(k[4] + 2);
			triangles.Add(k[5]);

			triangles.Add(k[4] + 2);
			triangles.Add(k[5] + 2);
			triangles.Add(k[5]);

			triangles.Add(k[5]);
			triangles.Add(k[5] + 2);
			triangles.Add(k[0]);

			triangles.Add(k[5] + 2);
			triangles.Add(k[0] + 1);
			triangles.Add(k[0]);
		}

		Debug.Log("Mesh creation: " + (Time.realtimeSinceStartup - t0));

		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.colors = colors.ToArray();
		mesh.RecalculateNormals();
	}

	// Use this for initialization
	void Awake()
	{
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "Hex Grid Lines";
		Generate();
	}

	// Update is called once per frame
	void Update()
	{

	}
}
