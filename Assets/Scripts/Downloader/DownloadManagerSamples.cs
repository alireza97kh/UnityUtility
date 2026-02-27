using Alireza97Khish.Downloader;
using UnityEngine;
using UnityEngine.UI;

public class DownloadManagerSamples : MonoBehaviour
{
	[SerializeField] private Image sampleImage;
	DownloadManager downloadManager;

	void Start()
	{
		downloadManager = new DownloadManager();

		DownloadRequest request = new DownloadRequest(
			"https://picsum.photos/512"
		)
		{
			Priority = DownloadPriority.High
		};

		downloadManager.Enqueue(
			request,
			OnDownloadComplete,
			OnProgress
		);
	}

	void OnProgress(float progress)
	{
		Debug.Log($"Download Progress: {progress}");
	}

	void OnDownloadComplete(DownloadResult result)
	{
		if (!result.Success)
		{
			Debug.LogError(result.Error);
			return;
		}

		Texture2D texture = new Texture2D(2, 2);

		bool loaded = texture.LoadImage(result.Data);

		if (!loaded)
		{
			Debug.LogError("Failed to load image data");
			return;
		}

		Sprite sprite = Sprite.Create(
			texture,
			new Rect(0, 0, texture.width, texture.height),
			new Vector2(0.5f, 0.5f)
		);
		sampleImage.sprite = sprite;
		sampleImage.SetNativeSize();
	}

}
