using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

[DefaultExecutionOrder(-1)]
public class InputManager : Singleton<InputManager>
{
    [SerializeField] private float minimumDistance = .2f;
    [SerializeField] private float maximumTime = 1f;
    [SerializeField] [Range(0,1)] private float directionThreshold = .9f;
    [SerializeField] private GameObject trail;

    public Vector2 startTouchPosition;
    public float startTouchTime;
    public Vector2 endTouchPosition;
    public float endTouchTime;

    private Camera mainCamera;
    private PlayerInput playerInput;
    private PlayerControls playerControls;

    private Coroutine trailRoutine;

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerInput = GetComponent<PlayerInput>();
        mainCamera = Camera.main;
    }


    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void TouchPress(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            startTouchPosition = GetTouchPosition();
            startTouchTime = (float)context.startTime;
            trail.SetActive(true);
            trail.transform.position = startTouchPosition;
            trailRoutine = StartCoroutine(Trail());
        }

        if (context.canceled)
        {
            trail.SetActive(false);
            StopCoroutine(trailRoutine);
            endTouchPosition = GetTouchPosition();
            endTouchTime = (float)context.time;

            SwipeDetection();
        }
    }
    public Vector2 GetTouchPosition()
    {
        return Utils.ScreenToWorld(mainCamera, playerControls.Touch.TouchPosition.ReadValue<Vector2>());
    }

    private void SwipeDetection()
    {
        if (Vector3.Distance(startTouchPosition, endTouchPosition) >= minimumDistance && 
            (endTouchTime - startTouchTime) <= maximumTime)
        {
            Vector3 direction = endTouchPosition - startTouchPosition;
            Vector2 direction2D = new Vector2(direction.x, direction.y).normalized;
            SwipeDirection(direction2D);
        }
    }

    private void SwipeDirection(Vector2 direction)
    {
        if (Vector2.Dot(Vector2.up, direction) > directionThreshold)
        {
            Debug.Log("Swipe Up");
        }
        else if (Vector2.Dot(Vector2.down, direction) > directionThreshold)
        {
            Debug.Log("Swipe Down");
        }
        else if (Vector2.Dot(Vector2.left, direction) > directionThreshold)
        {
            Debug.Log("Swipe Left");
        }
        else if (Vector2.Dot(Vector2.right, direction) > directionThreshold)
        {
            Debug.Log("Swipe Right");
        }
    }

    private IEnumerator Trail()
    {
        while (true)
        {
            trail.transform.position = GetTouchPosition();
            yield return null;
        }
    }
}
