using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public void ResetSprite() { GetComponent<Image>().sprite = null; }
}
