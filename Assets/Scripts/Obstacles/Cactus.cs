/*******************************************************
* Script:      Cactus.cs
* Author(s):   Alexander Art
* 
* Description:
*    Basic function(s) for cacti objects.
*******************************************************/

using UnityEngine;

public class Cactus : MonoBehaviour
{
    // Function for instantiating particles at the cactus's position
    public void instantiateParticles(ParticleSystem particles)
    {
        Instantiate(particles, transform.position, Quaternion.identity);
    }
}
