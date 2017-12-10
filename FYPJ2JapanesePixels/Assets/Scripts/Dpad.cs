using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dpad : MonoBehaviour 
{
    public void PressUp(bool _pressed)
    {
        if (_pressed)
            Debug.Log("Up");
    }

    public void PressDown(bool _pressed)
    {
        if (_pressed)
            Debug.Log("Down");
    }

    public void PressLeft(bool _pressed)
    {
        if (_pressed)
            Debug.Log("Left");
    }

    public void PressRight(bool _pressed)
    {
        if (_pressed)
            Debug.Log("Right");
    }


}
