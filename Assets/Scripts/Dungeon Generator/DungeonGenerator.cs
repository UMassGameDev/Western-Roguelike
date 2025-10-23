using System.Data;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Numerics;

// Dungeon Generator modeled after the generator found here, translated from DART to C#:
// https://github.com/munificent/hauberk/blob/db360d9efa714efb6d937c31953ef849c7394a39/lib/src/content/dungeon.dart
public class DungeonGenerator : MonoBehaviour
{
    private static readonly Vector2Int[] CardinalDirections = new Vector2Int[]
    {
        new Vector2Int(0, 1),   // North / Up
        new Vector2Int(0, -1),  // South / Down
        new Vector2Int(1, 0),   // East / Right
        new Vector2Int(-1, 0)   // West / Left
    };

    public enum Tile { Wall, Floor }

    [Header("Player:")]
    [SerializeField, Tooltip("The player object spawned in the scene.")] 
    private GameObject playerPrefab;

    [Header("Rendered tiles:")]
    [SerializeField, Tooltip("Tile used to represent walls.")]
    private TileBase wallTile;
    [SerializeField, Tooltip("Tile used to represent floors.")]
    private TileBase floorTile;

    [Header("Dungeon Parameters:")]
    [SerializeField, Tooltip("Max width of generated dungeon.")]
    private int dungeonWidth = 51;
    [SerializeField, Tooltip("Max height of generated dungeon.")]
    private int dungeonHeight = 51;

    [SerializeField, Tooltip("# of tries to generate rooms.")]
    private int numRoomTries = 50;
    [SerializeField, Tooltip("Added onto max value of initial room size.")]
    private int roomExtraSize = 0;
    [SerializeField, Tooltip("How many turns in passageways between rooms, 0-100%."), Range(0, 100)]
    private int windingPercent = 0;
    [SerializeField, Tooltip("How much bigger the buffer is than the dungeon.")]
    private int bufferScale = 2;

    private Tile[,] tiles;
    private Tile[,] buffer;

    private int[,] regions;
    private int currentRegion = -1;

    private List<RectInt> rooms = new List<RectInt>();



    void Awake()
    {
        Generate();
        RenderDungeon();
        PlacePlayer();
    }

    //~(Generate)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Calls all functions needed to properly generate the dungeon.
    // This is a big one, so I'll describe the algorithm and how it is used.
    // To Generate(), 
    private void Generate()
    {
        tiles = new Tile[dungeonWidth, dungeonHeight];
        buffer = new Tile[dungeonWidth * bufferScale, dungeonHeight * bufferScale];
        Fill(Tile.Wall);

        regions = new int[dungeonWidth, dungeonHeight];
        AddRooms();
        FillWithMazes();
        ConnectRegions();
        RemoveDeadEnds();
    }

    //~(Fill)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Fills <tiles[x, y]> & <buffer[x, y]> with wall tiles by default.
    private void Fill(Tile fillTile)
    {
        for (int x = 0; x < dungeonWidth; x++)
        {
            for (int y = 0; y < dungeonHeight; y++)
            {
                tiles[x, y] = fillTile;
            }
        }
        for (int x = 0; x < dungeonWidth * bufferScale; x++)
        {
            for (int y = 0; y < dungeonHeight * bufferScale; y++)
            {
                buffer[x, y] = fillTile;
            }
        }
    }

    //~(AddRooms)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Carves rectangles out of 
    private void AddRooms()
    {
        for (int i = 0; i < numRoomTries; i++)
        {
            // Create randomly sized square and variation (<rectangularity>).
            int squareSize = randSquareSize();
            int variation = randRectangularity(squareSize);

            int roomWidth, roomHeight;
            roomWidth = roomHeight = squareSize;

            // Randomly decide through 50/50 odds what side gets the variation.
            if (Random.value < 0.5f) { roomWidth += variation; }
            else { roomHeight += variation; }

            // Find a random odd x & y coord for this rooms top left corner.
            int x = Random.Range(0, (dungeonWidth - roomWidth) / 2) * 2 + 1;
            int y = Random.Range(0, (dungeonHeight - roomHeight) / 2) * 2 + 1;

            // Create a RectInt to check for overlaps.
            RectInt room = new RectInt(x, y, roomWidth, roomHeight);

            // If this overlaps with another room, restart the loop.
            bool overlaps = rooms.Any(r => r.Overlaps(room));
            if (overlaps) { continue; }

            // Add room to list of currently placed rooms, to check other rooms against for overlap.
            rooms.Add(room);

            // Creates a region for this room.
            StartRegion();

            // Carves out the room.
            for (int rx = x; rx < x + roomWidth; rx++)
            {
                for (int ry = y; ry < y + roomHeight; ry++)
                {
                    Carve(new Vector2Int(rx, ry));
                }
            }
        }
    }
    // Generates a random size for the base square.
    int randSquareSize() => (Random.Range(1, 3 + roomExtraSize) * 2) + 1;
    // Generates a random variation to be added to the dungeonWidth or dungeonHeight of the square.
    int randRectangularity(int squareSize) => Random.Range(0, 1 + squareSize / 2) * 2;

    //~(FillWithMazes)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Calls GrowMaze() on every odd numbered tile that is not already a floor (Odd numbered to keep).
    private void FillWithMazes()
    {
        for (int y = 1; y < dungeonHeight; y += 2)
        {
            for (int x = 1; x < dungeonWidth; x += 2)
            {
                if (!(tiles[x, y] == Tile.Wall)) { continue; }
                GrowMaze(new Vector2Int(x, y));
            }
        }
    }

    private void GrowMaze(Vector2Int start)
    {
        List<Vector2Int> cells = new List<Vector2Int>();
        Vector2Int? lastDir = null;

        StartRegion();
        Carve(start);

        cells.Add(start);
        while (cells.Count > 0)
        {
            Vector2Int cell = cells[cells.Count - 1];
            List<Vector2Int> unmade = new List<Vector2Int>();

            foreach (var dir in CardinalDirections)
            {
                if (CanCarve(cell, dir))
                {
                    unmade.Add(dir);
                }
            }

            if (unmade.Count > 0)
            {
                Vector2Int dir;
                if (lastDir.HasValue && unmade.Contains(lastDir.Value) && Random.Range(0, 100) > windingPercent)
                {
                    dir = lastDir.Value;
                }
                else
                {
                    dir = unmade[Random.Range(0, unmade.Count)];
                }
                Carve(cell + dir);
                Carve(cell + dir * 2);
                cells.Add(cell + dir * 2);
                lastDir = dir;
            }
            else
            {
                cells.RemoveAt(cells.Count - 1);
                lastDir = null;
            }
        }
    }

    private bool CanCarve(Vector2Int cell, Vector2Int dir)
    {
        Vector2Int next = cell + dir * 3;

        if (!IsInBounds(next)) { return false; }

        return tiles[cell.x + dir.x * 2, cell.y + dir.y * 2] == Tile.Wall;
    }

    private bool IsInBounds(Vector2Int pos)
    {
        return pos.x > 0 && pos.x < dungeonWidth - 1 && pos.y > 0 && pos.y < dungeonHeight - 1;
    }

    //~(ConnectRegions)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    private void ConnectRegions()
    {
        List<Vector2Int> connectorTiles = new List<Vector2Int>();

        for (int x = 1; x < dungeonWidth - 1; x++)
        {
            for (int y = 1; y < dungeonHeight - 1; y++)
            {
                if (tiles[x, y] != Tile.Wall) { continue; }

                HashSet<int> adjacentRegions = new HashSet<int>();

                foreach (var dir in CardinalDirections)
                {
                    Vector2Int neighbor = new Vector2Int(x + dir.x, y + dir.y);
                    int regionId = regions[neighbor.x, neighbor.y];

                    if (tiles[neighbor.x, neighbor.y] == Tile.Floor) { adjacentRegions.Add(regionId); }
                }

                if (adjacentRegions.Count >= 2) { connectorTiles.Add(new Vector2Int(x, y)); }
            }
        }

        System.Random rand = new System.Random();
        HashSet<int> mergedRegions = new HashSet<int>();
        Dictionary<int, int> regionMapping = new Dictionary<int, int>();

        int FindRoot(int id)
        {
            while (regionMapping.ContainsKey(id))
            {
                id = regionMapping[id];
            }
            return id;
        }

        foreach (var connector in connectorTiles.OrderBy(x => rand.Next()))
        {
            HashSet<int> adjacent = new HashSet<int>();

            foreach (var dir in CardinalDirections)
            {
                Vector2Int neighbor = connector + dir;
                if (tiles[neighbor.x, neighbor.y] == Tile.Floor) { adjacent.Add(FindRoot(regions[neighbor.x, neighbor.y])); }
            }

            if (adjacent.Count < 2) { continue; }

            Carve(connector);

            int[] list = adjacent.ToArray();
            int root = list[0];

            foreach (int id in list.Skip(1)) { regionMapping[id] = root; }
        }

        foreach (var connector in connectorTiles)
        {
            if (tiles[connector.x, connector.y] == Tile.Wall && UnityEngine.Random.value < 0.05f) { Carve(connector); }
        }
    }

    //~(RemoveDeadEnds)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    void RemoveDeadEnds()
    {
        bool done = false;
        while (!done)
        {
            done = true;
            for (int x = 1; x < dungeonWidth - 1; x++)
            {
                for (int y = 1; y < dungeonHeight - 1; y++)
                {
                    if (tiles[x, y] == 0) { continue; }
                    int exits = 0;
                    foreach (var dir in CardinalDirections)
                    {
                        if (tiles[x + dir.x, y + dir.y] != 0) { exits++; }
                    }
                    if (exits != 1) { continue; }

                    done = false;
                    tiles[x, y] = 0;
                }
            }
        }
    }

    //~(PlacePlayer)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    private void PlacePlayer()
    {
        List<Vector2Int> floorPositions = new List<Vector2Int>();

        for (int x = 0; x < dungeonWidth; x++)
        {
            for (int y = 0; y < dungeonHeight; y++)
            {
                if (tiles[x, y] == Tile.Floor) { floorPositions.Add(new Vector2Int(x, y)); }
            }
        }

        if (floorPositions.Count == 0) { return; }

        Vector2Int spawnPos = floorPositions[Random.Range(0, floorPositions.Count)];

        if (playerPrefab != null)
        {
            GameObject playerInstance = Instantiate(playerPrefab);
            playerInstance.transform.position = new UnityEngine.Vector3(spawnPos.x + 0.5f + -dungeonWidth / 2, spawnPos.y + 0.5f + -dungeonHeight / 2, -1);
        }
        else
        {
            Debug.LogWarning("Player prefab not assigned!");
        }
    }

    //~(RenderDungeon)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    private void RenderDungeon()
    {
        var tilemap = GetComponent<Tilemap>();
        int offsetX = -dungeonWidth / 2;
        int offsetY = -dungeonHeight / 2;

        for (int x = 0; x < dungeonWidth * bufferScale; x++)
        {
            for (int y = 0; y < dungeonHeight * bufferScale; y++)
            {
                TileBase tileAsset = null;

                if (buffer[x, y] == Tile.Wall) { tileAsset = wallTile; }
                else if (buffer[x, y] == Tile.Floor) { tileAsset = floorTile; }
                if (tileAsset != null) { tilemap.SetTile(new Vector3Int(x + offsetX * 2, y + offsetY * 2, 0), tileAsset); }
            }
        }

        for (int x = 0; x < dungeonWidth; x++)
        {
            for (int y = 0; y < dungeonHeight; y++)
            {
                TileBase tileAsset = null;

                if (tiles[x, y] == Tile.Wall) { tileAsset = wallTile; }
                else if (tiles[x, y] == Tile.Floor) { tileAsset = floorTile; }
                if (tileAsset != null) { tilemap.SetTile(new Vector3Int(x + offsetX, y + offsetY, 0), tileAsset); }
            }
        }
    }

    //~(HelperMethods)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Makes tile at <pos> a floor tile, and adds it to the region array.
    private void Carve(Vector2Int pos, Tile type = Tile.Floor)
    {
        tiles[pos.x, pos.y] = type;
        regions[pos.x, pos.y] = currentRegion;
    }

    // Creates a unique number for the new region.
    private void StartRegion() => currentRegion++;
}
