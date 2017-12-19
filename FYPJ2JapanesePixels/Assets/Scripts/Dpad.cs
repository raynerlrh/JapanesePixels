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

    PlayerMoveController player;

    void Start()
    {
        player = PlayerMoveController.instance;
    }

    public void PressUp(bool _pressed)
    {
        if (_pressed)
            moveDir = MOVE_DIR.UP;
        else
            moveDir = MOVE_DIR.NONE;
    }

    public void PressDown(bool _pressed)
    {
        if (_pressed)
            moveDir = MOVE_DIR.DOWN;
        else
            moveDir = MOVE_DIR.NONE;
    }

    public void PressLeft(bool _pressed)
    {
        if (_pressed)
            moveDir = MOVE_DIR.LEFT;
        else
            moveDir = MOVE_DIR.NONE;
    }

    public void PressRight(bool _pressed)
    {
        if (_pressed)
            moveDir = MOVE_DIR.RIGHT;
        else
            moveDir = MOVE_DIR.NONE;
    }
}
