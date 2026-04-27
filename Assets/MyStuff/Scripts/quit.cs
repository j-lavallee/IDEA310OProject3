using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class quit : MonoBehaviour
{
    public string MainMenu = "MainMenu";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(MainMenu);
        }
    }
}
