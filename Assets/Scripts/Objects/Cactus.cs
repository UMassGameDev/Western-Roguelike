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
    //~(Destroy)~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // This function is attached to the cactus prefab's objectHealth OnDeath event
    // so that the cactus is destroyed when its health reaches 0.
    public void Destroy()
    {
        GameObject.Destroy(this.gameObject);
    }
}
