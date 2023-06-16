using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MySwipe : MonoBehaviour
{


    private bool tap, swipeLeft, swipeRight, swipeUp, swipeDown;
    private bool isDraging = false;
    private Vector2 startTouch, swipeDelta;

    private void Update()
    {
        tap = swipeLeft = swipeRight = swipeUp = swipeDown = false;

        #region Standalone Inputs

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject() && EventSystem.current.currentSelectedGameObject != null &&
     EventSystem.current.currentSelectedGameObject.GetComponent<Button>() != null || EventSystem.current.IsPointerOverGameObject() && EventSystem.current.currentSelectedGameObject != null &&
     EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>() != null)
            {
                tap = false;
            }
            else
            {
                tap = true;
                isDraging = true;
            }
            
            
            startTouch = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDraging = false;
            Reset();
        }

        #endregion

        #region Mobile Inputs
        if (Input.touches.Length > 0)
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                
                
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) && EventSystem.current.currentSelectedGameObject != null &&
     EventSystem.current.currentSelectedGameObject.GetComponent<Button>() != null || EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) && EventSystem.current.currentSelectedGameObject != null &&
     EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>() != null)
                {
                    tap = false;
                }
                else
                {
                    tap = true;
                    isDraging = true;
                }
                startTouch = Input.touches[0].position;
            }
            else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                isDraging = false;
                Reset();
            }
        }
        #endregion

        swipeDelta = Vector2.zero;
        if (isDraging)
        {
            if (Input.touches.Length > 0)
            {
                swipeDelta = Input.touches[0].position - startTouch;
            }
            else if (Input.GetMouseButton(0))
            {
                swipeDelta = (Vector2)Input.mousePosition - startTouch;
            }
        }


        if (swipeDelta.magnitude > 25)
        {
            float x = swipeDelta.x;
            float y = swipeDelta.y;

            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                if (x < 0)
                {
                    swipeLeft = true;
                }
                else
                {
                    swipeRight = true;
                }
            }
            else
            {
                if (y < 0)
                {
                    swipeDown = true;
                }
                else
                {
                    swipeUp = true;
                }
            }
            Reset();
        }
    }

    private void Reset()
    {
        startTouch = swipeDelta = Vector2.zero;
        isDraging = false;
    }

    public Vector2 SwipeDelta { get { return swipeDelta; } }
    public bool SwipeLeft { get { return swipeLeft; } }
    public bool SwipeRight { get { return swipeRight; } }
    public bool SwipeUp { get { return swipeUp; } }
    public bool SwipeDown { get { return swipeDown; } }
    public bool Tap { get { return tap; } }
}

