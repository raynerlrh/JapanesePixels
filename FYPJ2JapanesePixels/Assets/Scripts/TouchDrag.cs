using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchDrag : MonoBehaviour
{
    Collider2D collider;

    public bool canDrag { get; set; }

    void Start()
    {
        canDrag = true;
        collider = GetComponent<BoxCollider2D>();
    }

	void Update() 
    {
        if (!canDrag)
            return;

        // if sprite is used, use Camera.main.ScreenToWorldPoint(position)
        // if image is used, use position directly

#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            if (collider == Physics2D.OverlapPoint(Input.mousePosition))
            {
                Drag();
            }
        }
        else
        {
            Release();
        }

#elif UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            if (collider == Physics2D.OverlapPoint(Input.GetTouch(0).position))
            {
                Drag();
            }
        }
        else
        {
            Release();
        }
#endif
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
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            if (transform.parent.GetChild(i) == this.transform)
                continue;

            transform.parent.GetChild(i).GetComponent<TouchDrag>().canDrag = true;
        }
    }
}
