using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public string LevelName="Segment A";
    public void StartGame()
    {
        SceneManager.LoadScene(LevelName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
