namespace Alireza97Khish.CacheSystem
{
	public interface ICacheSystem
	{
		bool TryGet(string key, out byte[] data, out string etag);

		void Save(string key, byte[] data, string etag);

		void Remove(string key);
	}
}
