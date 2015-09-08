using Selenium.Serializer;
using System.IO;

namespace Selenium.Core {

    class DriverExtension : IJsonBinary {

        private string _path;

        public DriverExtension(string path) {
            _path = path;
        }

        public string Path {
            get {
                return _path;
            }
        }

        public void Save(Stream stream) {
            byte[] buffer = new byte[1024 * 64];
            int read = buffer.Length;
            using (var file = File.Open(_path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                while ((read = file.Read(buffer, 0, read)) > 0)
                    stream.Write(buffer, 0, read);
            }
        }

    }

}
