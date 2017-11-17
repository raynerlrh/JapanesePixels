using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMenu : MonoBehaviour 
{
    public float speed;
    public GameObject buttons;

    Vector2 originalPos;
    bool disabled;

    void Start()
    {
        originalPos = transform.localPosition;
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        if (disabled)
            buttons.SetActive(false);
    }

    void OnDisable()
    {
        disabled = true;
        buttons.SetActive(true);
        transform.localPosition = originalPos;
    }

    void Update()
    {
        if (PlayerMoveController.instance.NoMovesLeft())
        {
            gameObject.SetActive(false);
        }

        transform.localPosition = Vector2.MoveTowards(transform.localPosition, new Vector2(0, transform.localPosition.y), speed * Time.deltaTime);
    }
}
