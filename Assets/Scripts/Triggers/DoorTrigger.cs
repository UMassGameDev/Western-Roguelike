using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTrigger : MonoBehaviour
{
    // This [] is an attribute, attributes mostly provide additional info in the editor.
    [SerializeField, Tooltip("Scene this door loads into. Default = \"MainScene\"")] 
    private string targetScene = "MainScene";

    // Detects if <other> is player, and if so, loads <targetScene> with a fade effect:
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneLoader.Instance.LoadSceneWithFade(targetScene);
        }
    }
}