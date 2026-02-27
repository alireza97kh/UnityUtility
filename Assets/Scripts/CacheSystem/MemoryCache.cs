using System.Collections.Generic;

namespace Alireza97Khish.CacheSystem
{
	public class MemoryCache : ICacheSystem
	{
		class CacheEntry
		{
			public byte[] Data;
			public string ETag;
		}

		Dictionary<string, CacheEntry> cache = new();

		public bool TryGet(string key, out byte[] data, out string etag)
		{
			if (cache.TryGetValue(key, out var entry))
			{
				data = entry.Data;
				etag = entry.ETag;
				return true;
			}

			data = null;
			etag = null;
			return false;
		}

		public void Save(string key, byte[] data, string etag)
		{
			cache[key] = new CacheEntry
			{
				Data = data,
				ETag = etag
			};
		}

		public void Remove(string key)
		{
			cache.Remove(key);
		}
	}
}