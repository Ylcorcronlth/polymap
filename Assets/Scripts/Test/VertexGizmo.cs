using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexGizmo : MonoBehaviour {

	public int Width=5, Height=5;

	private List<Vector3> vertices;

	// Use this for initialization
	void Start()
	{
		vertices = new List<Vector3>();
		float t0 = Time.realtimeSinceStartup;
		var map = Hex.Region.FlatRectangle(Width, Height, 0, 0);
		Debug.Log("Generating map grid: " + (Time.realtimeSinceStartup - t0) + " (" + map.PolygonCount + ", " + map.VertexCount + ")");
		foreach (Hex.Vertex vertex in map.Vertices) {
			Vector2 pt = vertex.ToCartesian();
			vertices.Add(new Vector3(pt.x, 0.0f, pt.y));
		}
		/*foreach (Hex.Polygon polygon in map.Polygons) {
			Vector2 pt = layout.GetScreenPosition(polygon);
			vertices.Add(new Vector3(pt.x, 0.0f, pt.y));
		}*/
	}

	// Update is called once per frame
	void Update()
	{

	}

	private void OnDrawGizmos() {
		if (vertices == null) {
			return;
		}
		Gizmos.color = Color.black;
		for (int i = 0; i < vertices.Count; i++) {
			Gizmos.DrawSphere(vertices[i], 0.05f);
		}
	}
}
