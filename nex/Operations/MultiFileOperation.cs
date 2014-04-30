using System;
using nex.DirectoryView;
using nex.FileSystem;

namespace nex.Operations
{
    /// <summary>
    /// Base class for for multi files operations
    /// </summary>
    [Serializable]
    public abstract class MultiFileOperation : OperationBase
    {
        #region Fields
        private IDirectoryViewItem currentItem;
        private TimeSpan duration;
        private FileSize speed;
        private TimeSpan estimatedEnd;
        #endregion

        #region Props
        /// <summary>
        /// Get or pset time when operation has been started
        /// </summary>
        public DateTime Started { get; private set; }

        /// <summary>
        /// Operation duration
        /// </summary>
        public TimeSpan Duration
        {
            get
            {
                return duration;
            }
            protected set
            {
                duration = value;
                OnPropertyChanged("Duration");
            }
        }

        /// <summary>
        /// Operation speed per second
        /// </summary>
        public FileSize Speed
        {
            get
            {
                return speed;
            }
            protected set
            {
                speed = value;
                OnPropertyChanged("Speed");
            }
        }

        /// <summary>
        /// EstimatedEnd of operation
        /// </summary>
        public TimeSpan EstimatedEnd
        {
            get
            {
                return estimatedEnd;
            }
            protected set
            {
                estimatedEnd = value;
                OnPropertyChanged("EstimatedEnd");
            }
        }

        /// <summary>
        /// Operation Items
        /// </summary>
        public IDirectoryViewItem[] Items { get; protected set; }

        /// <summary>
        /// Size of files to work
        /// </summary>
        public FileSize ItemsSize { get; protected set; }

        /// <summary>
        /// Get or set current item on which operation operate
        /// </summary>
        public IDirectoryViewItem CurrentItem
        {
            get
            {
                return currentItem;
            }
            protected set
            {
                currentItem = value;
                OnPropertyChanged("CurrentItem");
            }
        }

        public override bool IsComplexOperation
        {
            get
            {
                return true;
            }
        }
        #endregion

        #region Methods
        public override void Start()
        {
            Started = DateTime.Now;
            base.Start();
        }

        /// <summary>
        /// Counts the size of items
        /// </summary>
        protected void CountSize()
        {
            foreach (IDirectoryViewItem item in Items)
            {
                if (item.IsDirectory)
                    item.CountFolderSize();
                ItemsSize += item.Size;
            }
        }

        /// <summary>
        /// Rollback already made changes
        /// </summary>
        protected virtual void Rollback()
        {
        }

        /// <summary>   
        /// Raise a OperationFinished event
        /// </summary>
        protected override void OnFinished()
        {
            EstimatedEnd = TimeSpan.FromSeconds(0);
            base.OnFinished();
        }

        public override void Undo()
        {
            Rollback();
        }

        #endregion

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="items">Items to work on</param>
        public MultiFileOperation(IDirectoryViewItem[] items, FileSystemBase fileSystem) : base(fileSystem)
        {
            //TODO: Files in folders should be diffirent objects
            ItemsSize = FileSize.Empty;

            System.Diagnostics.Debug.Assert(items.Length > 0, "Empty operation!");
            Items = items;
            //CurrentItem = items[0];

            CountSize();
        }
    }
}