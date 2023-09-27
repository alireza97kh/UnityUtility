using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DobeilHelper : Singleton<DobeilHelper>
{
    public Sprite SpriteFromTexture2D(Texture2D texture, bool sqr)
    {
        try
        {
            if (sqr)
            {
                float _width = texture.width;
                float _height = texture.height;
                float ratio = _width - _height > 0 ? _width / _height : _height / _width;
                if (ratio > 1.05f)
                {
                    var result = SQR(texture);
                    return Sprite.Create(result, new Rect(0.0f, 0.0f, result.width, result.height), new Vector2(0.5f, 0.5f), 100.0f);
                }
                else
                {
                    return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                }
            }
            else
                return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        }
        catch
        {
            return null;
        }
    }

    public Texture2D SQR(Texture2D texture)
    {
        var width = texture.width;
        var height = texture.height;
        int min = Mathf.Min(width, height);
        Rect texR = new Rect(0, 0, min, min);
        _gpu_scale(texture, width, height, texture.filterMode);

        //Get rendered data back to a new texture
        Texture2D result = new Texture2D(min, min, TextureFormat.ARGB32, true);
        result.Resize(min, min);
        result.ReadPixels(texR, 0, 0, true);
        result.Apply();
        return result;
    }
    private void _gpu_scale(Texture2D texture, int width, int height, FilterMode fmode)
    {

        int min = Mathf.Min(width, height);
        bool widthIsBiger = width > height;

        float left = 0;
        float right = 1;
        float top = 0;
        float bottom = 1;

        if (widthIsBiger)
        {

            left = ((float)(width - min) / width) / 2;
            right = 1 - left;
        }
        else
        {
            top = ((float)(height - min) / height) / 2;
            bottom = 1 - top;
        }


        //We need the source texture in VRAM because we render with it
        texture.filterMode = fmode;
        texture.Apply(true);

        //Using RTT for best quality and performance. Thanks, Unity 5
        RenderTexture rtt = new RenderTexture(min, min, 32);

        //Set the RTT in order to render to it
        Graphics.SetRenderTarget(rtt);

        //Setup 2D matrix in range 0..1, so nobody needs to care about sized
        GL.LoadPixelMatrix(left, right, bottom, top);
        //     CustomLog.LogError($"left : {left}  right : {right}  bottom : {bottom } top : {top}");

        //Then clear & draw the texture to fill the entire RTT.
        GL.Clear(true, true, new Color(0, 0, 0, 0));
        Graphics.DrawTexture(new Rect(0, 0, 1, 1), texture);
    }
}
