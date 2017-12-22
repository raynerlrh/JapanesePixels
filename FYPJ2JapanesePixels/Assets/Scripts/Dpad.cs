using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dpad : MonoBehaviour 
{
    public enum MOVE_DIR
    {
        NONE,
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

    public MOVE_DIR moveDir { get; set; }
    private bool pressed;

    PlayerMoveController player;

    void Start()
    {
        player = PlayerMoveController.instance;
        pressed = false;
    }

    public void PressUp(bool _pressed)
    {
        if (_pressed)
            moveDir = MOVE_DIR.UP;
        else
            moveDir = MOVE_DIR.NONE;
        pressed = _pressed;
        
    }

    public void PressDown(bool _pressed)
    {
        if (_pressed)
            moveDir = MOVE_DIR.DOWN;
        else
            moveDir = MOVE_DIR.NONE;
        pressed = _pressed;
    }

    public void PressLeft(bool _pressed)
    {
        if (_pressed)
            moveDir = MOVE_DIR.LEFT;
        else
            moveDir = MOVE_DIR.NONE;
        pressed = _pressed;
    }

    public void PressRight(bool _pressed)
    {
        if (_pressed)
            moveDir = MOVE_DIR.RIGHT;
        else
            moveDir = MOVE_DIR.NONE;
        pressed = _pressed;
    }

    public void ResetDir()
    {
        if (moveDir != MOVE_DIR.NONE)
            moveDir = MOVE_DIR.NONE;
    }
}
