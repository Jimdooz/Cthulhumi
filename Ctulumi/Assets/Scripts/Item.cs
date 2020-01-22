using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public Sprite visual;
    private SpriteRenderer render;

    public string methodCall;
    // Start is called before the first frame update
    void Start()
    {
        render = GetComponent<SpriteRenderer>();
        render.sprite = visual;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Player player = col.gameObject.GetComponent<Player>();
            if (player)
            {
                System.Type thisType = player.GetType();
                System.Reflection.MethodInfo theMethod = thisType.GetMethod(methodCall);
                theMethod.Invoke((object)player, null);
                Destroy(this.gameObject);
            }
        }
    }
}
