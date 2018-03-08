using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMapGrid {
	int PolygonCount {
		get;
	}

	int VertexCount {
		get;
	}

	List<int> GetNeighbors(int polygon);
	List<int> GetCorners(int polygon);
	List<int> GetAdjacent(int vertex);
	List<int> GetTouches(int vertex);
}
