using System;
using System.Threading;
using lib12.Extensions;
using lib12.WPF.Core;
using nex.FileSystem;

namespace nex.Operations
{
    /// <summary>
    /// Base class for operations on file system that take some time
    /// </summary>
    [Serializable]
    public abstract class OperationBase : NotifyingObject
    {
        #region Events
        [field: NonSerialized]
        public event EventHandler Progressed;
        [field: NonSerialized]
        public event EventHandler<OperationErrorEventArgs> Error;
        [field: NonSerialized]
        public event EventHandler Finished;

        /// <summary>   
        /// Raise a OperationFinished event
        /// </summary>
        protected virtual void OnFinished()
        {
            Progress = 100;
            IsFinished = true;

            var handler = Finished;
            if (handler != null)
                handler(this, new EventArgs());
        }
        #endregion

        #region Fields
        private bool finished;
        private double progress = 0;
        #endregion

        #region Props
        /// <summary>
        /// Determine if operation has been canceled
        /// </summary>
        public bool IsCanceled { get; protected set; }

        /// <summary>
        /// Determine if operation can be undone
        /// </summary>
        public abstract bool CanUndo { get; }

        private string operationName;
        public string OperationName
        {
            get { return operationName; }
            set
            {
                operationName = value;
                OnPropertyChanged("OperationName");
            }
        }

        public FileSystemBase FileSystem { get; protected set; }

        /// <summary>
        /// Get or set operation progress
        /// </summary>
        public double Progress
        {
            get
            {
                return progress;
            }
            protected set
            {
                progress = value;
                OnPropertyChanged("Progress");
                Progressed.Raise(this);
            }
        }

        /// <summary>
        /// Determine if operation has been finished
        /// </summary>
        public bool IsFinished
        {
            get
            {
                return finished;
            }
            private set
            {
                finished = value;
                OnPropertyChanged("Finished");
            }
        }

        /// <summary>
        /// Determine if operation is complex, on many files
        /// </summary>
        public virtual bool IsComplexOperation
        {
            get
            {
                return false;
            }
        }
        #endregion

        #region ctor
        /// <summary>
        /// Ctor
        /// </summary>
        public OperationBase(FileSystemBase fileSystem)
        {
            IsCanceled = false;
            IsFinished = false;
            FileSystem = fileSystem;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Starts the operation - creates new thread calling Execute
        /// </summary>
        public virtual void Start()
        {
            Thread thread = new Thread(new ThreadStart(Execute));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        /// <summary>
        /// The work is do here
        /// </summary>
        protected abstract void Execute();

        /// <summary>
        /// Cancel operation
        /// </summary>
        public void Cancel()
        {
            IsCanceled = true;
        }

        /// <summary>
        /// Undo the operation
        /// </summary>
        public virtual void Undo()
        {
        }

        #endregion
    }
}