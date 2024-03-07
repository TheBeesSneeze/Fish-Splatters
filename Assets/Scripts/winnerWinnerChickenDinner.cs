/*******************************************************************************
 * File Name :         winnerWinnerChickenDinner.cs
 * Author(s) :         Tyler
 * Creation Date :     
 *
 * Brief Description : 
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class winnerWinnerChickenDinner : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        InputManager player = other.GetComponent<InputManager>();
        if (player != null) {
            SceneManager.LoadScene("WinScene");
        }
    }
}
