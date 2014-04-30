using System.Collections.Generic;
using nex.DirectoryView;

namespace nex.Controls.DirectoryViews
{
    public class DirectoryViewItemComparer : IEqualityComparer<IDirectoryViewItem>
    {
        private static DirectoryViewItemComparer instance;

        public static DirectoryViewItemComparer Instance
        {
            get
            {
                if (instance == null)
                    instance = new DirectoryViewItemComparer();
                return instance;
            }
        }

        public bool Equals(IDirectoryViewItem x, IDirectoryViewItem y)
        {
            return x == null || y == null ? false : x.FullName == y.FullName;
        }

        public int GetHashCode(IDirectoryViewItem obj)
        {
            return obj.FullName.GetHashCode();
        }
    }
}