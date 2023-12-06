using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace GameThing
{
    public class ProcessHandler
    {
        // PROPERTIES

        private const int PollInterval = 5 * 1000; // Seconds * 1000
        private const int UpdateInterval = 5 * 60 * 1000; // Minutes * 60 * 1000
        private const int AllowedLaunchTime = 15; // Time (in seconds) to wait for process to launch before it's considered failed to launch

        private string ProcessPath;
        private string OpenFilePath;
        private string ProcessArgs;
        private string ProcessName;
        private List<string> PreRunProcesses;
        private Process MyProcess;
        public DateTime? TimeStarted { get; private set; }
        public DateTime? TimeLastPolled { get; private set; }

        private Timer ListenerTimer;
        private Timer UpdateTimer;
        public event EventHandler OnPoll;
        public event EventHandler OnUpdate;
        public event EventHandler OnProcessEnd;

        public List<string> Errors { get; private set; }
        public TimeSpan RunTime
        {
            get
            {
                if (TimeStarted == null || TimeLastPolled == null) { return new TimeSpan(0); }
                return (TimeLastPolled ?? DateTime.Now) - (TimeStarted ?? DateTime.Now);
            }
        }


        // CONSTRUCTOR

        /// <summary>
        /// Handler class for running and managing applications
        /// </summary>
        /// <param name="applicationFilePath">File path of the application to run.</param>
        /// <param name="filePath">File path of the file to open in the application.</param>
        /// <param name="applicationArgs">Command line arguments to use for the application.</param>
        public ProcessHandler(string applicationFilePath, string filePath = null, string applicationArgs = null)
        {
            ProcessPath = applicationFilePath;
            OpenFilePath = filePath;
            ProcessArgs = applicationArgs;
            Errors = new List<string>();
        }


        // METHODS

        /// <summary>
        /// Get all currently running foreground processes
        /// </summary>
        /// <returns>A list of the running processes</returns>
        private List<Process> GetCurrentProcesses()
        {
            Process[] allProcesses = Process.GetProcesses();
            //List<Process> foregroundProcesses = allProcesses.Where(x => !string.IsNullOrEmpty(x.MainWindowTitle)).ToList();
            return allProcesses.ToList();
        }


        /// <summary>
        /// Check that the process is running and log the time
        /// </summary>
        private void PollProcess()
        {
            TimeLastPolled = DateTime.Now;

            List<Process> currentProcesses = GetCurrentProcesses();

            if (MyProcess == null)
            {
                //Find my process
                Process matchedProcess = currentProcesses.Find(x => x.ProcessName == ProcessName);
                if (matchedProcess == null)
                {
                    //Assume that my process is the first application that's now running that wasn't running before
                    matchedProcess = currentProcesses.Find(x => !PreRunProcesses.Contains(x.ProcessName));
                }
                MyProcess = matchedProcess;
                if (MyProcess == null)
                {
                    //If process still couldn't be found, it may have failed to run
                    if (RunTime.TotalSeconds > AllowedLaunchTime)
                    {
                        Errors.Add("Process could not be found. It likely failed to launch: " + ProcessName);
                        TimeLastPolled = null;
                        EndProcessListener();
                    }
                    return;
                }
                else
                {
                    ProcessName = MyProcess.ProcessName;
                    //TODO: Focus the process window?
                }
            }

            //Check that the process is running
            Process runningProcess = currentProcesses.Find(x => x.ProcessName == ProcessName);
            if (runningProcess == null)
            {
                //Process is not running
                EndProcessListener();
                return;
            }

            OnPoll?.Invoke(this, EventArgs.Empty);
        }


        /// <summary>
        /// Begin repeating interval for polling the process
        /// </summary>
        private void BeginProcessListener()
        {
            TimeStarted = DateTime.Now;

            try
            {
                //Start the main listener timer
                ListenerTimer = new Timer(PollInterval);
                ListenerTimer.Elapsed += ListenerTimer_Elapsed;
                ListenerTimer.Start();
                //Start the update timer
                UpdateTimer = new Timer(UpdateInterval);
                UpdateTimer.Elapsed += UpdateTimer_Elapsed;
                UpdateTimer.Start();
            }
            catch (Exception ex)
            {
                Tools.LogError(ex);
                Errors.Add("Error starting process listener: " + ex.Message);
            }
        }


        /// <summary>
        /// End process polling interval and finalise logging
        /// </summary>
        private void EndProcessListener()
        {
            try
            {
                //Stop main listener timer
                ListenerTimer.Stop();
                ListenerTimer.Dispose();
                //Stop update timer
                UpdateTimer.Stop();
                UpdateTimer.Dispose();
            }
            catch (Exception ex)
            {
                Tools.LogError(ex);
                Errors.Add("Error stopping process listener: " + ex.Message);
            }

            try
            {
                OnProcessEnd?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Tools.LogError(ex);
                Errors.Add("Error running Process End event: " + ex.Message);
            }
        }


        private void ListenerTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                PollProcess();
            }
            catch (Exception ex)
            {
                Tools.LogError(ex);
                Errors.Add("Error polling process: " + ex.Message);
            }
        }


        private void UpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                OnUpdate?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Tools.LogError(ex);
                Errors.Add("Error with process update: " + ex.Message);
            }
        }


        /// <summary>
        /// Open/run a given file/application.
        /// </summary>
        private void RunProcess()
        {
            Process.Start(ProcessPath);
        }


        /// <summary>
        /// Open a given file in a given application.
        /// </summary>
        private void RunProcessWithFile()
        {
            //Run a hidden Command Prompt process
            ProcessStartInfo runnerInfo = new ProcessStartInfo("cmd.exe")
            {
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                Verb = "runas" //Run as administrator (TODO: Doesn't work?)
            };
            Process cmdProc = new Process();
            cmdProc.StartInfo = runnerInfo;
            cmdProc.Start();

            string cmdArgs = "";
            if (!string.IsNullOrWhiteSpace(ProcessArgs))
            {
                cmdArgs = ProcessArgs.Trim() + " ";
            }

            string cmdCommand = $"\"{ProcessPath}\" {cmdArgs}\"{OpenFilePath}\"";

            //Execute commands in Command Prompt to run the application
            cmdProc.StandardInput.WriteLine(cmdCommand);
            cmdProc.StandardInput.Flush();
            cmdProc.StandardInput.Close();
            //Completely close the Command Prompt process
            cmdProc.Close();
            cmdProc.Dispose();
        }


        /// <summary>
        /// Run the process.
        /// </summary>
        public void Run()
        {
            if (TimeStarted != null)
            {
                throw new Exception("Process has already been run.");
            }

            IEnumerable<Process> currentProcesses = GetCurrentProcesses();
            PreRunProcesses = currentProcesses.Select(x => x.ProcessName).ToList();

            ProcessName = Path.GetFileNameWithoutExtension(ProcessPath);

            if (OpenFilePath == null)
            {
                RunProcess();
            }
            else
            {
                RunProcessWithFile();
            }
            
            BeginProcessListener();
        }


        /// <summary>
        /// Close the process.
        /// </summary>
        public void EndProcess()
        {
            if (TimeStarted == null) return;

            List<Process> currentProcesses = GetCurrentProcesses();
            Process runningProcess = currentProcesses.Find(x => x.ProcessName == ProcessName);

            if (runningProcess != null)
            {
                //CloseMainWindow would be cleaner, but we want to bypass any confirmation dialogs etc that may block immediate closing
                //TODO: Nicer way? Perhaps call CloseMainWindow, then use Kill if process is still running after a set time?
                //runningProcess.CloseMainWindow();
                runningProcess.Kill();
            }
        }


    }
}
