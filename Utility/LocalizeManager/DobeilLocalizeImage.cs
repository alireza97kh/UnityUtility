using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DobeilLocalizeImage : MonoBehaviour
{
    [SerializeField] private string _key;
    [SerializeField] private Image _image;

    void OnEnable()
    {
		if (_image == null)
            _image = GetComponent<Image>();

        DobeilLocalizeManager.RegisterLocalizeImage(this);
        SetImage();
    }

    public void SetImage()
    {
        if (!string.IsNullOrEmpty(_key))
            _image.sprite = DobeilLocalizeManager.GetTranslatedImage(_key);
    }

}
