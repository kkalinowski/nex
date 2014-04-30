using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace nex.Controls
{
    /// <summary>
    /// Interaction logic for CommandLineBox.xaml
    /// </summary>
    public partial class CommandLineBox : UserControl
    {
        #region Fields
        private Process psProcess;
        private StreamWriter input;
        #endregion

        #region Props
        public bool IsWorking { get; set; }
        #endregion

        public CommandLineBox()
        {
            IsWorking = false;

            RunPS();
            InitializeComponent();

            //start capturing output
            if (IsWorking)
                psProcess.BeginOutputReadLine();
        }

        private void RunPS()
        {
            //start info for ps
            ProcessStartInfo psStartInfo = new ProcessStartInfo("cmd");
            //psStartInfo.Arguments = "-NoExit";
            psStartInfo.ErrorDialog = false;
            psStartInfo.UseShellExecute = false;
            //psStartInfo.RedirectStandardError = true;
            psStartInfo.RedirectStandardInput = true;
            psStartInfo.RedirectStandardOutput = true;
            psStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            psStartInfo.CreateNoWindow = true;

            //create ps Process
            psProcess = new Process();
            psProcess.StartInfo = psStartInfo;

            try
            {
                //start ps
                if (psProcess.Start())
                {
                    //set up notyfing - using this you cannot use psProcess.StandardOutput
                    psProcess.OutputDataReceived += new DataReceivedEventHandler(psProcess_OutputDataReceived);

                    //set up streams
                    input = psProcess.StandardInput;
                    IsWorking = true;
                }
            }
            catch
            {
                //when ps isn't installed hide control
                this.Visibility = Visibility.Collapsed;
                IsWorking = false;
            }
        }

        void psProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            AddNewOutput(e.Data);
        }

        private void AddNewOutput(string newOutput)
        {
            if (Dispatcher.CheckAccess())
                tOutput.Text += string.Concat(newOutput, Environment.NewLine);
            else
                Dispatcher.Invoke(DispatcherPriority.Render, new Action<string>(AddNewOutput), newOutput);
        }

        ~CommandLineBox()
        {
            if (IsWorking)
                psProcess.Close();
        }

        private void CMD_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                if (tCommand.Text != string.Empty)
                    ExecuteCommand();
            }
        }

        private void ExecuteCommand()
        {
            input.WriteLine(tCommand.Text);
            tCommand.Text = string.Empty;
        }
    }
}