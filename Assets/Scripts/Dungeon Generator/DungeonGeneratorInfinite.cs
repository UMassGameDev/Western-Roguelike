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

    // Seed works as an offset rather than an actual seed because Mathf.PerlinNoise does not support seeding
    private int seedX;
    private int seedY;

    void Awake()
    {
        seedX = UnityEngine.Random.Range(1000, 1000000);
        seedY = UnityEngine.Random.Range(1000, 1000000);
        PlacePlayer();
    }

    void Update()
    {
        int playerTileX = (int)Math.Floor(playerInstance.transform.position.x);
        int playerTileY = (int)Math.Floor(playerInstance.transform.position.y);

        // Area around the player to generate tiles
        int cameraTileWidth = 20;
        int cameraTileHeight = 12;

        for (int y = playerTileY - cameraTileHeight / 2; y <= playerTileY + cameraTileHeight / 2; y++) {
            for (int x = playerTileX - cameraTileWidth / 2; x <= playerTileX + cameraTileWidth / 2; x++) {
                if (GetTile(x, y) == null) { // Avoids generating same position twice
                    SetTile(x, y, GenerateTile(x, y));
                }
            }
        }
    }

    //~(GetTile)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Get tilemap tile
    private TileBase GetTile(int x, int y)
    {
        return tilemap.GetTile(new Vector3Int(x, y, 0));
    }

    //~(SetTile)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Set tilemap tile
    private void SetTile(int x, int y, TileBase tileAsset)
    {
        tilemap.SetTile(new Vector3Int(x, y, 0), tileAsset);
    }

    //~(SetTile)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // Returns which tile should be generated at the passed coordinate
    private TileBase GenerateTile(int x, int y)
    {
        float tileNoise = Mathf.PerlinNoise(x * 0.05f + seedX, y * 0.05f + seedY);
        //Debug.Log(tileNoise * 1000000 % 1); // Random value between 0 and 1 instead of smooth value
        if (tileNoise < 0.5)
        {
            return floorTile;
        }
        else
        {
            return wallTile;
        }
    }

    //~(PlacePlayer)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    
    private void PlacePlayer()
    {
        Vector2Int spawnPos = new Vector2Int(0, 0);

        if (playerInstance != null)
        {
            playerInstance.transform.position = new UnityEngine.Vector3(spawnPos.x + 0.5f, spawnPos.y + 0.5f, -1);
        }
        else
        {
            Debug.LogWarning("Player instance not assigned!");
        }
    }
}
