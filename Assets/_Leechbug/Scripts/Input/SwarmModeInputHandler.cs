/*using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwarmModeInputHandler : MonoBehaviour, PlayerInputActions.ISwarmModeActions
{
    public static Vector2InputValue MousePosition = new Vector2InputValue();
    public static Vector2InputValue Look = new Vector2InputValue();
    public static BoolInputValue Inspect = new BoolInputValue();
    public static BoolInputValue EnableFreeLook = new BoolInputValue();

    private PlayerInputActions _actions;

    public static SwarmModeInputHandler INSTANCE;

    private bool isSwarmModeEnabled;
    public static bool allowSwarmModeToggling;
    public static Action OnEnableSwarmMode, OnDisableSwarmMode;
    private void Awake()
    {
        INSTANCE = this;
        GlobalInputHandler.ToggleSwarmMode.OnTrigger += SetSwarmModeEnabled;
        allowSwarmModeToggling = true;
    }

    private void OnDestroy()
    {
        GlobalInputHandler.ToggleSwarmMode.OnTrigger -= SetSwarmModeEnabled;
    }

    public void OnEnable()
    {
        if (_actions == null)
        {
            _actions = new PlayerInputActions();
            _actions.SwarmMode.SetCallbacks(this);
        }
    }

    private void OnDisable()
    {
        _actions.SwarmMode.Disable();
    }
    private void SetSwarmModeEnabled()
    {
        if (!allowSwarmModeToggling)
            return;
        if (TutorialGameManager.instance && TutorialGameManager.instance.pauseTutorialActive)
        {
            return;
        }
        isSwarmModeEnabled = !isSwarmModeEnabled;
        if (isSwarmModeEnabled)
        {
            OnEnableSwarmMode?.Invoke();
            _actions.SwarmMode.Enable();
        }
        else
        {
            // Tutorial stuff
            if (TutorialGameManager.instance)
            {
                if (TutorialGameManager.instance.possessFishTutorialActive)
                {
                    if (!TutorialGameManager.instance.possessFishTutorialSatisfied)
                    {
                        isSwarmModeEnabled = !isSwarmModeEnabled;
                        return;
                    }
                    else
                    {
                        TutorialGameManager.instance.possessFishTutorialActive = false;
                    }
                }
                if (TutorialGameManager.instance.assignTargetTutorialActive)
                {
                    if (!TutorialGameManager.instance.assignTargetTutorialSatisfied)
                    {
                        isSwarmModeEnabled = !isSwarmModeEnabled;
                        return;
                    }
                    else
                    {
                        TutorialGameManager.instance.assignTargetTutorialActive = false;
                    }
                }
            }
            ResetVariables();
            OnDisableSwarmMode?.Invoke();
            _actions.SwarmMode.Disable();
        }
    }

    public void OnMousePosition(InputAction.CallbackContext context) { MousePosition.TrySetValue(context.ReadValue<Vector2>()); }
    public void OnLook(InputAction.CallbackContext context) { Look.TrySetValue(context.ReadValue<Vector2>()); }
    public void OnEnableFreeLook(InputAction.CallbackContext context)
    {
        if (context.performed)
            EnableFreeLook.TrySetValue(true); 
        else if (context.canceled)
            EnableFreeLook.TrySetValue(false); 
    }

    public void OnInspect(InputAction.CallbackContext context)
    {
        if (context.performed)
            Inspect.TrySetValue(true);
        else if (context.canceled)
            Inspect.TrySetValue(false);
    }

    void ResetVariables()
    {
        Inspect.TrySetValue(false);
        EnableFreeLook.TrySetValue(false);
    }

    // For Tutorial
    public void ExitSwarmMode()
    {
        if (isSwarmModeEnabled)
        {
            isSwarmModeEnabled = !isSwarmModeEnabled;
            ResetVariables();
            OnDisableSwarmMode?.Invoke();
            _actions.SwarmMode.Disable();
        }
    }
}
*/