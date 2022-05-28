using System;
using System.IO;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace UnityOggConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            FFmpeg.SetExecutablesPath("C:\\Apps\\ffmpeg", ffmpegExeutableName: "FFmpeg");
            if (args.Length > 0)
            {
                DirectoryInfo dir = new DirectoryInfo(args[0]);
                Task task = ConvertDirectoryToOgg(dir);
                while (task.IsCompleted == false) { }
            }
            else Console.WriteLine("OggConverter.exe {startDirectory}");
        }
        /// <summary>
        /// recursivly convert all wav files in directory to ogg
        /// </summary>
        /// <param name="dirInfo"></param>
        static async Task ConvertDirectoryToOgg(DirectoryInfo dir)
        {
            Console.WriteLine($"converting directory {dir.FullName}");
            foreach (FileInfo file in dir.GetFiles())
            {
                try
                {
                    if (file.Extension == ".wav")
                    {
                        string outFile = Path.Combine(dir.FullName, Path.GetFileNameWithoutExtension(file.Name) + ".ogg");
                        string arguments = $"-i \"{file.FullName}\" -c:a libvorbis -b:a 64k \"{outFile}\"";
                        Console.WriteLine($"converting wav file {file.Name} to ogg");
                        var conversionResult = await FFmpeg.Conversions.New().Start(arguments);
                        file.Delete();
                    }
                    else if (file.Extension == ".meta")
                    {
                        Console.WriteLine($"converting meta file {file.Name} to ogg");
                        string newName = file.FullName.Replace(".wav", ".ogg");
                        file.MoveTo(newName);
                    }

                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Could not convert file {file.FullName} -- {ex.Message} {ex.StackTrace}");
                }
            }
            foreach(var subDir in dir.GetDirectories())
            {
                await ConvertDirectoryToOgg(subDir);
            }
        }
    }
}
