using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hex {
	public class HalfEdge {
			public enum Type { NE, N, NW, SW, S, SE };

			private static HalfEdge[] _Pair = {
				new HalfEdge(1, 0, Type.SW), new HalfEdge(0, 1, Type.S), new HalfEdge(-1, 1, Type.SE),
				new HalfEdge(-1, 0, Type.NE), new HalfEdge(0, -1, Type.N), new HalfEdge(1, -1, Type.NW)
			};

			private static HalfEdge[] _Continues = {
				new HalfEdge(0, 1, Type.SE), new HalfEdge(-1, 1, Type.NE), new HalfEdge(-1, 0, Type.N),
				new HalfEdge(0, -1, Type.NW), new HalfEdge(1, -1, Type.SW), new HalfEdge(1, 0, Type.S)
			};

			private static HalfEdge[] _Enters = {
				new HalfEdge(1, -1, Type.N), new HalfEdge(1, 0, Type.NW), new HalfEdge(0, 1, Type.SW),
				new HalfEdge(-1, 1, Type.S), new HalfEdge(-1, 0, Type.SE), new HalfEdge(0, -1, Type.NE)
			};

			private static Polygon[] _Joins = {
				new Polygon(1, 0, -1), new Polygon(0, 1, -1), new Polygon(-1, 1, 0),
				new Polygon(-1, 0, 1), new Polygon(0, -1, 1), new Polygon(1, -1, 0)
			};

			private static Vertex[] _Start = {
				new Vertex(0, 0, Vertex.Type.E), new Vertex(1, 0, Vertex.Type.W),
				new Vertex(-1, 1, Vertex.Type.E), new Vertex(0, 0, Vertex.Type.W),
				new Vertex(-1, 0, Vertex.Type.E), new Vertex(1, -1, Vertex.Type.W)
			};

			private static Vertex[] _End = {
				new Vertex(1, 0, Vertex.Type.W), new Vertex(-1, 1, Vertex.Type.E),
				new Vertex(0, 0, Vertex.Type.W), new Vertex(-1, 0, Vertex.Type.E),
				new Vertex(1, -1, Vertex.Type.W), new Vertex(0, 0, Vertex.Type.E)
			};

			private Polygon _polygon;
			private Type _side;

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

			public HalfEdge pair {
				get {
					HalfEdge offset = _Pair[(int)_side];
					return new HalfEdge(this._polygon + offset._polygon, offset._side);
				}
			}

			public HalfEdge next {
				get {
					return new HalfEdge(this._polygon, (Type)Utils.Mod((int)this._side + 1, 6));
				}
			}

			public HalfEdge prev {
				get {
					return new HalfEdge(this._polygon, (Type)Utils.Mod((int)this._side - 1, 6));
				}
			}

			public HalfEdge continues {
				get {
					HalfEdge offset = _Continues[(int)_side];
					return new HalfEdge(this._polygon + offset._polygon, offset._side);
				}
			}

			public HalfEdge enters {
				get {
					HalfEdge offset = _Enters[(int)_side];
					return new HalfEdge(this._polygon + offset._polygon, offset._side);
				}
			}

			public Polygon polygon {
				get { return _polygon; }
			}

			public Polygon joins {
				get { return polygon + _Joins[(int)_side]; }
			}

			public Vertex start {
				get {
					Vertex offset = _Start[(int)_side];
					return new Vertex(_polygon + offset.polygon, offset.side);
				}
			}

			public Vertex end {
				get {
					Vertex offset = _End[(int)_side];
					return new Vertex(_polygon + offset.polygon, offset.side);
				}
			}

			public HalfEdge(int q = 0, int r = 0, Type side = Type.NE) {
				_polygon = new Polygon(q, r);
				_side = side;
			}

			public HalfEdge(Polygon polygon, Type side = Type.NE) {
				_polygon = polygon;
				_side = side;
			}

		public static bool operator==(HalfEdge a, HalfEdge b) {
			return a._polygon == b._polygon && a._side == b._side;
		}

		public static bool operator!=(HalfEdge a, HalfEdge b) {
			return a._polygon != b._polygon || a._side != b._side;
		}

		public override bool Equals(object obj) {
			if (obj == null || GetType() != obj.GetType()) {
				return false;
			}
			HalfEdge b = (HalfEdge)obj;
			return _polygon == b._polygon && _side == b._side;
		}

		public override int GetHashCode() {
			unchecked {
				int result = 16069;
				result = result*496187739 + _polygon.GetHashCode();
				result = result*496187739 + (int)_side;
				return result;
			}
		}
	}
}
