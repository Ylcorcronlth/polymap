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
			new Vertex(-1, 1, Vertex.Type.E), new Vertex(0, 0, Vertex.Type.W),
			new Vertex(-1, 0, Vertex.Type.E), new Vertex(1, -1, Vertex.Type.W)
		};

		private int Q, R, S;
		private Vector2 _position;
		private bool _position_valid = false;

		public int q {
			get { return Q; }
		}
		public int r {
			get { return R; }
		}

		public int s {
			get { return S; }
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

		public Polygon(int q = 0, int r = 0) {
			Q = q;
			R = r;
			S = -q - r;
		}

		public Polygon(int q, int r, int s) {
			Q = q;
			R = r;
			S = s;
		}

		public static Polygon operator+(Polygon a, Polygon b) {
			return new Polygon(a.q + b.q, a.r + b.r, a.s + b.s);
		}

		public static Polygon operator-(Polygon a, Polygon b) {
			return new Polygon(a.q - b.q, a.r - b.r, a.s - b.s);
		}

		public static Polygon operator*(int k, Polygon a) {
			return new Polygon(a.q * k, a.r * k, a.s * k);
		}

		public static Polygon operator*(Polygon a, int k) {
			return new Polygon(a.q * k, a.r * k, a.s * k);
		}

		public static bool operator==(Polygon a, Polygon b) {
			return a.q == b.q && a.r == b.r;
		}

		public static bool operator!=(Polygon a, Polygon b) {
			return a.q != b.q || a.r != b.r;
		}

		public override bool Equals(object obj) {
			if (obj == null || GetType() != obj.GetType()) {
				return false;
			}
			Polygon b = (Polygon)obj;
			return q == b.q && r == b.r;
		}

		public override int GetHashCode() {
			unchecked {
				int result = 99989;
				result = result*496187739 + q;
				result = result*496187739 + r;
				return result;
			}
		}

		public Polygon GetNeighbor(int direction) {
			return this + _Neighbors[direction];
		}

		public Vertex GetCorner(int direction) {
			return this + _Corners[direction];
		}

		private Vector2 ToCartesian() {
			return new Vector2(
				Utils.SQRT3_2*q,
				0.5f*q + r
			);
		}

		public Polygon FromCartesian(Vector2 position) {
			return FractionalPolygon.FromCartesian(position).Round();
		}

		public override string ToString() {
			return "(" + q + ", " + r + ")";
		}
	}
}
