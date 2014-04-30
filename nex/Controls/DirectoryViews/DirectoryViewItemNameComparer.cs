using System.Collections.Generic;
using nex.DirectoryView;

namespace nex.Controls.DirectoryViews
{
    public class DirectoryViewItemNameComparer : IEqualityComparer<IDirectoryViewItem>
    {
        private static DirectoryViewItemNameComparer instance;

        public static DirectoryViewItemNameComparer Instance
        {
            get
            {
                if (instance == null)
                    instance = new DirectoryViewItemNameComparer();
                return instance;
            }
        }

        public bool Equals(IDirectoryViewItem x, IDirectoryViewItem y)
        {
            return x == null || y == null ? false : x.Name == y.Name;
        }

        public int GetHashCode(IDirectoryViewItem obj)
        {
            return obj.FullName.GetHashCode();
        }
    }
}