using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DobeilLocalizeText : MonoBehaviour
{
    public string _key;
    private Text _text;

    public void SetKey(string key)
    {
        _key = key;
        if (!_text)
            _text = GetComponent<Text>();
        DobeilLocalizeManager.RegisterLocalizeText(this);
        SetText();
    }

    void OnEnable()
    {
        if (!_text)
            _text = GetComponent<Text>();
        DobeilLocalizeManager.RegisterLocalizeText(this);
        SetText();
    }

    public void SetText()
    {
        string v = DobeilLocalizeManager.GetTranslatedText(_key);
        _text.text = string.IsNullOrEmpty(v) ? _text.text : v;
    }
}
