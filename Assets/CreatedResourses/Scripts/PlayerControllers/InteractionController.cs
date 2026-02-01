using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private float _interactionReach = 3f;
    [SerializeField][ReadOnlyInspector] private bool _busyInteracting = false;

    InteractableObject currentInteractableObject;

    private Camera _camera;

    private void Start()
    {
        _camera = FindFirstObjectByType<Camera>().GetComponent<Camera>();
    }

    private void Update()
    {
        CheckForInteractableObject();
    }

    public void OnInteract(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            if (currentInteractableObject != null)
            {
                if (currentInteractableObject.isContinousInteraction)
                {
                    _busyInteracting = true;
                    DisableHUDWhenBusy();
                }
                currentInteractableObject.Interact();
            }
        }
    }

    public void OnDiscard(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            if (currentInteractableObject != null)
            {
                if (_busyInteracting)
                {
                    currentInteractableObject.Discard();
                    _busyInteracting = false;
                }
            }
        }
    }

    private void CheckForInteractableObject()
    {
        if (_busyInteracting) return;

        Ray interactionRay = new Ray(_camera.transform.position, _camera.transform.forward);
        if (Physics.Raycast(interactionRay, out RaycastHit hitInfo, _interactionReach))
        {
            if (hitInfo.collider.tag == "Interactable")
            {
                InteractableObject newInteractableObject = hitInfo.collider.GetComponent<InteractableObject>();

                if (currentInteractableObject != null && currentInteractableObject != newInteractableObject)
                {
                    DisableCurrentInteractableObject();
                }

                if (newInteractableObject != null && newInteractableObject.enabled)
                {
                    SetNewInteractableObject(newInteractableObject);
                }
                else
                {
                    DisableCurrentInteractableObject();
                }
            }
            else
            {
                DisableCurrentInteractableObject();
            }
        }
        else
        {
            DisableCurrentInteractableObject();
        }
    }

    private void DisableHUDWhenBusy()
    {
        HUDcontroller.instance.DisableInteractionText();
        if (currentInteractableObject)
        {
            currentInteractableObject.DisableOutline();
        }
    }

    private void SetNewInteractableObject(InteractableObject newInteractableObject)
    {
        currentInteractableObject = newInteractableObject;
        currentInteractableObject.EnableOutline();
        HUDcontroller.instance.EnableInteractionText(currentInteractableObject._message);
    }

    private void DisableCurrentInteractableObject()
    {
        HUDcontroller.instance.DisableInteractionText();
        if (currentInteractableObject)
        {
            currentInteractableObject.DisableOutline();
            currentInteractableObject = null;
        }
    }
}
