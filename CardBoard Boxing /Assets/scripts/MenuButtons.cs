using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public Button play;
    public Button quit;

    // Load the "player type" scene when the play button is clicked.
    public void playgame()
    {
        SceneManager.LoadScene("player type");
    }

    // Quit the application when the quit button is clicked.
    public void quitgame()
    {
        Application.Quit();
    }

    // Load the "levels" scene when the singleplayer button is clicked.
    public void singleplayer()
    {
        SceneManager.LoadScene("levels");
    }

    // Set the difficulty to "Easy" and load the "fight" scene when the easy button is clicked.
    public void easy()
    {
        PlayerPrefs.SetString("Difficulty", "Easy");
        SceneManager.LoadScene("fight");
    }

    // Set the difficulty to "Normal" and load the "fight" scene when the normal button is clicked.
    public void normal()
    {
        PlayerPrefs.SetString("Difficulty", "Normal");
        SceneManager.LoadScene("fight");
    }

    // Set the difficulty to "Hard" and load the "fight" scene when the hard button is clicked.
    public void hard()
    {
        PlayerPrefs.SetString("Difficulty", "Hard");
        SceneManager.LoadScene("fight");
    }
}
