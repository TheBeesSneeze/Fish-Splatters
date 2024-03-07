/*******************************************************************************
 * File Name :         MenuButtons.cs
 * Author(s) :         toby
 * Creation Date :     3/6/2024
 *
 * Brief Description : 
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public string LevelName="Segment A";

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void StartGame()
    {
        SceneManager.LoadScene(LevelName);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
