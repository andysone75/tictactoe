using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool SoundState = true;
    public static bool isGameStarted = false;
    public static bool fromMainPage = true;
    public static Color theme;

    public static void StartPVP () { SceneManager.LoadScene("PVP"); }
    public static void StartPVE()  { SceneManager.LoadScene("PVE"); }
    public static void GoToMainPage ()
    {
        SceneManager.LoadScene("MainPage");
        fromMainPage = true;
    }

    public static void SetTheme ()
    {
        var camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        camera.backgroundColor = theme;
    }

    public IEnumerator SetThemeIE()
    {
        var mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        for (var i = 0f; i < 1; i += 0.05f)
        {
            mainCamera.backgroundColor = Color.Lerp(mainCamera.backgroundColor, theme, i);
            yield return new WaitForSeconds(0.015f);
        }
    }
}
