using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeTile : MonoBehaviour
{
    [SerializeField] string sceneName;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
