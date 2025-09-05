using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Tap.started += OnTapStarted;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
        inputActions.Player.Tap.started -= OnTapStarted;
    }

    private void OnTapStarted(InputAction.CallbackContext ctx)
    {
        Vector2 tapPosition = ctx.ReadValue<Vector2>();
        Debug.Log($"Tap detected at position: {tapPosition}");
    }
}
