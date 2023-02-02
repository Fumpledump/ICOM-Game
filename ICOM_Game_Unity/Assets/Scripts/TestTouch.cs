using UnityEngine;

public class TestTouch : MonoBehaviour
{
    private InputManager inputManager;
    private Camera camerMain;

    private void Awake()
    {
        inputManager = InputManager.Instance;
        camerMain = Camera.main;
    }

    private void OnEnable()
    {
        inputManager.OnStartTouch += Move;
    }

    private void OnDisable()
    {
        inputManager.OnEndTouch -= Move;
    }

    public void Move(Vector2 screenPosition, float time)
    {
        Vector3 screenCoordinates = new Vector3(screenPosition.x, screenPosition.y, camerMain.nearClipPlane);
        Vector3 worldCoordinates = camerMain.ScreenToWorldPoint(screenCoordinates);
        worldCoordinates.z = 0;
        transform.position = worldCoordinates;
    }
}
