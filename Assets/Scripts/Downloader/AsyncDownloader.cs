using System.Threading.Tasks;
using UnityEngine.Networking;
namespace Alireza97Khish.Downloader
{
	public class AsyncDownloader
	{
		public async Task<DownloadResult> Download(
			DownloadRequest request,
			System.Action<float> progress,
			string etag)
		{
			using UnityWebRequest web = UnityWebRequest.Get(request.Url);

			web.timeout = request.Timeout;

			if (!string.IsNullOrEmpty(etag))
				web.SetRequestHeader("If-None-Match", etag);

			foreach (var header in request.Headers)
				web.SetRequestHeader(header.Key, header.Value);

			var operation = web.SendWebRequest();

			while (!operation.isDone)
			{
				progress?.Invoke(web.downloadProgress);

				if (request.CancellationToken.IsCancellationRequested)
				{
					web.Abort();

					return new DownloadResult
					{
						Success = false,
						Error = "Cancelled"
					};
				}

				await Task.Yield();
			}

			if (web.responseCode == 304)
			{
				return new DownloadResult
				{
					Success = true,
					Data = null,
					ResponseCode = 304
				};
			}

			if (web.result != UnityWebRequest.Result.Success)
			{
				return new DownloadResult
				{
					Success = false,
					Error = web.error
				};
			}

			return new DownloadResult
			{
				Success = true,
				Data = web.downloadHandler.data,
				ResponseCode = web.responseCode,
				ETag = web.GetResponseHeader("ETag")
			};
		}
	}
}