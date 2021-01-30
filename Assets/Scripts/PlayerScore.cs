using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    public float score; 

    public void IncreaseScore(float value)
    {
        score = score + value;
    }
  
}
