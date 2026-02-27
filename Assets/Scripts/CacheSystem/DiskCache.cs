using System.IO;
using UnityEngine;
namespace Alireza97Khish.CacheSystem
{
	public class DiskCache : ICacheSystem
	{
		string root;

		public DiskCache()
		{
			root = Path.Combine(Application.persistentDataPath, "DownloaderCache");

			if (!Directory.Exists(root))
				Directory.CreateDirectory(root);
		}

		string DataPath(string key)
			=> Path.Combine(root, HashUtility.Hash(key));

		string MetaPath(string key)
			=> DataPath(key) + ".meta";

		public bool TryGet(string key, out byte[] data, out string etag)
		{
			string path = DataPath(key);

			if (!File.Exists(path))
			{
				data = null;
				etag = null;
				return false;
			}

			data = File.ReadAllBytes(path);

			string meta = MetaPath(key);

			etag = File.Exists(meta) ? File.ReadAllText(meta) : null;

			return true;
		}

		public void Save(string key, byte[] data, string etag)
		{
			File.WriteAllBytes(DataPath(key), data);

			if (!string.IsNullOrEmpty(etag))
				File.WriteAllText(MetaPath(key), etag);
		}

		public void Remove(string key)
		{
			var path = DataPath(key);

			if (File.Exists(path))
				File.Delete(path);
		}
	}
}