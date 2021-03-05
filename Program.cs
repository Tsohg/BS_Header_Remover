using System;
using System.Collections.Generic;
using System.IO;

namespace BS_Header_Remover
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputDir = args[0];
            string outputDir = args[1];
            string halt = args[2];
            CheckDir(inputDir);
            CheckDir(outputDir);
            List<string> lua = FindLua(inputDir);

            foreach (var l in lua)
                RemoveBitsquidHeader(l, outputDir);

            Console.Out.WriteLine("Removal done.");

            if(halt != null && halt == "halt")
                Console.Read();
        }

        private static void RemoveBitsquidHeader(string file, string outputDir)
        {
            int magic = 0x014a4c1b;

            using (var stream = File.Open(file, FileMode.Open))
            {
                byte[] buf = new byte[4];
                stream.Seek(8, SeekOrigin.Begin);
                stream.Read(buf, 0, 4);
                int testMagic = BitConverter.ToInt32(buf, 0);

                if (testMagic == magic)
                {
                    byte[] trimFile = new byte[stream.Length - 8];
                    stream.Seek(8, SeekOrigin.Begin);
                    stream.Read(trimFile, 0, (int)stream.Length - 8);
                    WriteFile(outputDir + @"\" + Path.GetFileName(file), trimFile);
                }
                else
                    Console.Out.WriteLine("The file is not a LuaJit compiled lua file or it lacks the bitsquid header already: " + file);
            }
        }

        private static List<string> FindLua(string inputDir)
        {
            List<string> lua = new List<string>();
            foreach (var file in Directory.GetFiles(inputDir))
                if (Path.GetExtension(file) == ".lua")
                    lua.Add(file);
            return lua;
        }

        private static void CheckDir(string dir)
        {
            if (!Directory.Exists(dir))
                Console.Out.WriteLine("Directory does not exist: " + dir);
        }

        private static void WriteFile(string path, byte[] contents)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(path));
            bw.Write(contents);
            bw.Close();
        }
    }
}
