using System.IO;

namespace FlameVM.Core
{
    public class libxFile
    {
        private string _js;

        private libxFile(string s)
        {
            this._js = s;
        }

        public static libxFile Load(string s)
        {
            return new libxFile(s);
        }
        public static libxFile LoadFromFile(string path)
        {
            return new libxFile(File.ReadAllText(path));
        }

        public override string ToString()
        {
            return this._js;
        }
    }
}