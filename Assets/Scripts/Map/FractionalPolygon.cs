using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hex {
    public class FractionalPolygon {
            private float Q, R, S;

            public float q {
                get { return Q; }
            }
            public float r {
                get { return R; }
            }

            public float s {
                get { return S; }
            }

            public FractionalPolygon(float q = 0, float r = 0) {
                Q = q;
                R = r;
                S = -q - r;
            }

            public FractionalPolygon(float q, float r, float s) {
                Q = q;
                R = r;
                S = s;
            }

            public static FractionalPolygon operator+(FractionalPolygon a, FractionalPolygon b) {
                return new FractionalPolygon(a.q + b.q, a.r + b.r, a.s + b.s);
            }

            public static FractionalPolygon operator+(FractionalPolygon a, Polygon b) {
                return new FractionalPolygon(a.q + b.q, a.r + b.r, a.s + b.s);
            }

            public static FractionalPolygon operator+(Polygon a, FractionalPolygon b) {
                return new FractionalPolygon(a.q + b.q, a.r + b.r, a.s + b.s);
            }

            public Polygon Round() {
                int qi = (int)Mathf.Round(q);
                int ri = (int)Mathf.Round(r);
                int si = (int)Mathf.Round(s);

                float dq = Mathf.Abs(qi - q);
                float dr = Mathf.Abs(ri - r);
                float ds = Mathf.Abs(si - s);

                if (dq > dr && dq > ds) {
                    qi = -ri - si;
                } else if (dr > ds) {
                    ri = -qi - si;
                } else {
                    si = -qi - ri;
                }
                return new Polygon(qi, ri, si);
            }
    }
}
