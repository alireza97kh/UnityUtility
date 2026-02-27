namespace Alireza97Khish.CacheSystem
{
	public class CacheSystem
	{
		ICacheSystem memory = new MemoryCache();

		ICacheSystem disk = new DiskCache();

		public bool TryGet(string key, out byte[] data, out string etag)
		{
			if (memory.TryGet(key, out data, out etag))
				return true;

			if (disk.TryGet(key, out data, out etag))
			{
				memory.Save(key, data, etag);
				return true;
			}

			return false;
		}

		public void Save(string key, byte[] data, string etag)
		{
			memory.Save(key, data, etag);
			disk.Save(key, data, etag);
		}
	}
}
