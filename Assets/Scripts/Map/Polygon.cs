using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hex {
	public class Polygon {
		private static Polygon[] _Neighbors = {
			new Polygon(1, 0, -1), new Polygon(0, 1, -1), new Polygon(-1, 1, 0),
			new Polygon(-1, 0, 1), new Polygon(0, -1, 1), new Polygon(1, -1, 0)
		};

		private static Vertex[] _Corners = {
			new Vertex(0, 0, Vertex.Type.E), new Vertex(1, 0, Vertex.Type.W),
			new Vertex(-1, 1, Vertex.Type.E), new Vertex(-1, 0, Vertex.Type.W),
			new Vertex(-1, 0, Vertex.Type.E), new Vertex(1, -1, Vertex.Type.W)
		};

		private int _q, _r, _s;
		private Vector2 _position;
		private bool _position_valid = false;

		public int q {
			get { return _q; }
		}
		public int r {
			get { return _r; }
		}

		public int s {
			get { return _s; }
		}

		public Vector2 position {
			get {
				if (_position_valid) {
					return _position;
				} else {
					_position_valid = true;
					_position = ToCartesian();
					return _position;
				}
			}
		}

		public IEnumerable<HalfEdge> borders {
			get {
				for (int i = 0; i < 6; i++) {
					yield return GetBorder(i);
				}
			}
		}

		public IEnumerable<Vertex> corners {
			get {
				for (int i = 0; i < 6; i++) {
					yield return GetCorner(i);
				}
			}
		}

		public IEnumerable<Polygon> neighbors {
			get {
				for (int i = 0; i < 6; i++) {
					yield return GetNeighbor(i);
				}
			}
		}

		public Polygon(int q = 0, int r = 0) {
			_q = q;
			_r = r;
			_s = -q - r;
		}

		public Polygon(int q, int r, int s) {
			_q = q;
			_r = r;
			_s = s;
		}

		public Polygon FromCartesian(Vector2 position) {
			return FractionalPolygon.FromCartesian(position).Round();
		}

		private Vector2 ToCartesian() {
			return new Vector2(
				Utils.SQRT3_2*_q,
				0.5f*_q + _r
			);
		}

		public static Polygon operator+(Polygon a, Polygon b) {
			return new Polygon(a._q + b._q, a._r + b._r, a._s + b._s);
		}

		public static Polygon operator-(Polygon a, Polygon b) {
			return new Polygon(a._q - b._q, a._r - b._r, a._s - b._s);
		}

		public static Polygon operator*(int k, Polygon a) {
			return new Polygon(a._q * k, a._r * k, a._s * k);
		}

		public static Polygon operator*(Polygon a, int k) {
			return new Polygon(a._q * k, a._r * k, a._s * k);
		}

		public static bool operator==(Polygon a, Polygon b) {
			return a._q == b._q && a._r == b._r;
		}

		public static bool operator!=(Polygon a, Polygon b) {
			return a._q != b._q || a._r != b._r;
		}

		public override bool Equals(object obj) {
			if (obj == null || GetType() != obj.GetType()) {
				return false;
			}
			Polygon b = (Polygon)obj;
			return _q == b._q && _r == b._r;
		}

		public override int GetHashCode() {
			unchecked {
				int result = 99989;
				result = result*496187739 + _q;
				result = result*496187739 + _r;
				return result;
			}
		}

		public override string ToString() {
			return "(" + _q + ", " + _r + ", " + _s + ")";
		}

		public Polygon GetNeighbor(int direction) {
			return this + _Neighbors[direction];
		}

		public Vertex GetCorner(int direction) {
			Vertex offset = _Corners[direction];
			return new Vertex(this + offset.polygon, offset.side);
		}

		public HalfEdge GetBorder(int direction) {
			return new HalfEdge(this, (HalfEdge.Type)direction);
		}
	}
}
