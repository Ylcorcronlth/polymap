using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexGridBorder : MonoBehaviour {
	public float LineWidthInternal;
	public float LineWidthExternal;
	public Color LineColor;
	public Hex.Region Region;
	public Hex.Region Chunk;

	private Mesh mesh;
	private List<Vector3> vertices;
	private List<int> triangles;
	private List<Color> colors;

	void Awake() {
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "Grid Border";

		vertices = new List<Vector3>();
		triangles = new List<int>();
		colors = new List<Color>();
	}

	private struct Edge {
		public Hex.Vertex v1, v2;
	}

	private void CreateStartVertex(Hex.HalfEdge edge) {
		Hex.HalfEdge prev;
		if (Region.Contains(edge.prev.polygon) && !Region.Contains(edge.prev.joins)) {
			prev = edge.prev;
		} else {
			prev = edge.enters;
		}
		Vector3 left = OffsetLineIntersection(prev.start.position, edge.start.position, edge.end.position, LineWidthExternal/2.0f);
		Vector3 right = OffsetLineIntersection(prev.start.position, edge.start.position, edge.end.position, -LineWidthInternal/2.0f);
		vertices.Add(left);
		vertices.Add(right);
		colors.Add(LineColor);
		colors.Add(LineColor);
	}

	private void CreateEndVertex(Hex.HalfEdge edge) {
		Hex.HalfEdge next;
		if (Region.Contains(edge.next.polygon) && !Region.Contains(edge.next.joins)) {
			next = edge.next;
		} else {
			next = edge.continues;
		}

		Vector3 left = OffsetLineIntersection(edge.start.position, edge.end.position, next.end.position, LineWidthExternal/2.0f);
		Vector3 right = OffsetLineIntersection(edge.start.position, edge.end.position, next.end.position, -LineWidthInternal/2.0f);
		vertices.Add(left);
		vertices.Add(right);
		colors.Add(LineColor);
		colors.Add(LineColor);
	}

	private Vector3 OffsetLineIntersection(Vector2 a, Vector2 b, Vector2 c, float amount) {
		Vector2 u1 = a - b;
		u1.Normalize();
		Vector2 u2 = b - c;
		u2.Normalize();
		float det = u1.x*u2.y - u2.x*u1.y;
		return new Vector3(amount*(u2.x - u1.x)/det + b.x, 0.0f, amount*(u2.y - u1.y)/det + b.y);
	}

	private void CreateQuad(int i1, int i2) {
		triangles.Add(i1);
		triangles.Add(i2 + 1);
		triangles.Add(i2);
		triangles.Add(i1);
		triangles.Add(i1 + 1);
		triangles.Add(i2 + 1);
	}

	public void Apply() {
		vertices.Clear();
		triangles.Clear();
		colors.Clear();

		var lookup = new Dictionary<Hex.Vertex, int>();

		foreach (Hex.HalfEdge edge in Chunk.Edges) {
			// The edge is contained in the chunk (so both of its vertices are) and the edge is on the border of the region.
			if (Region.Contains(edge.polygon) && !Region.Contains(edge.joins)) {
				int i1, i2;
				if (!lookup.TryGetValue(edge.start, out i1)) {
					i1 = vertices.Count;
					CreateStartVertex(edge);
					lookup[edge.start] = i1;
				}
				if (!lookup.TryGetValue(edge.end, out i2)) {
					i2 = vertices.Count;
					CreateEndVertex(edge);
					lookup[edge.end] = i2;
				}
				CreateQuad(i1, i2);
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
