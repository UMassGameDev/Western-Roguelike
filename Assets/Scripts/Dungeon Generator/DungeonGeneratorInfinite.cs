/*******************************************************
* Script:      DungeonGenerator.cs
* Author(s):   Alexander Art, Nicholas Johnson
* 
* Description:
*    This script generates a random roguelike dungeon from a set of tiles
*    on a tilemap.
* 
* Notes:
*    Dungeon Generator originally modeled after the generator found here, translated from DART to C#:
*    https://github.com/munificent/hauberk/blob/db360d9efa714efb6d937c31953ef849c7394a39/lib/src/content/dungeon.dart
*******************************************************/

using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Numerics;

public class DungeonGeneratorInfinite : MonoBehaviour
{
    private static readonly Vector2Int[] CardinalDirections = new Vector2Int[]
    {
        new Vector2Int(0, 1),   // North / Up
        new Vector2Int(0, -1),  // South / Down
        new Vector2Int(1, 0),   // East / Right
        new Vector2Int(-1, 0)   // West / Left
    };

    [Header("Player:")]
    [SerializeField, Tooltip("The player instance in the scene.")] 
    private GameObject playerInstance;

    [Header("Tilemap:")]
    [SerializeField, Tooltip("The tilemap to set tiles to.")]
    private Tilemap tilemap;

    [Header("Rendered tiles:")]
    [SerializeField, Tooltip("Tile used to represent walls.")]
    private TileBase wallTile;
    [SerializeField, Tooltip("Tile used to represent floors.")]
    private TileBase floorTile;

    [Header("Prefabs:")]
    [SerializeField, Tooltip("Prefab for cacti.")]
    private GameObject cactusPrefab;

    [Header("Parameters:")]
    [SerializeField, Tooltip("Radius around the spawn that terrain will not generate. This also determines the spawn building size.")]
    private float spawnAreaRadius = 7f;
    [SerializeField, Tooltip("Likelihood for each tile to attempt to generate a room, 0-100%. Note that failed attempts are common."), Range(0, 1)]
    private float roomDensity = 0.05f;
    [SerializeField, Tooltip("Percentage of floor tiles in plains biomes that generate cacti."), Range(0, 1)]
    private float plainsBiomeCactusPercentage = 0.01f;
    [SerializeField, Tooltip("Percentage of floor tiles in cactus biomes that generate cacti."), Range(0, 1)]
    private float cactusBiomeCactusPercentage = 0.5f;

    // Seed works as an offset rather than an actual seed because Mathf.PerlinNoise does not support seeding
    private int seedX;
    private int seedY;

    void Awake()
    {
        seedX = UnityEngine.Random.Range(100000, 200000);
        seedY = UnityEngine.Random.Range(100000, 200000);
        PlacePlayer();
    }

    void Update()
    {
        int playerTileX = (int)Math.Floor(playerInstance.transform.position.x);
        int playerTileY = (int)Math.Floor(playerInstance.transform.position.y);

        // Area around the player to generate tiles
        int cameraTileWidth = 20;
        int cameraTileHeight = 12;

        for (int y = playerTileY - cameraTileHeight / 2; y <= playerTileY + cameraTileHeight / 2; y++)
        {
            for (int x = playerTileX - cameraTileWidth / 2; x <= playerTileX + cameraTileWidth / 2; x++)
            {
                if (GetTile(x, y) == null) // Avoids generating same position twice
                {
                    // Generate and set the tile
                    SetTile(x, y, GenerateTile(x, y));

                    // Floor tiles outside the spawn area have a chance of generating a cactus
                    if (GetTile(x, y) == floorTile && Mathf.Sqrt(x * x + y * y) > spawnAreaRadius)
                    {
                        // The percentage of cactus generated per tile depends on the biome
                        if (GetBiome(x, y) == 0 && GetRandomNoise2(x, y) < plainsBiomeCactusPercentage)
                        {
                            Instantiate(cactusPrefab, new UnityEngine.Vector3(x + 0.5f, y + 0.5f, -0.5f), UnityEngine.Quaternion.identity);
                        }
                        else if (GetBiome(x, y) == 2 && GetRandomNoise2(x, y) < cactusBiomeCactusPercentage)
                        {
                            Instantiate(cactusPrefab, new UnityEngine.Vector3(x + 0.5f, y + 0.5f, -0.5f), UnityEngine.Quaternion.identity);
                        }
                    }
                }
            }
        }
    }

    //~(GetTile)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Gets tilemap tile
    private TileBase GetTile(int x, int y)
    {
        return tilemap.GetTile(new Vector3Int(x, y, 0));
    }

    //~(SetTile)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Sets tilemap tile
    private void SetTile(int x, int y, TileBase tileAsset)
    {
        tilemap.SetTile(new Vector3Int(x, y, 0), tileAsset);
    }

    //~(SetTile)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Returns which tile should be generated at the passed coordinate.
    private TileBase GenerateTile(int x, int y)
    {
        if (GetBiome(x, y) == 1 || GetSpawnBuilding(x, y) == 1 || GetRoom(x, y) == 1)
        {
            return wallTile;
        }
        else
        {
            return floorTile;
        }
    }

    //~(GetRandomNoise)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Returns random value between 0 and 1 based on the coordinate.
    // Mathf.PerlinNoise does not support actual seeding, so seedX and seedY are used as an offset.
    private float GetRandomNoise(int x, int y)
    {
        return Mathf.PerlinNoise(x * 0.05f + seedX, y * 0.05f + seedY) * 1000000 % 1;
    }

    //~(GetRandomNoise2)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Like GetRandomNoise, but even more random.
    private float GetRandomNoise2(int x, int y)
    {
        return GetRandomNoise((int)Math.Pow(GetRandomNoise(x * x, y * y) * 1000, 2), (int)Math.Pow(GetRandomNoise(x, y) * 1000, 2));
    }

    //~(GetBiome)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Returns which biome to generate at the passed coordinate.
    // 0 for desert plains, 1 for plateau, and 2 for cactus biome.
    private int GetBiome(int x, int y)
    {
        // Perlin noise for biome shapes
        float smoothNoise1 = Mathf.PerlinNoise(x * 0.05f + seedX, y * 0.05f + seedY);
        float smoothNoise2 = Mathf.PerlinNoise(x * 0.05f + seedY * 2, y * 0.05f + seedX);
        float smoothNoise3 = Mathf.PerlinNoise(x * 0.05f + seedY, y * 0.05f + seedX * 2);

        // Ensure that everything from (0, 0) to spawnAreaRadius cannot be plateau,
        // from spawnAreaRadius to 2 * spawnAreaRadius transitions linearly,
        // and anything beyond 2 * spawnAreaRadius is unaffected.
        float spawnBias = Mathf.Min(Mathf.Max(0, -Mathf.Sqrt(x * x + y * y) / spawnAreaRadius + 2), 1);

        if (smoothNoise1 > 0.55f + spawnBias)
        {
            return 1;
        }
        else if (smoothNoise2 > 0.65f && smoothNoise3 > 0.65f)
        {
            return 2;
        }
        else
        {
            return 0;
        }
    }

    //~(GetSpawnBuilding)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Returns true if the passed coordinate is part of the building in the spawn area.
    private float GetSpawnBuilding(int x, int y)
    {
        int wallLength = (int) (Mathf.Sqrt(2f) / 2f * spawnAreaRadius - 1f);

        // If the passed coordinate is part of the wall
        if ((x == wallLength || x == -wallLength || y == wallLength || y == -wallLength) && x < wallLength + 1 && x > -wallLength - 1 && y < wallLength + 1 && y > -wallLength - 1)
        {
            // Leave doors
            if (x != 0 && y != 0)
            {
                // Wall
                return 1;
            }
        }

        // Not wall
        return 0;
    }

    //~(GetRoom)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Determines if a room should be generated at the passed coordinate.
    // Returns 0 for no room and 1 for room.
    // Additionally, if a room is found that has not yet been generated, then the entire room is generated.
    private int GetRoom(int x, int y)
    {
        // This should be an odd number
        int maxRoomSize = 9;

        // Rooms generate on odd tiles, so find the nearest odd tiles
        int prevOddX = x;
        int prevOddY = y;
        int nextOddX = x;
        int nextOddY = y;
        if (x % 2 == 0) { --prevOddX; ++nextOddX; }
        if (y % 2 == 0) { --prevOddY; ++nextOddY; }

        // Search the area around the current tile for buildings:
        // - Buildings only generate on odd tiles
        // - Buildings only generate if tileNoise for that tile is less than roomDensity
        // - If two or more buildings overlap, the building with the greatest tileNoise within a radius of maxRoomSize is the one that gets generated
        float maxRoomValue = 0; // Used for finding the building with the greatest tileNoise
                                // Stays 0 if no buildings are found
        int roomX = 0; // Position of the room with the greatest tileNoise
        int roomY = 0;
        for (int ry = prevOddY - maxRoomSize + 1; ry <= nextOddY; ry += 2)
        {
            for (int rx = prevOddX - maxRoomSize + 1; rx <= nextOddX; rx += 2)
            {
                float tileNoise = GetRandomNoise(rx, ry);
                if (tileNoise < roomDensity && maxRoomValue < tileNoise)
                {
                    maxRoomValue = tileNoise;
                    roomX = rx;
                    roomY = ry;
                }
            }
        }

        // Before placing the building, ensure that no other buildings have a chance of overlapping with it
        bool validPlacement = true;
        for (int ry = roomY - maxRoomSize + 1; ry < roomY + maxRoomSize; ry += 2)
        {
            for (int rx = roomX - maxRoomSize + 1; rx < roomX + maxRoomSize; rx += 2)
            {
                float tileNoise = GetRandomNoise(rx, ry);
                if (tileNoise < roomDensity && maxRoomValue < tileNoise)
                {
                    validPlacement = false;
                    break;
                }
            }

            // Break out of the outer loop as well
            if (!validPlacement)
            {
                break;
            }
        }

        // Room sizes should be odd
        Vector2Int roomSize = GetRoomSize(maxRoomValue * 1000000 % 1);

        // Before placing the building, ensure:
        // - It does not generate in a plateau biome
        // - No part of it generates too close to spawn (0, 0)
        for (int ry = roomY - 1; ry < roomY + roomSize.y + 1; ry++)
        {
            for (int rx = roomX - 1; rx < roomX + roomSize.x + 1; rx++)
            {
                if (GetBiome(rx, ry) == 1)
                {
                    validPlacement = false;
                    break;
                }
                else if (Mathf.Sqrt(rx * rx + ry * ry) < spawnAreaRadius)
                {
                    validPlacement = false;
                    break;
                }
            }

            // Break out of the outer loop as well
            if (!validPlacement)
            {
                break;
            }
        }

        // The tile contains a room if:
        // - A building was found
        // - The placement of the building was validated
        // - The tile at (x, y) is within the bounds of the room
        if (maxRoomValue > 0 && validPlacement && roomX <= x && roomX + roomSize.x > x && roomY <= y && roomY + roomSize.y > y)
        {
            // If the room has not been generated yet, then generate it.
            // Generating the room all at once is more efficient, and it
            // will make it easier to generate the interior of each room.
            if (GetTile(roomX, roomY) == null)
            {
                // Generate the entire room
                for (int ry = roomY; ry < roomY + roomSize.y; ry++)
                {
                    for (int rx = roomX; rx < roomX + roomSize.x; rx++)
                    {
                        SetTile(rx, ry, wallTile);
                    }
                }
            }

            return 1;
        }
        else
        {
            return 0;
        }
    }

    //~(GetRoomSize)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Returns random room size based on random value between 0 and 1.
    // Room sizes should be odd.
    private Vector2Int GetRoomSize(float randomValue)
    {
        Vector2Int roomSize;

        if (randomValue < 0.1f)
        {
            roomSize = new Vector2Int(5, 5);
        }
        else if (randomValue < 0.2f)
        {
            roomSize = new Vector2Int(5, 7);
        }
        else if (randomValue < 0.3f)
        {
            roomSize = new Vector2Int(5, 9);
        }
        else if (randomValue < 0.4f)
        {
            roomSize = new Vector2Int(7, 5);
        }
        else if (randomValue < 0.5f)
        {
            roomSize = new Vector2Int(7, 7);
        }
        else if (randomValue < 0.6f)
        {
            roomSize = new Vector2Int(7, 9);
        }
        else if (randomValue < 0.7f)
        {
            roomSize = new Vector2Int(9, 5);
        }
        else if (randomValue < 0.8f)
        {
            roomSize = new Vector2Int(9, 7);
        }
        else
        {
            roomSize = new Vector2Int(9, 9);
        }

        return roomSize;
    }

    //~(PlacePlayer)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Finds a floor tile to spawn the player at.
    private void PlacePlayer()
    {
        // Player spawn location is always (0, 0)
        int spawnX = 0;
        int spawnY = 0;
        /*
        while (GenerateTile(spawnX, spawnY) != floorTile)
        {
            ++spawnX;
        }
        */

        if (playerInstance != null)
        {
            playerInstance.transform.position = new UnityEngine.Vector3(spawnX + 0.5f, spawnY + 0.5f, -1);
        }
        else
        {
            Debug.LogWarning("Player instance not assigned!");
        }
    }
}
