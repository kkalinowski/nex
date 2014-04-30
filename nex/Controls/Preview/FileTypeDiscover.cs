using System.Collections.Generic;
using lib12.Collections;
using nex.FileSystem;

namespace nex.Controls.Preview
{
    public static class FileTypeDiscover
    {
        private static readonly Dictionary<string, FileContentType> FileTypeDescriptors = new Dictionary<string, FileContentType>
        {
            {"txt", FileContentType.Text},
            {"pdf", FileContentType.Pdf},
            {"png", FileContentType.Image},
            {"jpg", FileContentType.Image},
            {"bmp", FileContentType.Image},
            {"gif", FileContentType.Image},
            {"avi", FileContentType.Video},
            {"mkv", FileContentType.Video},
            {"mov", FileContentType.Video},
            {"mpg", FileContentType.Video},
            {"mpeg", FileContentType.Video},
            {"mp3", FileContentType.Audio},
            {"wab", FileContentType.Audio},
            {"midi", FileContentType.Audio},
            {"mid", FileContentType.Audio},
            {"wma", FileContentType.Audio},
            {"m4a", FileContentType.Audio},
            {"c", FileContentType.SourceCode},
            {"h", FileContentType.SourceCode},
            {"cpp", FileContentType.SourceCode},
            {"cs", FileContentType.SourceCode},
            {"java", FileContentType.SourceCode},
            {"xml", FileContentType.SourceCode},
            {"xaml", FileContentType.SourceCode},
            {"py", FileContentType.SourceCode},
            {"ruby", FileContentType.SourceCode},
            {"sql", FileContentType.SourceCode},
            {"bat", FileContentType.SourceCode},
            {"html", FileContentType.SourceCode},
            {"htm", FileContentType.SourceCode},
            {"css", FileContentType.SourceCode},
        };

        public static FileContentType DiscoverType(string path)
        {
            var ext = PathExt.GetExtensionWithoutDot(path);
            return FileTypeDescriptors.GetValueOrDefault(ext);
        }
    }
}
