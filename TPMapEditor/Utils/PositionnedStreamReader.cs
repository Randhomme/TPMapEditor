using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Utils
{
    /// <summary>
    /// A StreamReader that keeps track of the current byte position in the stream.
    /// </summary>
    public class PositionnedStreamReader : StreamReader
    {
        private long currentPosition = 0;

        public long CurrentPosition
        {
            get => currentPosition;
            set
            {
                if (currentPosition != value && BaseStream.CanSeek)
                {
                    currentPosition = value;
                    BaseStream.Seek(value, SeekOrigin.Begin);
                    DiscardBufferedData();
                }
            }
        }

        #region Constructors

        public PositionnedStreamReader(Stream stream) : base(stream)
        {
        }

        public PositionnedStreamReader(string path) : base(path)
        {
        }

        public PositionnedStreamReader(Stream stream, bool detectEncodingFromByteOrderMarks) : base(stream, detectEncodingFromByteOrderMarks)
        {
        }

        public PositionnedStreamReader(Stream stream, Encoding encoding) : base(stream, encoding)
        {
        }

        public PositionnedStreamReader(string path, bool detectEncodingFromByteOrderMarks) : base(path, detectEncodingFromByteOrderMarks)
        {
        }

        public PositionnedStreamReader(string path, Encoding encoding) : base(path, encoding)
        {
        }

        public PositionnedStreamReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks) : base(stream, encoding, detectEncodingFromByteOrderMarks)
        {
        }

        public PositionnedStreamReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks) : base(path, encoding, detectEncodingFromByteOrderMarks)
        {
        }

        public PositionnedStreamReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize) : base(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize)
        {
        }

        public PositionnedStreamReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize) : base(path, encoding, detectEncodingFromByteOrderMarks, bufferSize)
        {
        }

        public PositionnedStreamReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize, bool leaveOpen) : base(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize, leaveOpen)
        {
        }

        #endregion

        public override string ReadLine()
        {
            var line = base.ReadLine();
            if (line != null)
                CurrentPosition += CurrentEncoding.GetByteCount(line) + CurrentEncoding.GetByteCount(Environment.NewLine);
            return line;
        }
    }
}
