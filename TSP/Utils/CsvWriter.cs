using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace TSP.Utils
{
    public class CsvWriter
    {
        private readonly float[,] _data;
        private readonly string _fileName;
        private readonly string[] _headers;
        private FileStream _fStream;

        public CsvWriter(string fileName, string[] headers, float[,] data)
        {
            _fileName = fileName;
            _headers = headers;
            _data = data;
            if (data.GetLength(1) != headers.Length)
                throw new ArgumentException("Data/header size does not match data size.");
        }

        private void WriteLine(string line)
        {
            _fStream ??= File.Open(Directory.GetCurrentDirectory() +
                                   Path.DirectorySeparatorChar + _fileName, FileMode.Create);
            _fStream.Write(new UTF8Encoding(true).GetBytes(line + "\n"));
        }

        public void WriteFile()
        {
            _fStream = File.Open(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + _fileName,
                FileMode.Create);
            var sb = new StringBuilder();
            foreach (var header in _headers)
            {
                if (Array.IndexOf(_headers, header) != 0) sb.Append(',');
                sb.Append(header);
            }

            WriteLine(sb.ToString());

            for (var i = 0; i < _data.GetLength(0); i++)
            {
                var line = new StringBuilder();
                for (var j = 0; j < _data.GetLength(1); j++)
                    line.Append(_data[i, j].ToString(CultureInfo.InvariantCulture)).Append(',');

                line.Length--;
                WriteLine(line.ToString());
            }

            _fStream.Close();

            Console.WriteLine("File written to: {0}", _fStream.Name);
        }
    }
}