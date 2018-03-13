using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hex {
	public class MapArray<T> {
		private Dictionary<Polygon, T> PolygonData;
		private Dictionary<Vertex, T> VertexData;
		private T DefaultValue;

		public MapArray(T default_value) {
			PolygonData = new Dictionary<Polygon, T>();
			VertexData = new Dictionary<Vertex, T>();
			DefaultValue = default_value;
		}

		public T Get(Polygon polygon) {
			T value;
			bool found = PolygonData.TryGetValue(polygon, out value);
			return (found ? value : DefaultValue);
		}

		public T Get(Vertex vertex) {
			T value;
			bool found = VertexData.TryGetValue(vertex, out value);
			return (found ? value : DefaultValue);
		}

		public void Set(Polygon polygon, T value) {
			PolygonData[polygon] = value;
		}

		public void Set(Vertex vertex, T value) {
			VertexData[vertex] = value;
		}
	}
}
