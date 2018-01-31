using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour , IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    Image backgroundImage;
    Image joystickImage;
    Vector3 inputVector;

    public enum MOVE_DIR
    {
        NONE,
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

    MOVE_DIR moveDir;

    public MOVE_DIR GetMoveDir()
    {
        if (inputVector.x > -0.5 && inputVector.x < 0.5f)
        {
            if (inputVector.z > 0f)
                return MOVE_DIR.UP;
            else if (inputVector.z < 0f)
                return MOVE_DIR.DOWN;
        }

        if (inputVector.z > -0.5 && inputVector.z < 0.5f)
        {
            if (inputVector.x > 0f)
                return MOVE_DIR.RIGHT;
            else if (inputVector.x < 0f)
                return MOVE_DIR.LEFT;
        }

        return MOVE_DIR.NONE;
    }

    void Start()
    {
        backgroundImage = GetComponent<Image>();
        joystickImage = transform.GetChild(0).GetComponent<Image>();
    }

    public virtual void OnPointerDown(PointerEventData ped)
    {
        OnDrag(ped);
    }

    public virtual void OnPointerUp(PointerEventData ped)
    {
        inputVector = Vector3.zero;
        joystickImage.rectTransform.anchoredPosition = Vector3.zero;
    }

    public virtual void OnDrag(PointerEventData ped)
    {
        Vector2 pos;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(backgroundImage.rectTransform, ped.position, ped.pressEventCamera, out pos))
        {
            pos.x = (pos.x / backgroundImage.rectTransform.sizeDelta.x);
            pos.y = (pos.y / backgroundImage.rectTransform.sizeDelta.y);

            inputVector = new Vector3(pos.x * 2, 0, pos.y * 2);
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

            joystickImage.rectTransform.anchoredPosition = new Vector3(inputVector.x * (backgroundImage.rectTransform.sizeDelta.x / 3), inputVector.z * (backgroundImage.rectTransform.sizeDelta.y / 3));
        }
    }
}
