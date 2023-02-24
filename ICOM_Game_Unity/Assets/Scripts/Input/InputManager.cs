using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

[DefaultExecutionOrder(-1)]
public class InputManager : Singleton<InputManager>
{
    public delegate void Swipe(Vector2 direction);
    public event Swipe swipePerformed;

    private PlayerInput playerInput;

    private InputAction touchPositionAction;
    private InputAction touchPressAction;


    [SerializeField] private float swipeResitance = 100;

    private Vector2 startPos;
    private Vector2 currentPos => touchPositionAction.ReadValue<Vector2>();

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        touchPressAction = playerInput.actions["TouchPress"];
        touchPositionAction = playerInput.actions["TouchPosition"];
    }

    private void OnEnable()
    {
        touchPressAction.performed += DetectSwipe;
        touchPressAction.performed += _ => { startPos = currentPos; };
        touchPressAction.canceled += _ => { startPos = currentPos; };
    }

    private void OnDisable()
    {
        touchPressAction.performed -= DetectSwipe;
    }

    private void DetectSwipe(InputAction.CallbackContext context)
    {
        Debug.Log("Press!");

        Vector2 delta = currentPos - startPos;
        Vector2 direction = Vector2.zero;

        if (Mathf.Abs(delta.x) > swipeResitance)
        {
            direction.x = Mathf.Clamp(delta.x, -1, 1);
        }
        if (Mathf.Abs(delta.y) > swipeResitance)
        {
            direction.y = Mathf.Clamp(delta.y, -1, 1);
        }

        if (direction != Vector2.zero & swipePerformed != null)
        {
            swipePerformed(direction);
            Debug.Log(direction);
        }
    }
}
