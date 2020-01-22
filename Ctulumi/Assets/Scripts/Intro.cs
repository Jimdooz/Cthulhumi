using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    // Start is called before the first frame update

    private int position = 0;
    private bool canPressKey = true;
    public string LevelToLoad;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown && canPressKey)
        {
            canPressKey = false;
            position++;
            GetComponent<Animator>().SetInteger("continue", position);

            if(position == 4)
            {
                SceneManager.LoadScene(LevelToLoad);
            }
        }

        if (!Input.anyKeyDown)
        {
            canPressKey = true;
        }
    }
}
