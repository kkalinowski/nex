using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace nex.Dialogs.FileCompareDialog
{
    /// <summary>
    /// Interaction logic for FileCompareDialog.xaml
    /// </summary>
    public partial class FileCompareDialog
    {
        #region Fields
        private string[] leftContent;
        private string[] rightContent;
        #endregion

        #region PropDP
        public string LeftFile
        {
            get
            {
                return (string)GetValue(LeftFileProperty);
            }
            set
            {
                SetValue(LeftFileProperty, value);
            }
        }
        public static readonly DependencyProperty LeftFileProperty =
            DependencyProperty.Register("LeftFile", typeof(string), typeof(FileCompareDialog));

        public string RightFile
        {
            get
            {
                return (string)GetValue(RightFileProperty);
            }
            set
            {
                SetValue(RightFileProperty, value);
            }
        }
        public static readonly DependencyProperty RightFileProperty =
            DependencyProperty.Register("RightFile", typeof(string), typeof(FileCompareDialog));
        #endregion

        public FileCompareDialog(string leftFile, string rightFile)
        {
            LeftFile = leftFile;
            RightFile = rightFile;

            InitializeComponent();
        }

        private void bChangeFile_Click(object sender, RoutedEventArgs e)
        {
            bool left = sender == bChangeLeftFile;
            System.Windows.Forms.OpenFileDialog ofDialog = new System.Windows.Forms.OpenFileDialog();
            ofDialog.FileName = left ? LeftFile : RightFile;

            if (ofDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (left)
                    LeftFile = ofDialog.FileName;
                else
                    RightFile = ofDialog.FileName;
            }
        }

        private void bCompare_Click(object sender, RoutedEventArgs e)
        {
            //read file content
            leftContent = File.ReadAllLines(LeftFile);
            rightContent = File.ReadAllLines(RightFile);

            //create and assign documents
            tLeftContent.Document = CreateDocumentFromFile(leftContent);
            tRightContent.Document = CreateDocumentFromFile(rightContent);

            //((Paragraph)tLeftContent.Document.Blocks.FirstBlock).Inlines.FirstInline.Background = Brushes.Red;
            //((Paragraph)tRightContent.Document.Blocks.FirstBlock).Inlines.FirstInline.Background = Brushes.Green;

            int[,] c = CreateLCSTable();
            List<int>[] lcs = GetMatchingRecords(c);

            //get access to lines of document
            Paragraph leftP = (Paragraph)tLeftContent.Document.Blocks.FirstBlock;
            Inline[] leftLines = leftP.Inlines.ToArray();
            Paragraph rightP = (Paragraph)tRightContent.Document.Blocks.FirstBlock;
            Inline[] rightLines = rightP.Inlines.ToArray();

            //go through left lines and higligth unmatched
            for (int i = 0; i < leftContent.Length; i++)
                if (lcs[0].Contains(i))
                    lcs[0].Remove(i);
                else
                    leftLines[i].Background = Brushes.Red;

            //go through right lines and higligth unmatched
            for (int i = 0; i < rightContent.Length; i++)
                if (lcs[1].Contains(i))
                    lcs[1].Remove(i);
                else
                    rightLines[i].Background = Brushes.Red;
        }

        private FlowDocument CreateDocumentFromFile(string[] content)
        {
            //create document
            FlowDocument doc = new FlowDocument();
            doc.PageWidth = 1000;
            Paragraph p = new Paragraph();
            doc.Blocks.Add(p);

            //write content to document
            foreach (string line in content)
            {
                p.Inlines.Add(line);
                p.Inlines.Add(new LineBreak());
            }

            return doc;
        }

        /// <summary>
        /// Create C table for Longest Common Sequence algorithm
        /// </summary>
        /// <returns>C table for Longest Common Sequence algorithm</returns>
        private int[,] CreateLCSTable()
        {
            int n = leftContent.Length, m = rightContent.Length;
            int[,] c = new int[n, m];

            for (int i = 1; i < n; i++)
                for (int j = 1; j < m; j++)
                    if (leftContent[i] == rightContent[j])
                        c[i, j] = c[i - 1, j - 1] + 1;
                    else
                        c[i, j] = Math.Max(c[i - 1, j], c[i, j - 1]);

            return c;
        }

        /// <summary>
        /// Recover matching records on both leftContent and rightContent tables
        /// </summary>
        /// <param name="c">C table of LCS algorithm</param>
        /// <returns>Matching records on both leftContent and rightContent tables</returns>
        private List<int>[] GetMatchingRecords(int[,] c)
        {
            int i = leftContent.Length - 1, j = rightContent.Length - 1;
            List<int>[] lcs = new List<int>[2];
            lcs[0] = new List<int>(c[i, j]);
            lcs[1] = new List<int>(c[i, j]);

            while (true)
            {
                //step 1.
                if (i != 0 && c[i - 1, j] == c[i, j])
                    i--;
                else if (j != 0 && c[i, j - 1] == c[i, j])
                    j--;
                else //step 3.
                {
                    lcs[0].Add(i);//leftContent match
                    lcs[1].Add(j);//rightContent match

                    //move up and left
                    i--;
                    j--;
                }

                //step 2. and 4.
                if (i == 0 && j == 0)
                    break;
            }

            return lcs;
        }

        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}