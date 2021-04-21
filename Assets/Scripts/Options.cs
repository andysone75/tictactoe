using UnityEngine;

[RequireComponent(typeof(Animation))]
public class Options : MonoBehaviour
{
    public bool IsActive { get; private set; } = false;
    public static bool Block = true;

    public void Show()
    {
        if (IsActive || Block)
            return;

        var animation = GetComponent<Animation>();
        animation.Play("Show");

        IsActive = true;
    }

    public void Hide()
    {
        if (!IsActive || Block)
            return;

        var animation = GetComponent<Animation>();
        animation.Play("Hide");

        IsActive = false;
    }

    public void DocumentsShow()
    {
        if (!IsActive)
            return;

        var animation = GetComponent<Animation>();
        animation.PlayQueued("Documents_Show");
    }

    public void DocumentsHide()
    {
        var animation = GetComponent<Animation>();
        animation.Play("Documents_Hide");
    }
}
