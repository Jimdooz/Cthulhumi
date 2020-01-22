using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Reloader : MonoBehaviour
{

    public Player player;
    public float minBase;
    bool reloading = false;
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
        if (!reloading && player!=null && player.IsDead())
        {
            reloading = true;
            Reload(3);
        }
    }

    void reloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Reload(float t)
    {
        StartCoroutine("ReloadCoroutine",t);
    }
    IEnumerator ReloadCoroutine(float t)
    {
        yield return new WaitForSeconds(t);
        reloadScene();
    }
}
