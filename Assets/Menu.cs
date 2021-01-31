using System.Collections;
using System.Collections.Generic;
using Tools;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public Game game;



    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            game.LoadNextLevel();
        }
    }
}
