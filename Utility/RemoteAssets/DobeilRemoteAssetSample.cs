using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DobeilRemoteAssetSample : MonoBehaviour
{
    [SerializeField] Image myImage;
    // Start is called before the first frame update
    void Start()
    {
        DobeilRemoteAsset.Instance.DownloadTexture(
            "https://www.oliveboard.in/blog/wp-content/uploads/2023/09/Google.png",
            (texure, errMessage, cashed) => 
            {
				if (errMessage == "")
                    myImage.sprite = DobeilHelper.Instance.SpriteFromTexture2D(texure, false);
            });
    }
}
