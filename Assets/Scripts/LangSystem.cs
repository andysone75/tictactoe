using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LangSystem : MonoBehaviour
{
    [SerializeField] private Image button;
    [SerializeField] private Sprite[] flags;
    public static Lang lng = new Lang();
    private int index = 0;
    private string[] langsArray = { "ru_RU", "en_US" };

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("Language"))
        {
            if (Application.systemLanguage == SystemLanguage.Russian || Application.systemLanguage == SystemLanguage.Ukrainian || Application.systemLanguage == SystemLanguage.Belarusian)
                PlayerPrefs.SetString("Language", "ru_RU");
            else
                PlayerPrefs.SetString("Language", "en_US");
        }
        index = Array.IndexOf(langsArray, PlayerPrefs.GetString("Language"));
        LangLoad();
    }

    private void Start()
    {
        button.sprite = flags[index];
    }

    private void LangLoad()
    {
        string json;
#if UNITY_ANDROID && !UNITY_EDITOR
        string path = Path.Combine(Application.streamingAssetsPath, string.Format("Languages/{0}.json", PlayerPrefs.GetString("Language")));
        WWW reader = new WWW(path);
        while (!reader.isDone) { }
        json = reader.text;
#endif
#if UNITY_EDITOR
        json = File.ReadAllText(Application.streamingAssetsPath + "/Languages/" + PlayerPrefs.GetString("Language") + ".json");
#endif
        lng = JsonUtility.FromJson<Lang>(json);
    }

    public void SwitchLang()
    {
        if (++index == langsArray.Length) index = 0;
        PlayerPrefs.SetString("Language", langsArray[index]);
        button.sprite = flags[index];
        LangLoad();

        var mainPage = GetComponent<MainPage>();
        if (mainPage != null) mainPage.SendMessage("LangSwitch");
    }
}

public class Lang
{
    public string[] mainPage = new string[2];
    public string[] pve = new string[12];
    public string[] pvp = new string[7];
}
