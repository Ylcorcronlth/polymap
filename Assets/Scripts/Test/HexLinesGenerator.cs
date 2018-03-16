using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexLinesGenerator : MonoBehaviour {
	public float LineWidth;
	public float BorderWidthInternal, BorderWidthExternal;
	public Color LineColor;
	public Hex.Region Region;
	public Hex.Region Chunk;

	private Mesh mesh;

	void Awake() {
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "Hex Grid Lines";
	}

	private Vector2 LineIntersection(Vector2 v1, float c1, Vector2 v2, float c2) {
		float det = v1.x*v2.y - v2.x*v1.y;
		return new Vector2(
			(c1*v2.x - c2*v1.x)/det,
			(c1*v2.y - c2*v1.y)/det
		);
	}

	public void Apply() {
		mesh.Clear();
		var vertices = new List<Vector3>();
		var triangles = new List<int>();
		var colors = new List<Color>();

		var lookup = new Dictionary<Hex.Vertex, int>();

		// Create points surrounding each vertex.
		foreach (Hex.Vertex vertex in Chunk.Vertices) {
			// todo: break this into its own method.
			lookup[vertex] = vertices.Count;

			Hex.Vertex v0 = vertex.GetAdjacent(0);
			Hex.Polygon p0 = vertex.GetTouches(0);
			Hex.Vertex v1 = vertex.GetAdjacent(1);
			Hex.Polygon p1 = vertex.GetTouches(1);
			Hex.Vertex v2 = vertex.GetAdjacent(2);
			Hex.Polygon p2 = vertex.GetTouches(2);
			float w0n, w0p, w1n, w1p, w2n, w2p;
			switch ((Region.Contains(p0) ? 1 : 0) + (Region.Contains(p1) ? 2 : 0) + (Region.Contains(p2) ? 4 : 0)) {
				case 0:
					w0n = w0p = w1n = w1p = w2n = w2p = 0.0f;
					break;
				case 1:
					w0n = BorderWidthExternal/2.0f;
					w0p = BorderWidthInternal/2.0f;
					w1n = BorderWidthInternal/2.0f;
					w1p = BorderWidthExternal/2.0f;
					w2n = 0.0f;
					w2p = 0.0f;
					break;
				case 2:
					w0n = 0.0f;
					w0p = 0.0f;
					w1n = BorderWidthExternal/2.0f;
					w1p = BorderWidthInternal/2.0f;
					w2n = BorderWidthInternal/2.0f;
					w2p = BorderWidthExternal/2.0f;
					break;
				case 3:
					w0n = BorderWidthExternal/2.0f;
					w0p = BorderWidthInternal/2.0f;
					w1n = LineWidth/2.0f;
					w1p = LineWidth/2.0f;
					w2n = BorderWidthInternal/2.0f;
					w2p = BorderWidthExternal/2.0f;
					break;
				case 4:
					w0n = BorderWidthInternal/2.0f;
					w0p = BorderWidthExternal/2.0f;
					w1n = 0.0f;
					w1p = 0.0f;
					w2n = BorderWidthExternal/2.0f;
					w2p = BorderWidthInternal/2.0f;
					break;
				case 5:
					w0n = LineWidth/2.0f;
					w0p = LineWidth/2.0f;
					w1n = BorderWidthInternal/2.0f;
					w1p = BorderWidthExternal/2.0f;
					w2n = BorderWidthExternal/2.0f;
					w2p = BorderWidthInternal/2.0f;
					break;
				case 6:
					w0n = BorderWidthInternal/2.0f;
					w0p = BorderWidthExternal/2.0f;
					w1n = BorderWidthExternal/2.0f;
					w1p = BorderWidthInternal/2.0f;
					w2n = LineWidth/2.0f;
					w2p = LineWidth/2.0f;
					break;
				case 7:
					w0n = w0p = w1n = w1p = w2n = w2p = LineWidth/2.0f;
					break;
				default:
					w0n = w0p = w1n = w1p = w2n = w2p = 0.0f;
					break;
			}

			Vector2 pt0 = vertex.position + LineIntersection(
				v1.position - vertex.position, w1p,
				v2.position - vertex.position, -w2n
			);
			Vector2 pt1 = vertex.position + LineIntersection(
				v2.position - vertex.position, w2p,
				v0.position - vertex.position, -w0n
			);
			Vector2 pt2 = vertex.position + LineIntersection(
				v0.position - vertex.position, w0p,
				v1.position - vertex.position, -w1n
			);

			//Debug.Log(LineWidth/2.0f);
			int k = vertices.Count;
			lookup[vertex] = k;
			vertices.Add(new Vector3(pt0.x, 0.0f, pt0.y));
			vertices.Add(new Vector3(pt1.x, 0.0f, pt1.y));
			vertices.Add(new Vector3(pt2.x, 0.0f, pt2.y));
			colors.Add(LineColor);
			colors.Add(LineColor);
			colors.Add(LineColor);
			triangles.Add(k);
			triangles.Add(k + 2);
			triangles.Add(k + 1);
		}
		// Create edges connecting points.
		foreach (Hex.Polygon polygon in Chunk.Polygons) {
			if (Region.Contains(polygon)) {
				int[] k = new int[6];
				for (int i = 0; i < 6; i++) {
					k[i] = lookup[polygon.GetCorner(i)];
				}

				triangles.Add(k[0]);
				triangles.Add(k[1]);
				triangles.Add(k[0] + 2);
				triangles.Add(k[0]);
				triangles.Add(k[1] + 2);
				triangles.Add(k[1]);

				triangles.Add(k[1] + 2);
				triangles.Add(k[2] + 2);
				triangles.Add(k[1] + 1);
				triangles.Add(k[1] + 2);
				triangles.Add(k[2] + 1);
				triangles.Add(k[2] + 2);

				triangles.Add(k[2]);
				triangles.Add(k[3]);
				triangles.Add(k[3] + 1);
				triangles.Add(k[2]);
				triangles.Add(k[2] + 1);
				triangles.Add(k[3]);

				if (!Region.Contains(polygon.GetNeighbor(3))) {
					triangles.Add(k[3]);
					triangles.Add(k[4]);
					triangles.Add(k[3] + 2);
					triangles.Add(k[3]);
					triangles.Add(k[4] + 2);
					triangles.Add(k[4]);
				}
				if (!Region.Contains(polygon.GetNeighbor(4))) {
					triangles.Add(k[4] + 1);
					triangles.Add(k[4] + 2);
					triangles.Add(k[5] + 2);
					triangles.Add(k[4] + 2);
					triangles.Add(k[5] + 1);
					triangles.Add(k[5] + 2);
				}
				if (!Region.Contains(polygon.GetNeighbor(5))) {
					triangles.Add(k[5] + 1);
					triangles.Add(k[0] + 1);
					triangles.Add(k[5]);
					triangles.Add(k[5] + 1);
					triangles.Add(k[0]);
					triangles.Add(k[0] + 1);
				}
			}
		}

		// Finalize mesh.
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.colors = colors.ToArray();
		mesh.RecalculateNormals();
	}
}
