using System;
using System.IO;

namespace nex.FileSystem
{
    /// <summary>
    /// Struct represents size of a file or directory
    /// </summary>
    [Serializable]
    public struct FileSize
    {
        //TODO: It should use special WinAPI function for drive size
        #region Fields
        private double size;
        private FileSizeUnit unit;
        public static FileSize Empty = new FileSize(0);
        #endregion

        #region Props
        /// <summary>
        /// Get size of file/directory
        /// </summary>
        public double Size
        {
            get
            {
                return size;
            }
            private set
            {
                size = value;
            }
        }

        /// <summary>
        /// Get unit of file size
        /// </summary>
        public FileSizeUnit Unit
        {
            get
            {
                return unit;
            }
            set
            {
                unit = value;
                //TODO: Convert val
                //OPT: Check if anybody use it
            }
        }
        #endregion

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="bytes">File size</param>
        /// <param name="isDir">Is directory</param>
        private FileSize(long bytes)
        {
            if (bytes < 0)
                throw new ArgumentException("File size must be grater than 0");
            else if (bytes == 0)
            {
                size = 0;
                unit = FileSizeUnit.Byte;
            }
            else
            {
                unit = FileSizeUnit.Byte;
                size = bytes;

                RecomputeSize();
            }
        }

        /// <summary>
        /// Computes new size unit
        /// </summary>
        private void RecomputeSize()
        {
            while (size >= 1000)
            {
                size /= 1024.0;
                ++unit;
            }
        }

        /// <summary>
        /// Creates new FileSize for file from path
        /// </summary>
        /// <param name="filename">Path to file</param>
        /// <returns>New FileSize</returns>
        public static FileSize CreateForFile(string filename)
        {
            FileInfo file = new FileInfo(filename);
            return new FileSize(file.Length);
        }

        /// <summary>
        /// Creates new FileSize for file system object
        /// </summary>
        /// <param name="filename">Path to object</param>
        /// <returns>New FileSize</returns>
        public static FileSize CreateForUnknownObject(string path)
        {
            if (File.Exists(path))
                return CreateForFile(path);
            else
                return CreateForDirectory(path);
        }

        /// <summary>
        /// Creates FileSize for directory
        /// </summary>
        /// <param name="dirname">Path to directory</param>
        /// <returns>New FileSize</returns>
        public static FileSize CreateForDirectory(string dirname)
        {
            DirectoryInfo di = new DirectoryInfo(dirname);
            FileSize sum = FileSize.Empty;

            foreach (DirectoryInfo dir in di.GetDirectories())
                sum += CreateForDirectory(dir.FullName);

            foreach (FileInfo file in di.GetFiles())
                sum += CreateForFile(file.FullName);

            return sum;
        }

        /// <summary>
        /// Creates FileSize from bytes count
        /// </summary>
        /// <param name="bytes">File size</param>
        /// <returns>Return new FileSize instance</returns>
        public static FileSize CreateFromBytes(long bytes)
        {
            return new FileSize(bytes);
        }

        /// <summary>
        /// Round up size to specified unit
        /// </summary>
        /// <param name="fileSizeUnit">New FileSizeUnit</param>
        private void UnitUp(FileSizeUnit upUnit)
        {
            while (unit != upUnit && unit < FileSizeUnit.PetaByte)
            {
                size /= 1024.0;
                ++unit;
            }
        }

        /// <summary>
        /// Short ToString of FileSizeUnit
        /// </summary>
        /// <param name="unit">Unit to string</param>
        /// <returns>Shortcut of unit</returns>
        public string ShortUnitForm(FileSizeUnit unit)
        {
            switch (unit)
            {
                case FileSizeUnit.Unknown:
                    return string.Empty;
                case FileSizeUnit.Bit:
                    return "b";
                case FileSizeUnit.Byte:
                    return "B";
                case FileSizeUnit.KiloByte:
                    return "kB";
                case FileSizeUnit.MegaByte:
                    return "MB";
                case FileSizeUnit.GigaByte:
                    return "GB";
                case FileSizeUnit.TeraByte:
                    return "TB";
                case FileSizeUnit.PetaByte:
                    return "PB";
                default:
                    throw new NotImplementedException("There is no such unit");
            }
        }

        /// <summary>
        /// Return size in bytes
        /// </summary>
        /// <returns>Size in bytes</returns>
        public long ToBytes()
        {
            switch (unit)
            {
                case FileSizeUnit.Unknown:
                    return 0;
                case FileSizeUnit.Bit:
                    return 0;
                case FileSizeUnit.Byte:
                    return (long)Size;
                case FileSizeUnit.KiloByte:
                    return (long)(Size * 1024);
                case FileSizeUnit.MegaByte:
                    return (long)(Size * 1048576);
                case FileSizeUnit.GigaByte:
                    return (long)(Size * 1073741824);
                case FileSizeUnit.TeraByte:
                    return (long)(Size * 1099511627776);
                case FileSizeUnit.PetaByte:
                    return (long)(Size * 1125899906842624);
                default:
                    throw new NotImplementedException("There is no such unit");
            }
        }

        #region Operators
        public static FileSize operator +(FileSize first, FileSize second)
        {
            FileSize sum = FileSize.Empty;
            if (first.Unit == second.Unit)
            {
                sum.Size = first.Size + second.Size;
                sum.Unit = first.Unit;
                sum.RecomputeSize();
            }
            else if (first.Unit > second.Unit)
            {
                sum.Size = first.Size;
                sum.Unit = first.Unit;

                second.UnitUp(first.Unit);
                sum.Size += second.Size;

                sum.RecomputeSize();
            }
            else
            {
                sum.size = second.Size;
                sum.Unit = second.Unit;

                first.UnitUp(second.Unit);
                sum.Size += first.Size;

                sum.RecomputeSize();
            }

            return sum;
        }

        #endregion

        #region ObjectOverride
        public override string ToString()
        {
            if (Size == 0)
                return "...";
            else
                return string.Format("{0:0.##}", Size) + ShortUnitForm(unit);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            FileSize b = (FileSize)obj;
            return Size == b.Size && Unit == b.Unit;
        }

        public override int GetHashCode()
        {
            return Size.GetHashCode() + Unit.GetHashCode();
        }
        #endregion
    }
}