using System.Security.Cryptography;
using System.Text;

namespace PresentationLayer.Helpers
{
    public static class FileVersionHelper
    {
        private static readonly Dictionary<string, string> _cache = new Dictionary<string, string>();

        public static string GetFileVersion(IWebHostEnvironment env, string filePath)
        {
            // Si ya está en caché, retornar
            if (_cache.TryGetValue(filePath, out var cachedVersion))
            {
                return cachedVersion;
            }

            try
            {
                var physicalPath = Path.Combine(env.WebRootPath, filePath.TrimStart('~', '/'));

                if (File.Exists(physicalPath))
                {
                    var fileInfo = new FileInfo(physicalPath);
                    var lastModified = fileInfo.LastWriteTimeUtc.Ticks.ToString();

                    // Usar hash MD5 del timestamp para versión más corta
                    using (var md5 = MD5.Create())
                    {
                        var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(lastModified));
                        var version = BitConverter.ToString(hashBytes).Replace("-", "").Substring(0, 8).ToLower();
                        _cache[filePath] = version;
                        return version;
                    }
                }
            }
            catch
            {
                // Si hay error, usar timestamp actual
            }

            // Fallback: timestamp actual
            return DateTime.Now.Ticks.ToString();
        }
    }
}
