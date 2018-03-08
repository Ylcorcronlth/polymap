using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseDistanceWeighting {
    private Vector2[] Points;  // The sample points to interpolate between.
    private float SearchRadius;  // The maximum distance at which points influence interpolation value.
    private List<int>[,] Chunks;  // The indices of points falling into cells in a 2Rx2R grid, for fast lookup.
    private int Size;  // The size of the above grid.
    private float Exponent; // The exponent for distance weighting.

    public InverseDistanceWeighting(Vector2[] points, float search_radius=0.05f, float exponent=2.0f) {
        Points = points;
        SearchRadius = search_radius;
        Exponent = exponent;
        // Points are assumed to fall into the rectangle (0, 0, 1, 1).
        Size = (int)(1.0f/(2.0f*SearchRadius));

        // Initialize chunks.
        Chunks = new List<int>[Size + 1, Size + 1];
        for (int x = 0; x <= Size; x++) {
            for (int y = 0; y <= Size; y++) {
                Chunks[x, y] = new List<int>();
            }
        }

        // Sort points into the grid.
        for (int i = 0; i < Points.Length; i++) {
            Vector2 pos = Points[i];
            int x = Utils.Fastfloor(pos.x * Size);
            int y = Utils.Fastfloor(pos.y * Size);
            Chunks[x, y].Add(i);
        }
    }

    private List<int> GetChunk(int xi, int yi) {
        // Get the points in a given chunk, returning an empty list if
        // outside bounds.
        if (xi >= 0 && xi <= Size && yi >= 0 && yi <= Size) {
            return Chunks[xi, yi];
        } else {
            return new List<int>();
        }
    }

    private List<int> GetNearbyPoints(Vector2 pos) {
        // Get points inside the search radius of pos.
        // This returns the entire 2x2 neighborhood of pos, so some points
        // may not strictly be within SearchRadius, but all points within
        // SearchRadius will be returned.
        var nearby_points = new List<int>();

        int xi = Utils.Fastfloor(pos.x*Size);
        int yi = Utils.Fastfloor(pos.y*Size);

        int xo = (pos.x - xi > 0.5f ? 1 : -1);
        int yo = (pos.y - yi > 0.5f ? 1 : -1);

        nearby_points.AddRange(GetChunk(xi, yi));
        nearby_points.AddRange(GetChunk(xi + xo, yi));
        nearby_points.AddRange(GetChunk(xi, yi + yo));
        nearby_points.AddRange(GetChunk(xi + xo, yi + yo));

        return nearby_points;
    }

    public float Evaluate(Vector2 pos, float[] values, Vector2[] gradients) {
        List<int> nearby_points = GetNearbyPoints(pos);
        // Leave early if we're too far outside the unit square.
        if (pos.x < -SearchRadius || pos.x > 1.0f + SearchRadius || pos.y < -SearchRadius || pos.y > 1.0f + SearchRadius) {
            return 0.0f;
        }

        // Compute weights for each point.
        List<float> weights = new List<float>();
        List<int> indices = new List<int>();
        float weight_sum = 0.0f;
        foreach (int index in nearby_points) {
            Vector2 point = Points[index];
            float distance = (pos - point).magnitude;
            if (distance == 0.0f) {  // Exit early to avoid division by zero.
                return values[index];
            }
            float w = (1.0f - Utils.Smootherstep(distance/SearchRadius))/Mathf.Pow(distance, Exponent);
            weight_sum += w;
            weights.Add(w);
            indices.Add(index);
        }

        // Sum the contributions from each point.
        float value = 0.0f;
        for (int i = 0; i < indices.Count; i++) {
            int index = indices[i];
            Vector2 point = Points[index];
            float t = weights[i]/weight_sum;
            value += t*(values[index] + t*Vector2.Dot(pos - point, gradients[index]));
        }

        return value;
    }
}
