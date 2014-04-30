using System;
using System.Collections.ObjectModel;
using System.Linq;
using lib12.Collections;
using lib12.DependencyInjection;
using lib12.WPF.Core;
using nex.Operations;

namespace nex
{
    [Singleton]
    public sealed class OperationsManager : NotifyingObject
    {
        #region Props
        private bool areAnyOperationsOngoing;
        public bool AreAnyOperationsOngoing
        {
            get { return areAnyOperationsOngoing; }
            set
            {
                areAnyOperationsOngoing = value;
                OnPropertyChanged("AreAnyOperationsOngoing");
            }
        }

        private bool error;
        public bool Error
        {
            get { return error; }
            set
            {
                error = value;
                OnPropertyChanged("Error");
            }
        }

        private string currentlyProcessedFile;
        public string CurrentlyProcessedFile
        {
            get { return currentlyProcessedFile; }
            set
            {
                currentlyProcessedFile = value;
                OnPropertyChanged("CurrentlyProcessedFile");
            }
        }

        private double progress;
        public double Progress
        {
            get { return progress; }
            set
            {
                progress = value;
                OnPropertyChanged("Progress");
            }
        }

        public ObservableCollection<OperationBase> Pending { get; private set; }
        public ObservableCollection<OperationBase> Finished { get; private set; }
        #endregion

        #region ctor
        public OperationsManager()
        {
            Pending = new ObservableCollection<OperationBase>();
            Finished = new ObservableCollection<OperationBase>();
        }
        #endregion

        #region Logic
        public void ExecuteOperation(OperationBase operation)
        {
            Pending.Add(operation);
            operation.Finished += operation_Finished;
            operation.Error += operation_Error;
            if (operation.IsComplexOperation)
                operation.Progressed += operation_Progressed;

            AreAnyOperationsOngoing = true;
            operation.Start();
        }

        public void CancelOperation(OperationBase operation)
        {
            operation.Cancel();
            CleanAfterOperation(operation);
        }

        public void UndoOperation(OperationBase operation)
        {
            operation.Undo();
            Finished.Remove(operation);
        }

        private void operation_Progressed(object sender, EventArgs e)
        {
            Progress = Pending.Sum(x => x.Progress) / Pending.Count;
        }

        private void operation_Error(object sender, OperationErrorEventArgs e)
        {
            Error = e.Error;
        }

        private void operation_Finished(object sender, EventArgs e)
        {
            var operation = (OperationBase)sender;
            CleanAfterOperation(operation);
            WpfUtilities.ThreadSafeInvoke(() => Finished.Add(operation));
        }

        private void CleanAfterOperation(OperationBase operation)
        {
            operation.Finished -= operation_Finished;
            if (operation.IsComplexOperation)
                operation.Progressed -= operation_Progressed;

            WpfUtilities.ThreadSafeInvoke(() => Pending.Remove(operation));
            CheckOngoingOperations();
        }

        private void CheckOngoingOperations()
        {
            AreAnyOperationsOngoing = Pending.IsNotEmpty();
        }
        #endregion
    }
}
