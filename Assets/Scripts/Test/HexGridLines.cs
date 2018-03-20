using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexGridLines : MonoBehaviour {
	public float LineWidth;
	public Color LineColor;
	public Hex.Region Region;
	public Hex.Region Chunk;

	private Mesh mesh;
	private List<Vector3> vertices;
	private List<int> triangles;
	private List<Color> colors;

	private static Vector3[] triangle_vertices = {
		new Vector3(-1.0f/Utils.SQRT3, 0.0f, 0.0f),
		new Vector3(0.5f/Utils.SQRT3, 0.0f, -0.5f),
		new Vector3(0.5f/Utils.SQRT3, 0.0f, 0.5f)
	};

	void Awake() {
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "Grid Lines";
		vertices = new List<Vector3>();
		triangles = new List<int>();
		colors = new List<Color>();
	}

	private void CreateVertex(Hex.Vertex vertex) {
		Vector3 origin = new Vector3(vertex.position.x, 0.0f, vertex.position.y);
		triangles.Add(vertices.Count);
		triangles.Add(vertices.Count + 2);
		triangles.Add(vertices.Count + 1);
		if (vertex.side == Hex.Vertex.Type.E) {
			vertices.Add(origin + triangle_vertices[0]*LineWidth);
			vertices.Add(origin + triangle_vertices[1]*LineWidth);
			vertices.Add(origin + triangle_vertices[2]*LineWidth);
		} else {
			vertices.Add(origin - triangle_vertices[0]*LineWidth);
			vertices.Add(origin - triangle_vertices[1]*LineWidth);
			vertices.Add(origin - triangle_vertices[2]*LineWidth);
		}
		colors.Add(LineColor);
		colors.Add(LineColor);
		colors.Add(LineColor);
	}

	private void CreateQuad(int i1, int i2, Hex.HalfEdge.Type side) {
		if (side == Hex.HalfEdge.Type.NE) {
			triangles.Add(i1);
			triangles.Add(i2 + 2);
			triangles.Add(i2);
			triangles.Add(i1);
			triangles.Add(i2);
			triangles.Add(i1 + 2);
		} else if (side == Hex.HalfEdge.Type.N) {
			triangles.Add(i1 + 2);
			triangles.Add(i2 + 1);
			triangles.Add(i2 + 2);
			triangles.Add(i1 + 2);
			triangles.Add(i2 + 2);
			triangles.Add(i1 + 1);
		} else if (side == Hex.HalfEdge.Type.NW) {
			triangles.Add(i1 + 1);
			triangles.Add(i2);
			triangles.Add(i2 + 1);
			triangles.Add(i1 + 1);
			triangles.Add(i2 + 1);
			triangles.Add(i1);
		}
	}

	public void Apply() {
		vertices.Clear();
		triangles.Clear();
		colors.Clear();

		var lookup = new Dictionary<Hex.Vertex, int>();

		foreach (Hex.Polygon polygon in Chunk.Polygons) {
			for (int i = 0; i < 3; i++) {
				Hex.HalfEdge edge = polygon.GetBorder(i);
				if (Region.Contains(edge.polygon) && Region.Contains(edge.joins)) {
					int i1, i2;
					if (!lookup.TryGetValue(edge.start, out i1)) {
						i1 = vertices.Count;
						CreateVertex(edge.start);
						lookup[edge.start] = i1;
					}
					if (!lookup.TryGetValue(edge.end, out i2)) {
						i2 = vertices.Count;
						CreateVertex(edge.end);
						lookup[edge.end] = i2;
					}
					CreateQuad(i1, i2, edge.side);
				}
			}
		}

		// Finalize mesh.
		mesh.Clear();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.colors = colors.ToArray();
		mesh.RecalculateNormals();
	}
}
