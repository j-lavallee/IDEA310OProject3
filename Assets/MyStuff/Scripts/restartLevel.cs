using UnityEngine;
using UnityEngine.SceneManagement;

public class restartLevel : MonoBehaviour
{
    public void restart()
    {
        string restartScene = PlayerPrefs.GetString("LastScene");

        if (!string.IsNullOrEmpty(restartScene))
        {
            SceneManager.LoadScene(restartScene);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }

    public void exit()
    {
        SceneManager.LoadScene(0);
    }
}
