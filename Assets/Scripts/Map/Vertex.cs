using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hex {
	public class Vertex {
		private static Polygon[] _Adjacent = {
			new Polygon(2, -1, -1), new Polygon(1, 0, -1), new Polygon(1, -1, 0)
		};

		private static Polygon[] _Touches = {
			new Polygon(1, 0, -1), new Polygon(0, 0, 0), new Polygon(1, -1, 0)
		};

		private static HalfEdge.Type[] _Protrudes = {
			HalfEdge.Type.S, HalfEdge.Type.NE, HalfEdge.Type.SW,
			HalfEdge.Type.N, HalfEdge.Type.SW, HalfEdge.Type.NE
		};

		public enum Type { E, W };
		private Polygon _polygon;
		private Type _side;
		private Vector2 _position;
		private bool _position_valid = false;

		public int q {
			get { return _polygon.q; }
		}

		public int r {
			get { return _polygon.r; }
		}

		public int s {
			get { return _polygon.s; }
		}

		public Type side {
			get { return _side; }
		}

		public Polygon polygon {
			get { return _polygon; }
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

		public IEnumerable<HalfEdge> protrudes {
			get {
				for (int i = 0; i < 3; i++) {
					yield return GetProtrudes(i);
				}
			}
		}

		public IEnumerable<Polygon> touches {
			get {
				for (int i = 0; i < 3; i++) {
					yield return GetTouches(i);
				}
			}
		}

		public IEnumerable<Vertex> adjacent {
			get {
				for (int i = 0; i < 3; i++) {
					yield return GetAdjacent(i);
				}
			}
		}

		public Vertex(int q = 0, int r = 0, Type side = Type.E) {
			_polygon = new Polygon(q, r);
			_side = side;
		}

		public Vertex(Polygon polygon, Type side = Type.E) {
			_polygon = polygon;
			_side = side;
		}

		private Vector2 ToCartesian() {
			Vector2 b = polygon.position;
			if (_side == Type.E) {
				return b + new Vector2(1.0f/Utils.SQRT3, 0.0f);
			} else {
				return b - new Vector2(1.0f/Utils.SQRT3, 0.0f);
			}
		}

		public static Vertex operator*(Vertex a, int k) {
			return new Vertex(a._polygon * k, a.side);
		}

		public static bool operator==(Vertex a, Vertex b) {
			return a._polygon == b._polygon && a.side == b.side;
		}

		public static bool operator!=(Vertex a, Vertex b) {
			return a.q != b.q || a.r != b.r || a.side != b.side;
		}

		public override bool Equals(object obj) {
			if (obj == null || GetType() != obj.GetType()) {
				return false;
			}
			Vertex b = (Vertex)obj;
			return q == b.q && r == b.r && side == b.side;
		}

		public override int GetHashCode() {
			unchecked {
				int result = 99989;
				result = result*496187739 + q;
				result = result*496187739 + r;
				result = result*496187739 + (int)side;
				return result;
			}
		}

		public override string ToString() {
			return "(" + _polygon.q + ", " + _polygon.r + ", " + _polygon.s + ", " + (_side == Type.E ? "E" : "W") + ")";
		}

		public Vertex GetAdjacent(int direction) {
			if (side == Type.E) {
				return new Vertex(this._polygon + _Adjacent[direction], Type.W);
			} else {
				return new Vertex(this._polygon - _Adjacent[direction], Type.E);
			}
		}

		public Polygon GetTouches(int direction) {
			if (side == Type.E) {
				return this._polygon + _Touches[direction];
			} else {
				return this._polygon - _Touches[direction];
			}
		}

		public HalfEdge GetProtrudes(int direction) {
			if (side == Type.E) {
				return new HalfEdge(this.polygon + _Touches[direction], _Protrudes[direction]);
			} else {
				return new HalfEdge(this.polygon - _Touches[direction], _Protrudes[3 + direction]);
			}
		}
	}
}
