using UnityEngine;
using UnityEngine.SceneManagement;

public class buttons : MonoBehaviour
{
    public string level = "Level1";
    public string tutorial = "Tutorial";

    public void Play()
    {
        SceneManager.LoadScene(level);
    }

    public void Tutorial()
    {
        SceneManager.LoadScene(tutorial);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
