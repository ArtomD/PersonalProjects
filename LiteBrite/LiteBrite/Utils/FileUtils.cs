using LiteBrite.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace LiteBrite.Utils
{
    class FileUtils
    {
        public static string saveFilePath;
        public static void saveFile(BoardFile file, String path)
        {
            BinaryFormatter bf = new BinaryFormatter();

            FileStream fsout = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);

            using (fsout)
            {
                bf.Serialize(fsout, file);

            }

        }

        public static BoardFile loadFile(string filename)
        {
            BoardFile board = new BoardFile();

            BinaryFormatter bf = new BinaryFormatter();

            FileStream fsin = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None);

            using (fsin)
            {
                board = (BoardFile)bf.Deserialize(fsin);

            }
            return board;

        }

        public static bool fileExists(string name)
        {
            return File.Exists(name + ".binary");
        }

        public static List<string> getAllFiles()
        {
            return Directory.GetFiles(saveFilePath, "*.binary", System.IO.SearchOption.TopDirectoryOnly).ToList<string>();
        }
    }
}
