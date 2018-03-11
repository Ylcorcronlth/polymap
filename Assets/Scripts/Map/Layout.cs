using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hex {

	public class Layout {
		// Handles conversion between hexagonal lattice and Cartesian coordinates.
		private Orientation orientation;
		private Vector2 scale;
		private Vector2 origin;

		public Layout(float scale = 1.0f) {
			orientation = Orientation.Flat;
			this.scale = new Vector2(scale, scale);
			origin = new Vector2(0.0f, 0.0f);
		}

		public Layout(float scale, Orientation orientation) {
			this.orientation = orientation;
			this.scale = new Vector2(scale, scale);
			origin = new Vector2(0.0f, 0.0f);
		}

		public Layout(Vector2 scale, Orientation orientation) {
			this.scale = scale;
			this.orientation = orientation;
			origin = new Vector2(0.0f, 0.0f);
		}

		public Vector2 GetScreenPosition(Polygon polygon) {
			float x = polygon.q*orientation.f00 + polygon.r*orientation.f10;
			float y = polygon.q*orientation.f01 + polygon.r*orientation.f11;
			return new Vector2(x*scale.x, y*scale.y) + origin;
		}

		public Vector2 GetScreenPosition(FractionalPolygon polygon) {
			float x = polygon.q*orientation.f00 + polygon.r*orientation.f10;
			float y = polygon.q*orientation.f01 + polygon.r*orientation.f11;
			return new Vector2(x*scale.x, y*scale.y) + origin;
		}

		public Vector2 GetScreenPosition(Vertex vertex) {
			if (vertex.side == Vertex.Type.E) {
				return GetScreenPosition(
					vertex.polygon + new FractionalPolygon(2.0f/3.0f, -1.0f/3.0f)
				);
			} else {
				return GetScreenPosition(
					vertex.polygon + new FractionalPolygon(-2.0f/3.0f, 1.0f/3.0f)
				);
			}
		}

		public Polygon GetGridPosition(Vector2 position) {
			position -= origin;
			position = new Vector2(position.x/scale.x, position.y/scale.y);
			float q = orientation.b00*position.x + orientation.b10*position.y;
			float r = orientation.b01*position.x + orientation.b11*position.y;
			return new FractionalPolygon(q, r).Round();
		}
	}
}
