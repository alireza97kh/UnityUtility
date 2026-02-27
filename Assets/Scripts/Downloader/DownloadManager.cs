using Alireza97Khish.CacheSystem;
using Alireza97Khish.Downloader;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
public class DownloadManager
{
	AsyncDownloader downloader = new();

	CacheSystem cache = new();

	List<DownloadTask> queue = new();

	int activeDownloads;

	int maxParallel = 4;

	public void Enqueue(
		DownloadRequest request,
		System.Action<DownloadResult> callback,
		System.Action<float> progress = null)
	{
		queue.Add(new DownloadTask(request, callback, progress));

		queue = queue.OrderByDescending(x => x.Priority).ToList();

		ProcessQueue();
	}

	async void ProcessQueue()
	{
		if (activeDownloads >= maxParallel)
			return;

		if (queue.Count == 0)
			return;

		var task = queue[0];

		queue.RemoveAt(0);

		activeDownloads++;

		await ExecuteTask(task);

		activeDownloads--;

		ProcessQueue();
	}

	async Task ExecuteTask(DownloadTask task)
	{
		var req = task.Request;

		if (req.UseCache)
		{
			if (cache.TryGet(req.Url, out var data, out var etag))
			{
				var result = await downloader.Download(req, task.Progress, etag);

				if (result.ResponseCode == 304)
				{
					task.Callback(new DownloadResult
					{
						Success = true,
						Data = data
					});

					return;
				}

				if (result.Success)
				{
					cache.Save(req.Url, result.Data, result.ETag);

					task.Callback(result);

					return;
				}
			}
		}

		for (int i = 0; i <= req.RetryCount; i++)
		{
			var result = await downloader.Download(req, task.Progress, null);

			if (result.Success)
			{
				if (req.UseCache)
					cache.Save(req.Url, result.Data, result.ETag);

				task.Callback(result);

				return;
			}
		}

		task.Callback(new DownloadResult
		{
			Success = false,
			Error = "Failed after retries"
		});
	}
}
