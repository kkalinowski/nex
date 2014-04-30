using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using lib12.Collections;
using nex.FileSystem;

namespace nex.Controls.Preview
{
    /// <summary>
    /// Interaction logic for SourceCodePreview.xaml
    /// </summary>
    public partial class SourceCodePreview : UserControl
    {
        #region Const
        private const string SyntaxtHighlightingDirectory = "SyntaxtHighlighting";
        private static readonly Dictionary<string, string> FileExtToLanguage = new Dictionary<string, string>
        {
            {"c", "C++"},
            {"h", "C++"},
            {"cc", "C++"},
            {"cpp", "C++"},
            {"hpp", "C++"},
            {"cs", "C#"},
            {"java", "Java"},
            {"xml", "XML"},
            {"xaml", "XML"},
            {"py", "py"},
            {"rb", "ruby"},
            {"sql", "sql"},
            {"html", "HTML"},
            {"htm", "HTML"},
            {"css", "CSS"},
        };
        #endregion

        #region ctor
        public SourceCodePreview()
        {
            InitializeComponent();
        }
        #endregion

        #region Load
        public void LoadSourceCodeFile(string fileName)
        {
            var lang = GetLanguageOfFile(fileName);
            var highlighting = LoadLanguageSyntaxHighlighting(lang);
            if (highlighting != null)
                tSource.SyntaxHighlighting = highlighting;

            tSource.Load(fileName);
        }

        private string GetLanguageOfFile(string fileName)
        {
            var ext = PathExt.GetExtensionWithoutDot(fileName);
            var lang = FileExtToLanguage.GetValueOrDefault(ext);
            return lang ?? ext;
        }

        private IHighlightingDefinition LoadLanguageSyntaxHighlighting(string lang)
        {
            var definition = HighlightingManager.Instance.GetDefinition(lang);
            if (definition != null)
                return definition;

            var xshdFile = Path.Combine(SyntaxtHighlightingDirectory, lang + ".xshd");
            if (!File.Exists(xshdFile))
                return null;

            using (var stream = File.Open(xshdFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (XmlReader reader = new XmlTextReader(stream))
                {
                    return HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
        }
        #endregion
    }
}