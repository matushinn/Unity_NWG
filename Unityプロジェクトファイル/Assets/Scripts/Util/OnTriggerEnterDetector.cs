using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class OnTriggerEnterDetector : MonoBehaviour
{
    private Collider _thisCollider;
    private Collider thisCollider
    {
        get
        {
            //AwakeがInstantiateで呼ばれないことがあるので初アクセス時にキャッシュする//
            if (_thisCollider == null)
            {
                _thisCollider = GetComponent<Collider>();
            }

            return _thisCollider;
        }
    }

    private void OnValidate()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    public Action<Collider> callBack;

    private void OnTriggerEnter(Collider col)
    {
        if (callBack != null) callBack(col);
    }

    public void OnEnable()
    {
        thisCollider.enabled = true;
    }

    public void OnDisable()
    {
        thisCollider.enabled = false;
    }
}