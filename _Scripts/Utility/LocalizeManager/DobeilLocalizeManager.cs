using Dobeil;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DobeilLocalizeManager : MonoSingleton<DobeilLocalizeManager>
{
    private Dictionary<string, string> localizeDictionary = new Dictionary<string, string>();
    private Dictionary<string, Sprite> imageLocalizeDictionary = new Dictionary<string, Sprite>();
    private List<DobeilLocalizeText> localizeTexts = new List<DobeilLocalizeText>();
    private List<DobeilLocalizeImage> localizeImages = new List<DobeilLocalizeImage>();
    private List<DobeilDirection> directions = new List<DobeilDirection>();
    private Action onLanguageChange;
    public TextAsset defultLanguage;
    public bool downloadTextAsset = false;
    public string textAssetDownloadUrl = "";
    public bool isRtl;
	protected override void Awake()
    {
        base.Awake();
		if (downloadTextAsset)
		{
            DobeilRemoteAsset.Instance.DownloadText(
                textAssetDownloadUrl,
                (txt, errorMessage, fromCash) => 
                {
					if (errorMessage != "")
					{
                        Debug.LogError(errorMessage);
                        return;
					}
                    FillLocalizeDictionary(txt);
                });
		}
        else
		{
            FillLocalizeDictionary(defultLanguage);
		}
    }
    void FillLocalizeDictionary(TextAsset _text) 
	{
        Instance.localizeDictionary.Clear();
        RootObject[] deserializeObject = JsonUtility.FromJson<RootObject[]>(_text.text);
        foreach (RootObject obj in deserializeObject)
        {
            Instance.localizeDictionary.Add(obj.key, obj.value);
        }
    }

    public static string GetTranslatedText(string key, bool isRtlFixed = false, params string[] parameters)
    {
        string value = key;
        if (!string.IsNullOrEmpty(key))
            if (Instance.localizeDictionary.TryGetValue(key, out value))
            {
                value = isRtlFixed || !Instance.isRtl ? value : value.RtlFix();
                if (parameters.Length > 0)
                {
                    value = value.Replace("$", " ");
                    value = string.Format(value, parameters);
                }
            }
        return value;
    }
    public static string GetTranslatedText(string key, bool isFixed = false)
    {
        string value = key;
        if (!string.IsNullOrEmpty(key))
            if (Instance.localizeDictionary.TryGetValue(key, out value))
                value = isFixed || !Instance.isRtl ? value : value.RtlFix();

        value = string.IsNullOrEmpty(value) ? key : value;
        return value;
    }
    public static Sprite GetTranslatedImage(string key)
    {
        if (Instance.imageLocalizeDictionary.TryGetValue(key, out Sprite value))
        {
            return value;
        }
        else
        {
            DobeilLogger.Log($" --  {key}  --  NotFounded");
            return null;
        }
    }

    public static void RegisterLocalizeText(DobeilLocalizeText localizeText)
    {
        if (!Instance.localizeTexts.Contains(localizeText))
            Instance.localizeTexts.Add(localizeText);
    }
    public static void RegisterLocalizeImage(DobeilLocalizeImage localizeImage)
    {
        if (!Instance.localizeImages.Contains(localizeImage))
            Instance.localizeImages.Add(localizeImage);
    }
    public static void RegisterDirection(DobeilDirection direction)
    {
        if (!Instance.directions.Contains(direction))
            Instance.directions.Add(direction);
    }
    public static void ChangeLanguage(string[] language, Sprite[] sprites)
    {
        FillDictionary(language, sprites);
        foreach (DobeilLocalizeText localizeText in Instance.localizeTexts)
        {
            if (localizeText)
                localizeText.SetText();
        }
        foreach (DobeilLocalizeImage localizeImage in Instance.localizeImages)
        {
            if (localizeImage)
                localizeImage.SetImage();
        }
        foreach (DobeilDirection direction in Instance.directions)
        {
            if (direction)
                direction.ChangeDirection();
        }
        Instance.onLanguageChange?.Invoke();
    }

    private static void FillDictionary(string[] languageTexts, Sprite[] sprites)
    {

        Instance.localizeDictionary.Clear();

        foreach (string languageText in languageTexts)
        {
            RootObject[] deserializeObject = JsonUtility.FromJson<RootObject[]>(languageText);
            foreach (RootObject obj in deserializeObject)
            {
                if (Instance.localizeDictionary.ContainsKey(obj.key))
                {
                    DobeilLogger.LogWarning(obj.value);
                }
                else
                    Instance.localizeDictionary.Add(obj.key, obj.value);
            }
        }

        Instance.imageLocalizeDictionary.Clear();
        foreach (Sprite spr in sprites)
        {
            Instance.imageLocalizeDictionary.Add(spr.name, spr);
        }

    }

}