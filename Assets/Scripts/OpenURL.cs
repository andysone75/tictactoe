using UnityEngine;

public class OpenURL : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] string url;
#pragma warning restore 0649

    public void Go()
    {
        Application.OpenURL(url);
    }
}
