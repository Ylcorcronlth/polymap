﻿using System.Collections;
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

		public IEnumerable<Polygon> BorderPolygons {
			get {
				foreach (Polygon polygon in Polygons) {
					bool border = false;
					for (int i = 0; i < 6; i++) {
						border = border || polygons.Contains(polygon.GetNeighbor(i));
					}
					if (border) {
						yield return polygon;
					}
				}
			}
		}

		public int PolygonCount {
			get { return polygons.Count; }
		}

		public IEnumerable<Vertex> Vertices {
			get { return (IEnumerable<Vertex>)vertices; }
		}

		public IEnumerable<Vertex> BorderVertices {
			get {
				foreach (Vertex vertex in Vertices) {
					bool border = false;
					for (int i = 0; i < 3; i++) {
						border = border || polygons.Contains(vertex.GetTouches(i));
					}
					if (border) {
						yield return vertex;
					}
				}
			}
		}


		public IEnumerable<HalfEdge> Edges {
			get {
				foreach (Polygon polygon in polygons) {
					foreach (HalfEdge edge in polygon.borders) {
						yield return edge;
					}
				}
			}
		}

		public IEnumerable<HalfEdge> BorderEdges {
			get {
				foreach (HalfEdge edge in Edges) {
					if (true) {
						yield return edge;
					}
				}
			}
		}

		public int VertexCount {
			get { return vertices.Count; }
		}

		public Region() {
			polygons = new HashSet<Polygon>();
			vertices = new HashSet<Vertex>();
		}

		public Region(IEnumerable<Polygon> polygons) {
			this.polygons = new HashSet<Polygon>(polygons);
			vertices = new HashSet<Vertex>();
		}

		public Region(IEnumerable<Polygon> polygons, IEnumerable<Vertex> vertices) {
			this.polygons = new HashSet<Polygon>(polygons);
			this.vertices = new HashSet<Vertex>(vertices);
		}

		public Region(IEnumerable<Polygon> polygons, IEnumerable<Vertex> vertices, IEnumerable<HalfEdge> edges) {
			this.polygons = new HashSet<Polygon>(polygons);
			this.vertices = new HashSet<Vertex>(vertices);
		}

		public void Add(Polygon polygon) {
			polygons.Add(polygon);
			foreach (Vertex vertex in polygon.corners) {
				vertices.Add(vertex);
			}
		}

		public void Remove(Polygon polygon) {
			polygons.Remove(polygon);
			foreach (Vertex vertex in polygon.corners) {
				bool contains = false;
				foreach (Polygon touches in vertex.touches) {
					if (Contains(touches)) {
						contains = true;
						break;
					}
				}
				if (!contains) {
					vertices.Remove(vertex);
				}
			}
		}

		public bool Contains(Polygon polygon) {
			return polygons.Contains(polygon);
		}

		public bool Contains(Vertex vertex) {
			return vertices.Contains(vertex);
		}

		public bool Contains(HalfEdge edge) {
			return polygons.Contains(edge.polygon);
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
			var edges = new HashSet<HalfEdge>();
			foreach (Polygon poly in polygons) {
				for (int i = 0; i < 6; i++) {
					vertices.Add(poly.GetCorner(i));
					edges.Add(poly.GetBorder(i));
				}
			}
			return new Region(polygons, vertices, edges);
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
