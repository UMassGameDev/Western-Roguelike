/*******************************************************
* Script:      Pathfinder.cs
* Author(s):   Alexander Art
* 
* Description:
*    Modified from tutorial: https://www.youtube.com/watch?v=i0x5fj4PqP4
*    Call the FindPath function to calculate a path using A*.
*    The current implementation is not optimized and it does not work on an infinite grid.
*******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

// Followed: https://www.youtube.com/watch?v=i0x5fj4PqP4
public class NodeBase {
	public NodeBase Connection { get; private set; }
	public float G { get; private set; }
	public float H { get; private set; }
	public float F => G + H;
	public Vector2Int coords;
	public Tilemap tilemap;
	public Dictionary<Vector2Int, NodeBase> nodeCache;
	public Vector2Int origin;
	public bool walkable;
	
	public NodeBase(Vector2Int coords, Tilemap tilemap, Dictionary<Vector2Int, NodeBase> nodeCache, Vector2Int origin) {
		this.coords = coords;
		this.tilemap = tilemap;
		this.nodeCache = nodeCache;
		this.origin = origin; // The coordinates of the starting point of the pathfinding. Only used for limiting pathfinding distance.
		
		// Restrict the pathfinding calculation to a 20x12 area around the pathfinding starting point (to prevent searching an infinite grid)
		// TODO: check that the 20x12 area is centered around the point I expect (could be like 0.5 tiles off or something)
		if (coords.x - origin.x < -10 || coords.x - origin.x > 10 || coords.y - origin.y < -6 || coords.y - origin.y > 6) {
			this.walkable = false;
		}
		else {
			TileBase tileItem = tilemap.GetTile(new Vector3Int(coords.x, coords.y, 0));

			// Walkable only if the tile ID is null
			this.walkable = tileItem == null;
		}
	}
	
	public void SetConnection(NodeBase nodeBase) => Connection = nodeBase;
	public void SetG(float g) => G = g;
	public void SetH(float h) => H = h;
	
	public List<NodeBase> GetNeighbors() {
		List<NodeBase> neighbors = new List<NodeBase>();
		
		for (int ry = -1; ry <= 1; ++ry) {
			for (int rx = -1; rx <= 1; ++rx) {
				if (rx == 0 && ry == 0) continue;
				
				Vector2Int neighborCoords = coords + new Vector2Int(rx, ry);
				if (!nodeCache.TryGetValue(neighborCoords, out NodeBase neighbor)) {
					neighbor = new NodeBase(neighborCoords, tilemap, nodeCache, origin);
					nodeCache[neighborCoords] = neighbor;
				}
				
				// Add extra walkable check (skip diagonals if blocked (not null means blocked))
				if (neighbor.walkable && rx != 0 && ry != 0) {
					if (tilemap.GetTile(new Vector3Int(coords.x + rx, coords.y, 0)) != null || tilemap.GetTile(new Vector3Int(coords.x, coords.y + ry, 0)) != null) {
						continue;
					}
				}
				
				neighbors.Add(neighbor);
			}
		}
		
		return neighbors;
	}
	
	public float GetDistance(NodeBase otherNode) {
		float distance = 0;
		
		int dx = Math.Abs(otherNode.coords.x - this.coords.x);
		int dy = Math.Abs(otherNode.coords.y - this.coords.y); // This is actually for the z-axis
		
		if (dx < dy) {
			distance = dx * 1.414f + dy - dx;
		}
		else {
			distance = dy * 1.414f + dx - dy;
		}
		
		return distance;
	}
}

public partial class Pathfinder : MonoBehaviour {
	private Dictionary<Vector2Int, NodeBase> nodeCache;
	
	// Followed: https://www.youtube.com/watch?v=i0x5fj4PqP4
	public List<Vector2Int> FindPath(Vector2Int start, Vector2Int target, Tilemap tilemap) {
		nodeCache = new Dictionary<Vector2Int, NodeBase>();
		
		NodeBase startNode = new NodeBase(start, tilemap, nodeCache, start);
		nodeCache[start] = startNode;
		
		NodeBase targetNode = new NodeBase(target, tilemap, nodeCache, start);
		
		var toSearch = new List<NodeBase>() { startNode };
		var processed = new List<NodeBase>();
		
		while (toSearch.Any()) {
			var current = toSearch[0];
			foreach (var t in toSearch)
				if (t.F < current.F || t.F == current.F && t.H < current.H)
					current = t;
			
			processed.Add(current);
			toSearch.Remove(current);
			
			if (current.coords == targetNode.coords) {
				NodeBase currentPathTile = nodeCache[targetNode.coords];
				List<Vector2Int> path = new List<Vector2Int>();
				while (currentPathTile != startNode) {
					path.Add(currentPathTile.coords);
					currentPathTile = currentPathTile.Connection;
				}
				path.Reverse();
				return path;
			}
			
			foreach (var neighbor in current.GetNeighbors().Where(t => t.walkable && !processed.Contains(t))) {
				var inSearch = toSearch.Contains(neighbor);
				
				var costToNeighbor = current.G + current.GetDistance(neighbor);
				
				if (!inSearch || costToNeighbor < neighbor.G) {
					neighbor.SetG(costToNeighbor);
					neighbor.SetConnection(current);
					
					if (!inSearch) {
						neighbor.SetH(neighbor.GetDistance(targetNode));
						toSearch.Add(neighbor);
					}
				}
			}
		}
		
		return null;
	}
}
