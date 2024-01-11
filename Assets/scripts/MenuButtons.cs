using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public Button play;
    public Button quit;

    public void playgame()
    {
        SceneManager.LoadScene("player type");
    }

    public void quitgame()
    {
        Application.Quit();
    }

    public void singleplayer()
    {
        SceneManager.LoadScene("levels");
    }

    public void easy()
    {
        PlayerPrefs.SetString("Difficulty", "Easy");
        SceneManager.LoadScene("fight");
    }

    public void normal()
    {
        PlayerPrefs.SetString("Difficulty", "Normal");
        SceneManager.LoadScene("fight");
    }

    public void hard()
    {
        PlayerPrefs.SetString("Difficulty", "Hard");
        SceneManager.LoadScene("fight");
    }
}
