using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hex {
	public class Vertex {
		private static Vertex[] _Adjacent = {
			new Vertex(2, -1, Type.W), new Vertex(1, 0, Type.W), new Vertex(1, -1, Type.W)
		};

		private static Polygon[] _Touches = {
			new Polygon(1, 0, -1), new Polygon(0, 0, 0), new Polygon(1, -1, 0)
		};

		public enum Type { E, W };
		private Polygon Poly;
		private Type Side;
		private Vector2 _position;
		private bool _position_valid = false;

		public int q {
			get { return Poly.q; }
		}

		public int r {
			get { return Poly.r; }
		}

		public int s {
			get { return Poly.s; }
		}

		public Type side {
			get { return Side; }
		}

		public Polygon polygon {
			get { return Poly; }
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

		public Vertex(int q = 0, int r = 0, Type side = Type.E) {
			Poly = new Polygon(q, r);
			Side = side;
		}

		public static Vertex operator+(Vertex a, Polygon b) {
			return new Vertex(a.q + b.q, a.r + b.r, a.side);
		}

		public static Vertex operator+(Polygon a, Vertex b) {
			return new Vertex(a.q + b.q, a.r + b.r, b.side);
		}

		public static Vertex operator*(int k, Vertex a) {
			return new Vertex(a.q * k, a.r * k, a.side);
		}

		public static Vertex operator*(Vertex a, int k) {
			return new Vertex(a.q * k, a.r * k, a.side);
		}

		public static bool operator==(Vertex a, Vertex b) {
			return a.q == b.q && a.r == b.r && a.side == b.side;
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

		private Vertex Reverse() {
			return new Vertex(-q, -r, (Side == Type.E ? Type.W : Type.E));
		}

		public Vertex GetAdjacent(int direction) {
			if (side == Type.E) {
				return this.Poly + _Adjacent[direction];
			} else {
				return this.Poly + _Adjacent[direction].Reverse();
			}
		}

		public Polygon GetTouches(int direction) {
			if (side == Type.E) {
				return this.Poly + _Touches[direction];
			} else {
				return this.Poly - _Touches[direction];
			}
		}

		private Vector2 ToCartesian() {
			Vector2 b = polygon.position;
			if (Side == Type.E) {
				return b + new Vector2(1.0f/Utils.SQRT3, 0.0f);
			} else {
				return b - new Vector2(1.0f/Utils.SQRT3, 0.0f);
			}
		}
	}
}
