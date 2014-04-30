using System.Collections.Generic;

namespace nex.Dialogs.DirectorySynchronizeDialog
{
    public class DirectorySynchronizeResult
    {
        #region Props
        public List<DirectoryComparison> Comparison { get; set; }

        public string LeftDir { get; set; }
        public string RightDir { get; set; }
        #endregion

        public DirectorySynchronizeResult(List<DirectoryComparison> comparison, string leftDir, string rightDir)
        {
            Comparison = comparison;
            LeftDir = leftDir;
            RightDir = rightDir;
        }
    }
}