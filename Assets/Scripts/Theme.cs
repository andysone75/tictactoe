using UnityEngine;
using UnityEngine.UI;

public class Theme : MonoBehaviour {
    [SerializeField] Image border;
    [SerializeField] Text goalTxt;
    public bool open = false;

    private void Start()
    {
        if (GetGoal() <= PlayerPrefs.GetInt("HighScore")) open = true;
    }

    public int GetGoal() { return int.Parse(goalTxt.text); }

    public void UnBlock()
    {
        open = true;

        var borderColor = border.color;
        borderColor.a = 0;
        border.color = borderColor;

        goalTxt.color = borderColor;
    }

    public void Pick ()
    {
        var borderColor = new Color(0.1764706f, 0.2039216f, 0.2117647f);
        border.color = borderColor;
    }

    public void UnPick ()
    {
        var borderColor = border.color;
        borderColor.a = 0;
        border.color = borderColor;
    }
}