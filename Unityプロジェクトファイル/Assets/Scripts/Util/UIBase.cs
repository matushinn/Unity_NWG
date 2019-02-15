using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasGroup))]
public abstract class UIBase : MonoBehaviour
{
    protected CanvasGroup thisCanvasGroup;

    public virtual void Awake()
    {
        thisCanvasGroup = GetComponent<CanvasGroup>();
        Hide();
    }

    public virtual void Show()
    {
        thisCanvasGroup.Show();
    }

    public virtual void Hide()
    {
        thisCanvasGroup.Hide();
    }
}