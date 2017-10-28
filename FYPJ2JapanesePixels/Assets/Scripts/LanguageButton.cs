using UnityEngine;
using UnityEngine.UI;

public class LanguageButton : MonoBehaviour 
{
    public int buttonIndex { get; set; }
    public bool b_ReturnToOriginalPos { get; set; }

    TouchDrag touchDragComponent;
    Vector3 originalPos;

    void Start()
    {
        touchDragComponent = GetComponent<TouchDrag>();
        originalPos = transform.localPosition;
    }

    void Update()
    {
        if (b_ReturnToOriginalPos)
        {
            ReturnToOriginalPosition();
        }
    }

    void ReturnToOriginalPosition()
    {
        touchDragComponent.Release();
        
        Vector3 dir = originalPos - transform.localPosition;
        float speed = 5f;

        if (dir.sqrMagnitude > 1)
        {
            transform.position += dir * Time.deltaTime * speed;
        }
        else
        {
            b_ReturnToOriginalPos = false;
        }
    }
}
