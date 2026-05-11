/*******************************************************
* Script:      NightWaveSpawner.cs
* Author(s):   Alexander Art
* 
* Description:
*    Script that spawns enemies when it turns night.
*******************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NightWaveSpawner : MonoBehaviour
{
    [SerializeField, Tooltip("The tilemap to search for empty tiles before spawning enemies.")]
    private Tilemap wallTilemap;
    [SerializeField, Tooltip("The tilemap to check that the terrain has been generated.")]
    private Tilemap floorTilemap;

    [SerializeField, Tooltip("The prefab for the bounty hunter to be spawned when it turns night.")]
    private GameObject bountyHunterPrefab;

    private DayNightHandler dayNightHandler;
    private GameObject playerInstance;
    private Camera cameraInstance;
    System.Random random = new System.Random();

    bool isNight = true;

    void Awake()
    {
        dayNightHandler = GameObject.FindAnyObjectByType<DayNightHandler>().GetComponent<DayNightHandler>();
        playerInstance = GameObject.FindWithTag("Player");
        cameraInstance = GameObject.FindAnyObjectByType<Camera>().GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check for when it turns night
        // THERE IS A BETTER WAY TO DO THIS THAN TO CHECK THIS EVERY FRAME. USE UNITYEVENTS OR SOMETHING! im being lazy
        if (!isNight && dayNightHandler.IsNight())
        {
            int playerTileX = (int)Math.Floor(playerInstance.transform.position.x);
            int playerTileY = (int)Math.Floor(playerInstance.transform.position.y);

            int halfCameraTileWidth = (int)Math.Ceiling(cameraInstance.orthographicSize * cameraInstance.aspect) + 1;
            int halfCameraTileHeight = (int)Math.Ceiling(cameraInstance.orthographicSize) + 1;

            // Search the area immediately outside of the player's view for a place to spawn a bounty hunter
            List<Vector2Int> candidates = new List<Vector2Int>();
            int x, y;
            // Top row
            y = playerTileY + halfCameraTileHeight;
            for (x = playerTileX - halfCameraTileWidth; x <= playerTileX + halfCameraTileWidth; x++)
            {
                // If the indexed tile has been generated but is empty
                if (floorTilemap.GetTile(new Vector3Int(x, y, 0)) != null && wallTilemap.GetTile(new Vector3Int(x, y, 0)) == null)
                {
                    candidates.Add(new Vector2Int(x, y));
                }
            }
            // Left column
            x = playerTileX - halfCameraTileWidth + 1;
            for (y = playerTileY - halfCameraTileHeight + 1; y < playerTileY + halfCameraTileHeight; y++)
            {
                // If the indexed tile has been generated but is empty
                if (floorTilemap.GetTile(new Vector3Int(x, y, 0)) != null && wallTilemap.GetTile(new Vector3Int(x, y, 0)) == null)
                {
                    candidates.Add(new Vector2Int(x, y));
                }
            }
            // Right column
            x = playerTileX + halfCameraTileWidth - 1;
            for (y = playerTileY - halfCameraTileHeight + 1; y < playerTileY + halfCameraTileHeight; y++)
            {
                // If the indexed tile has been generated but is empty
                if (floorTilemap.GetTile(new Vector3Int(x, y, 0)) != null && wallTilemap.GetTile(new Vector3Int(x, y, 0)) == null)
                {
                    candidates.Add(new Vector2Int(x, y));
                }
            }
            // Bottom row
            y = playerTileY - halfCameraTileHeight;
            for (x = playerTileX - halfCameraTileWidth; x <= playerTileX + halfCameraTileWidth; x++)
            {
                // If the indexed tile has been generated but is empty
                if (floorTilemap.GetTile(new Vector3Int(x, y, 0)) != null && wallTilemap.GetTile(new Vector3Int(x, y, 0)) == null)
                {
                    candidates.Add(new Vector2Int(x, y));
                }
            }

            // Spawn the bounty hunter in a random valid location
            if (candidates.Count != 0)
            {
                Vector2Int location = candidates[random.Next(candidates.Count)];
                Instantiate(bountyHunterPrefab, new Vector3(location.x + 0.5f, location.y + 0.5f, -0.5f), UnityEngine.Quaternion.identity);
            }
        }

        isNight = dayNightHandler.IsNight();
    }
}
