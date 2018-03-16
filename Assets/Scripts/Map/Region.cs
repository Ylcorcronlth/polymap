using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hex {
	public class Region {
		// Handles determining if a given hex/vertex is inside a region, and iterating over the region.
		// Provides static methods for creating regions of particular shape.
		private HashSet<Polygon> polygons;
		private HashSet<Vertex> vertices;

		public IEnumerable<Polygon> Polygons {
			get { return (IEnumerable<Polygon>)polygons; }
		}

		public int PolygonCount {
			get { return polygons.Count; }
		}

		public IEnumerable<Vertex> Vertices {
			get { return (IEnumerable<Vertex>)vertices; }
		}

		public int VertexCount {
			get { return vertices.Count; }
		}

		public Region() {
			polygons = new HashSet<Polygon>();
			vertices = new HashSet<Vertex>();
		}

		public Region(IEnumerable<Polygon> polygons, IEnumerable<Vertex> vertices) {
			this.polygons = new HashSet<Polygon>(polygons);
			this.vertices = new HashSet<Vertex>(vertices);
		}

		public bool Contains(Polygon polygon) {
			return polygons.Contains(polygon);
		}

		public bool Contains(Vertex vertex) {
			return vertices.Contains(vertex);
		}

		public Bounds GetBounds(Transform transform) {
			// Gets bounds in world space for the given transform.
			Bounds result = new Bounds();
			foreach (Vertex vertex in Vertices) {
				Vector3 pos = new Vector3(vertex.position.x, 0.0f, vertex.position.y);
				result.Encapsulate(transform.TransformPoint(pos));
			}
			return result;
		}

		public static Region FromPolygons(IEnumerable<Polygon> polygons) {
			var vertices = new HashSet<Vertex>();
			foreach (Polygon poly in polygons) {
				for (int i = 0; i < 6; i++) {
					vertices.Add(poly.GetCorner(i));
				}
			}
			return new Region(polygons, vertices);
		}

		public static Region FlatRectangle(int width, int height, int origin_q = 0, int origin_r = 0) {
			var polygons = new HashSet<Polygon>();
			for (int q = 0; q < width; q++) {
				for (int r = -Utils.Fastfloor(q/2.0f); r < height - Utils.Fastceil(q/2.0f); r++) {
					polygons.Add(new Polygon(q + origin_q, r + origin_r));
				}
			}
			return Region.FromPolygons(polygons);
		}

	}
}
