using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hex {
	public class Orientation {
		public float f00, f10, f01, f11;
		public float b00, b10, b01, b11;
		public static Orientation Flat = new Orientation(Mathf.Sqrt(3.0f)/2.0f, 0.0f, 0.5f, 1.0f);
		public static Orientation Pointy = new Orientation(1.0f, 0.5f, 0.0f, Mathf.Sqrt(3.0f)/2.0f);
		public Orientation(float F00, float F10, float F01, float F11) {
			float det = F00*F11 - F10*F01;

			f00 = F00;
			f10 = F10;
			f01 = F01;
			f11 = F11;

			b00 = f11/det;
			b10 = -f10/det;
			b01 = -f01/det;
			b11 = f00/det;
		}
	}
}
