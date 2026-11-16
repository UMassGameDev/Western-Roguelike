/*******************************************************
* Script:      Cactus.cs
* Author(s):   Alexander Art
* 
* Description:
*    Basic function(s) for cacti objects.
*******************************************************/

using System.Collections.Generic;
using UnityEngine;

public class Cactus : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [SerializeField, Tooltip("The cactus's sprite is randomly selected from this list.")]
    private List<Sprite> sprites;

    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Count)];
    }

    // Function for instantiating particles at the cactus's position
    public void instantiateParticles(ParticleSystem particles)
    {
        Instantiate(particles, transform.position, Quaternion.identity);
    }
}
