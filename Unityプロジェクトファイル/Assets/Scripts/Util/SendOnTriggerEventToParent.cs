using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof (Collider))]
public class SendOnTriggerEventToParent : MonoBehaviour
{
    public UnityAction<Collider> OnTriggerEnterEvent;

    private Collider thisCollider;

    private void Awake()
    {
        thisCollider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        thisCollider.enabled = true;
    }

    private void OnDisable()
    {
        thisCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterEvent(other);
    }

}
