using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    public string LevelToLoad;
    private bool continueScene = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (!continueScene)
            {
                continueScene = true;
                GetComponent<Animator>().SetBool("continue", true);
            }
        }
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene(LevelToLoad);
    }
}
