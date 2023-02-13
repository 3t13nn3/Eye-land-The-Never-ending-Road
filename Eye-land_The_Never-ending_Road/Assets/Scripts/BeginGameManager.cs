using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BeginGameManager : MonoBehaviour
{
    // Start is called before the first frame update
    
    public static void loadMenuScene()
    {
        Time.timeScale = 0;
        SceneManager.LoadScene("MenuScene", LoadSceneMode.Additive);
    }
    
    public void playGame()
    {
        Time.timeScale = 1;
        SceneManager.UnloadSceneAsync("MenuScene");
        
    }

   
}
