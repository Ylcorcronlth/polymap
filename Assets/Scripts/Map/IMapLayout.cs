using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMapLayout {
	List<Vector2> PolygonPositions {
		get;
	}
	List<Vector2> VertexPositions {
		get;
	}
}
