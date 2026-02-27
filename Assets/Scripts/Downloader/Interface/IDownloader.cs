using System.Threading.Tasks;

namespace Alireza97Khish.Downloader
{
	public interface IDownloader
	{
		Task<DownloadResult> DownloadAsync(DownloadRequest request);
	}
}