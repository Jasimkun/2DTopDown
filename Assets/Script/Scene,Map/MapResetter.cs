using UnityEngine;
using UnityEngine.SceneManagement;

public class MapResetter : MonoBehaviour
{
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
