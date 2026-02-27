using System;
using System.Collections.Generic;
using System.Threading;

namespace Alireza97Khish.Downloader
{
	public class DownloadResult
	{
		public bool Success;

		public byte[] Data;

		public string Error;

		public long ResponseCode;

		public string ETag;
	}

	public class DownloadRequest
	{
		public string Url;

		public bool UseCache = true;

		public int RetryCount = 2;

		public int Timeout = 30;

		public DownloadPriority Priority = DownloadPriority.Normal;

		public Dictionary<string, string> Headers = new();

		public CancellationToken CancellationToken;

		public DownloadRequest(string url)
		{
			Url = url;
		}
	}

	public class DownloadTask
	{
		public DownloadRequest Request;

		public Action<DownloadResult> Callback;

		public Action<float> Progress;

		public int Priority;

		public DownloadTask(DownloadRequest request,
			Action<DownloadResult> callback,
			Action<float> progress)
		{
			Request = request;
			Callback = callback;
			Progress = progress;
			Priority = (int)request.Priority;
		}
	}

	public enum DownloadPriority
	{
		Low = 0,
		Normal = 1,
		High = 2,
		Critical = 3
	}
}