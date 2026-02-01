using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InteractableObject : MonoBehaviour
{
    public bool isContinousInteraction;
    Outline outline;
    public string _message;

    [SerializeField] private UnityEvent _onInteraction;
    [SerializeField] private UnityEvent _onDiscard;

    private void Start()
    {
        outline = GetComponent<Outline>();
        DisableOutline(); 
    }

    public void Interact()
    {
        _onInteraction.Invoke();
    }

    public void Discard()
    {
        _onDiscard.Invoke();
    }

    public void DisableOutline()
    {
        outline.enabled = false;
    }

    public void EnableOutline()
    {
        outline.enabled = true;
    }
}
