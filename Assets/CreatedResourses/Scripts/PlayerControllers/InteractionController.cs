using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private float _interactionReach = 3f;

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
                currentInteractableObject.Interact();
            }
        }
    }

    private void CheckForInteractableObject()
    {
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
