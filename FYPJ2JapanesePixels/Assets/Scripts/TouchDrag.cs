using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchDrag : MonoBehaviour
{
    Collider2D collider;
    Vector3 originalPos;

    bool b_isDragged;
    public bool canDrag { get; set; }
    public bool b_ReturnToOriginalPos { get; set; }

    void Start()
    {
        canDrag = true;
        originalPos = transform.localPosition;
        collider = GetComponent<BoxCollider2D>();
    }

	void Update() 
    {
        if (b_ReturnToOriginalPos)
        {
            ReturnToOriginalPosition();
        }

        if (!canDrag)
            return;

        // if sprite is used, use Camera.main.ScreenToWorldPoint(position)
        // if image is used, use position directly

#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            if (collider == Physics2D.OverlapPoint(Input.mousePosition))
                b_isDragged = true;
        }
        else
            Release();

#elif UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            if (collider == Physics2D.OverlapPoint(Input.GetTouch(0).position))
                b_isDragged = true;
        }
        else
            Release();
#endif

        if (b_isDragged)
            Drag();
    }

    // Make buttons move to cursor/finger position
    void Drag()
    {
        Vector3 pos;
        Vector3 touchPos;

#if UNITY_EDITOR
        touchPos = Input.mousePosition;
#elif UNITY_ANDROID
        touchPos = Input.GetTouch(0).position
#endif

        pos = touchPos;
        pos.z = transform.position.z;
        transform.position = pos;

        // Limit dragging to only buttons area
        // temp value until UI is settled
        if (transform.localPosition.y > 39f)
        {
            transform.localPosition = new Vector2(transform.localPosition.x, 39f);
        }

        OnDrag();
    }

    // Prevents other buttons from being dragged
    void OnDrag()
    {
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            if (transform.parent.GetChild(i) == this.transform)
                continue;

            transform.parent.GetChild(i).GetComponent<TouchDrag>().canDrag = false;
        }
    }

    // Allows other buttons to be dragged
    public void Release()
    {
        ReturnToOriginalPosition();

        for (int i = 0; i < transform.parent.childCount; i++)
        {
            if (transform.parent.GetChild(i) == this.transform)
                continue;

            transform.parent.GetChild(i).GetComponent<TouchDrag>().canDrag = true;
        }
    }

    void ReturnToOriginalPosition()
    {
        Vector3 dir = originalPos - transform.localPosition;
        float speed = 5f;

        if (dir.sqrMagnitude > 1)
        {
            b_isDragged = false;
            canDrag = false;
            transform.position += dir * Time.deltaTime * speed;
        }
        else
        {
            canDrag = true;
            b_ReturnToOriginalPos = false;
        }
    }
}
