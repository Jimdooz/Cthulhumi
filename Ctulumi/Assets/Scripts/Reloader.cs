using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Reloader : MonoBehaviour
{

    public Player player;
    public float minBase;
    // Start is called before the first frame update
    void Start()
    {
        if (!GameObject.Find("MAIN_AUDIO"))
        {
            GameObject.DontDestroyOnLoad(gameObject);
            gameObject.name = "MAIN_AUDIO";
        }
        else
        {
            GetComponent<AudioSource>().Stop();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(player.transform.position.y < minBase)
        {
            reloadScene();
           
        }
        if (player != null && player.IsDead())
        {
            player = null;
            StartCoroutine("Reload");
        }
    }

    void reloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(3);
        reloadScene();
    }
}
