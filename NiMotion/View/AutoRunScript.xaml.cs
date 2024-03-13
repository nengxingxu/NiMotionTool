using Python.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using NiMotion.ViewModel;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Diagnostics;
using System.Management;
using Path = System.IO.Path;

namespace NiMotion.View
{
    /// <summary>
    /// RunMatlab.xaml 的交互逻辑
    /// </summary>
    public partial class AutoRunScript
    {
        private string pyScriptPath = AppDomain.CurrentDomain.BaseDirectory + "runmat.py";
        private string pyDLPScriptPath = AppDomain.CurrentDomain.BaseDirectory + "DLP.py";
        private List<string> matList = new List<string>();
        private CancellationTokenSource source = new CancellationTokenSource();
        private AutoRunScriptViewModel context = new AutoRunScriptViewModel();
        private Dictionary<string, string> matlabDict = new Dictionary<string, string>();
        private IntPtr threadState = IntPtr.Zero;
        private ulong pythonThreadID;
        private string matlabScriptPath = string.Empty;
        private bool isRunnigScript = false;
        private bool isCancleScript = false;
        private System.Diagnostics.Process curProcess = null;

        public delegate int MotorStart(double motorSpeed);
        public delegate void MotorStop();

        public event MotorStart MotorStartEvent;
        public event MotorStop MotorStopEvent;




        public AutoRunScript()
        {
            InitializeComponent();
            PythonRuntimeInit();
            DataContext = context;
            listView.ItemsSource = context.DataList;
            MatlabDictInit();
        }

        private void MatlabDictInit()
        {
            matlabDict.Add("9.9", "9.9.4");
            matlabDict.Add("9.10", "9.10.3");
            matlabDict.Add("9.11", "9.11.21");
            matlabDict.Add("9.12", "9.12.19");
            matlabDict.Add("9.13", "9.13.8");
            matlabDict.Add("9.14", "9.13.2");

        }

        private void PythonRuntimeInit()
        {
            try
            {
                Runtime.PythonDLL = AppDomain.CurrentDomain.BaseDirectory + @"\python-3.9.8-embed-amd64\python39.dll";
                PythonEngine.Initialize();
                threadState = PythonEngine.BeginAllowThreads();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //PythonEngine.EndAllowThreads(threadState);
            //PythonEngine.Shutdown();
        }

        private void CheckRunScriptPrepare()
        {
            if(context.MatDataPath == string.Empty)
            {
                throw new Exception("Select matalab data file first");
            }
            if (!System.IO.File.Exists(context.MatDataPath))
            {
                throw new Exception("Matalab data file not Exists");
            }
            if (context.GetListDataItemCount() <= 0)
            {
                throw new Exception("No matlab script need to run");
            }
        }

        private string SearchMatlabRootDirectory()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\MathWorks\MATLAB");
            if (key == null)
                throw new Exception("Not Matlab Install");

            if (key.GetSubKeyNames().Length == 0)
                throw new Exception("Not Matlab Install.");

            string subKeyName = key.GetSubKeyNames()[0];
            RegistryKey subkey = key.OpenSubKey(subKeyName);
            context.MatlabRootPath = subkey.GetValue("MATLABROOT").ToString();
            return subKeyName;
        }


        public async Task<int> ExecutePythonPip()
        {
            Task<int> myTask = Task.Run(() =>
            {
                int retValue = 0;
                try
                {
                    string matlab_version = SearchMatlabRootDirectory();
                    if (matlab_version == string.Empty)
                    {
                        throw new Exception("Not Matlab Install.");
                    }

                    if (matlabDict.ContainsKey(matlab_version))
                    {
                        string matlab_pip_version = matlabDict[matlab_version];
                        string ffmpegPath = AppDomain.CurrentDomain.BaseDirectory + "python-3.9.8-embed-amd64\\python.exe";
                        string ffmpegArgs = $" -m pip install matlabengine=={matlab_pip_version}";
                        System.Diagnostics.Process process = new System.Diagnostics.Process();
                        process.StartInfo.FileName = ffmpegPath;
                        process.StartInfo.Arguments = ffmpegArgs;
                        process.StartInfo.UseShellExecute = false;          // 不使用操作系统的 shell 执行
                        process.StartInfo.RedirectStandardOutput = true;    // 重定向标准输出
                        process.StartInfo.RedirectStandardError = true;     // 重定错误输出
                        process.StartInfo.CreateNoWindow = true;            // 不创建新窗口

                        process.Start();
                        string error_output = process.StandardError.ReadToEnd();
                        string output = process.StandardOutput.ReadToEnd();
                        process.WaitForExit();

                        if (process.ExitCode == 0)
                        {
                            // PrintLog("pip install success！" + output);
                            // PrintLog("pip install success！" + output);
                            retValue = 0;
                        }
                        else
                        {
                            //PrintLog("pip install fail : " + error_output);
                            retValue = 1;
                        }
                    }
                    else
                    {
                        throw new Exception($"The version {matlab_version} in the registry is not support");
                    }
                }
                catch (Exception ex)
                {
                    retValue = 1;
                    MessageBox.Show(ex.Message);
                }
                return retValue;
            });
            int ret =await myTask;
            return ret;
        }


        private async Task<int> CreaetPythonRunScript()
        {
            Task<int> myTask = Task.Run(() =>
            {
                int retValue = 0;
                if (File.Exists(pyScriptPath))
                {
                    File.Delete(pyScriptPath);
                }
                using (StreamWriter writer = new StreamWriter(pyScriptPath))
                {
                    writer.WriteLine("import matlab.engine");
                    writer.WriteLine("\n\n\n");
                    writer.WriteLine("def run_mat():");
                    writer.WriteLine("    eng = matlab.engine.start_matlab()");
                    writer.WriteLine($"    eng.eval(\"load('{context.MatDataPath}')\", nargout = 0);");
                    for (int i = 0; i < context.GetListDataItemCount(); i++)
                    {
                        var item = context.GetListItem(i);
                        string matscript_path = item.Path;
                        string path = System.IO.Path.GetDirectoryName(matscript_path);
                        path = path.Replace('\\', '/');
                        string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(matscript_path);
                        writer.WriteLine($"    eng.cd('{path}')");
                        writer.WriteLine($"    eng.{fileNameWithoutExtension}(nargout=0)");
                        matlabScriptPath = System.IO.Path.GetDirectoryName(matscript_path);
                    }
                    writer.WriteLine("    eng.quit()");
                    writer.WriteLine("\n\n\n");
                    writer.WriteLine("run_mat()");
                }

                return retValue;
            });

            int ret = await myTask;
            return ret;

        }

        public async Task<int> ExecutePythonScriptC()
        {
            
            Task<int> myTask = Task.Run(() =>
            {
                int ret = 0;
                string pythonExePath = AppDomain.CurrentDomain.BaseDirectory + "python-3.9.8-embed-amd64\\python.exe";
                string pythonArgs = pyScriptPath;
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                curProcess = process;
                process.StartInfo.FileName = pythonExePath;
                process.StartInfo.Arguments = pythonArgs;
                process.StartInfo.UseShellExecute = false;          // 不使用操作系统的 shell 执行
                process.StartInfo.RedirectStandardOutput = true;    // 重定向标准输出
                process.StartInfo.RedirectStandardError = true;     // 重定错误输出
                process.StartInfo.CreateNoWindow = true;            // 不创建新窗口
                process.Start();
                string error_output = process.StandardError.ReadToEnd();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                if (process.ExitCode == 0)
                {
                    ret = 0;
                }
                else
                {
                    ret = -1;
                }
                return ret;
            });
            int retValue = await myTask;
            return retValue;
        }

        public async Task<int> ExecutePythonScript()
        {
            Task<int> myTask = Task.Run(() =>
            {
                int ret = 0;
                using (Py.GIL())
                {
                    pythonThreadID = PythonEngine.GetPythonThreadID();
                    PyModule scope = Py.CreateScope();
                    try
                    {
                        string temp_path = matlabScriptPath + "\\David-DC";
                        if (Directory.Exists(temp_path))
                        {
                            string newPath = matlabScriptPath + "\\David-DC-" + DateTime.Now.ToString("yyyyMMdd-HHmmss");
                            Directory.Move(temp_path, newPath);
                        }
                        string code = File.ReadAllText(pyScriptPath); // Get the python file as raw text
                        var scriptCompiled = PythonEngine.Compile(code, pyScriptPath); // Compile the code/file
                        PyObject py_object = scope.Execute(scriptCompiled); // Execute the compiled python
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        ret = 1;
                    }
                    finally
                    {
                        scope.Dispose();
                    }
                }
                return ret;
            });

            int retValue = await myTask;
            return retValue;
        }

        public List<string> GetImageFiles(string imageDirectory)
        {
            try
            {
                return Directory.EnumerateFiles(imageDirectory)
                    .Where(file => IsImageFile(file))
                    .OrderBy(file => GetFileNumber(file))
                    .ToList();
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine($"Directory not found: {imageDirectory}");
                return new List<string>();
            }
        }

        private bool IsImageFile(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLower();
            return extension == ".jpg" || extension == ".png" || extension == ".tiff" || extension == ".tif";
        }

        private int GetFileNumber(string fileName)
        {
            string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string numericPart = new string(nameWithoutExtension.Where(char.IsDigit).ToArray());
            if (int.TryParse(numericPart, out int number))
            {
                return number;
            }
            return 0;
        }

        private async Task<int> InnerExecuteDLP(string inputPath)
        {
            Task<int> task = Task.Run(() =>
            {
                string inputFilePath = inputPath + "\\David-DC\\SC-1";
                return ExecuteDLP(inputFilePath);
            });
            int ret = await task;
            return ret;
        }

        private int ExecuteDLP(string inputPath)
        {
            int retValue = 1;

            List<string> listImage = GetImageFiles(inputPath);

            double speed = 360 / (listImage.Count / Convert.ToDouble(context.FPS));  //motor speed
            string pythonExePath = AppDomain.CurrentDomain.BaseDirectory + "python-3.9.8-embed-amd64\\python.exe";
            string pythonArgs = pyDLPScriptPath + " " + context.Cycles + " " + context.FPS + " " + context.FlushTime + " " + context.LED460Brightness + " "+ context.LED385Brightness + " " + context.SelectedIndex.ToString() + " " + inputPath;
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            curProcess = process;
            process.StartInfo.FileName = pythonExePath;
            process.StartInfo.Arguments = pythonArgs;
            process.StartInfo.UseShellExecute = true;          // 不使用操作系统的 shell 执行
            process.StartInfo.RedirectStandardOutput = false;    // 重定向标准输出
            process.StartInfo.RedirectStandardError = false;     // 重定错误输出
            process.StartInfo.CreateNoWindow = true;            // 不创建新窗口
            try
            {
                if (MotorStartEvent(speed) == 1)
                    process.Start();
                else
                    MessageBox.Show("Motor Start Failed");
            }
            catch (Exception ex)
            {
                retValue = 1;
                MotorStopEvent();
            }
            //string error_output = process.StandardError.ReadToEnd();
            //string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            MotorStopEvent();
            return retValue;
        }

        private async Task<int> InnerExecuteFFmpeg(string inputPath)
        {
            Task<int> task = Task.Run(() =>
            {
                string inputFilePath = inputPath + "\\David-DC\\SC-1";
                return ExecuteFFmpeg(inputFilePath);
            });
            int ret = await task;
            return ret;
        }

         private int ExecuteFFmpeg(string inputPath)
        {
            int retValue = 1;
            string ffmpegPath = AppDomain.CurrentDomain.BaseDirectory + "ffmpeg.exe";
            string inputFilePath = inputPath + "\\David_step-%05d.tiff";
            string outDir = inputPath + "/../video/" + DateTime.Now.ToString("yyyyMMdd/");
            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
            }
            string outputVideo = outDir + "ouput" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".mp4";

            if (System.IO.File.Exists(outputVideo))
            {
                System.IO.File.Delete(outputVideo);
            }
            string ffmpegArgs = $"-framerate 30 -i \"{inputFilePath}\" -y -c:v libvpx-vp9 -r 30 -pix_fmt yuv420p -vf pad=\"width = iw:height = ih + 1:x = 0:y = 0:color = white\" \"{outputVideo}\"";
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            curProcess = process;
            process.StartInfo.FileName = ffmpegPath;
            process.StartInfo.Arguments = ffmpegArgs;
            process.StartInfo.UseShellExecute = false;          // 不使用操作系统的 shell 执行
            process.StartInfo.RedirectStandardOutput = true;    // 重定向标准输出
            process.StartInfo.RedirectStandardError = true;     // 重定错误输出
            process.StartInfo.CreateNoWindow = true;            // 不创建新窗口

            try
            {
                process.Start();
                string error_output = process.StandardError.ReadToEnd();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    if(matlabScriptPath != string.Empty)
                    {
                        string newPath = matlabScriptPath + "\\David-DC-" + DateTime.Now.ToString("yyyyMMdd-HHmmss");
                        Directory.Move(matlabScriptPath + "\\David-DC\\", newPath);
                    }
                    retValue = 0;
                }
                else
                {
                    MessageBox.Show("FFmpeg fail :" + error_output);
                    retValue = 1;
                }
            }
            catch (Exception ex)
            {
                retValue = 1;
                MessageBox.Show(ex.Message);
            }
            return retValue;
        }

        private void Button_ImportMat_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "matlab data file (*.mat)|*.mat"; // 可以根据需要设置文件类型过滤器

            if (openFileDialog.ShowDialog() == true)
            {
                string path = openFileDialog.FileName;
                context.MatDataPath = path.Replace("\\", "/");
            }
        }

        private void Button_AddScript_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "matlab script file (*.m)|*.m"; // 可以根据需要设置文件类型过滤器

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                context.AddListDataItem(System.IO.Path.GetFileName(selectedFilePath), selectedFilePath);
            }
        }

        private void Button_DeleteScript_Click(object sender, RoutedEventArgs e)
        {
            int index = listView.SelectedIndex;
            context.RemoveListDataItem(index);
        }

        private void Button_ClearScriptList_Click(object sender, RoutedEventArgs e)
        {
            context.ClearListData();
        }


        private void ClearPythonEnv()
        {
            string pythonExePath = AppDomain.CurrentDomain.BaseDirectory + "python-3.9.8-embed-amd64\\python.exe";
            string pythonArgs = "-m pip uninstall matlabengine -y";
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            curProcess = process;
            process.StartInfo.FileName = pythonExePath;
            process.StartInfo.Arguments = pythonArgs;
            process.StartInfo.UseShellExecute = false;          // 不使用操作系统的 shell 执行
            process.StartInfo.RedirectStandardOutput = true;    // 重定向标准输出
            process.StartInfo.RedirectStandardError = true;     // 重定错误输出
            process.StartInfo.CreateNoWindow = true;            // 不创建新窗口


            try
            {
                process.Start();
                string error_output = process.StandardError.ReadToEnd();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    MessageBox.Show("pip uninstall fail :" + error_output);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void Button_ClearPythonEnvClick(object sender, RoutedEventArgs e)
        {
            Task task = Task.Run(() =>
            {
                ClearPythonEnv();
            });
            await task;
        }

        private async void Button_RunScript_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isRunnigScript)
                {
                    MessageBox.Show("The current task is not over");
                    return;
                }
                BtnRunScript.IsEnabled = false;
                do
                {
                    stepBar.StepIndex = 0;
                    isRunnigScript = true;
                    isCancleScript = false;
                    CheckRunScriptPrepare();
                    stepBar.Next();
                    int ret = await ExecutePythonPip();
                    if (0 != ret || isCancleScript) break;
                    stepBar.Next();
                    ret = await CreaetPythonRunScript();
                    if(0 != ret || isCancleScript) break;
                    stepBar.Next();
                    ret = await ExecutePythonScript();
                    if (0 != ret || isCancleScript) break;
                    stepBar.Next();
                    ret = await InnerExecuteDLP(matlabScriptPath);
                    if (0 != ret || isCancleScript) break;
                    stepBar.Next();
                } while (false);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message);
            }
            isRunnigScript = false;
            BtnRunScript.IsEnabled = true;
        }

        static void TerminateChildren(int processId)
        {
            // 获取指定进程的子进程
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                $"Select * From Win32_Process Where ParentProcessID={processId}");
            ManagementObjectCollection moc = searcher.Get();

            // 终止子进程
            foreach (ManagementObject mo in moc)
            {
                int childProcessId = Convert.ToInt32(mo["ProcessID"]);
                TerminateChildren(childProcessId);
            }

            if (processId == Process.GetCurrentProcess().Id)
                return;

            // 终止当前进程
            try
            {
                Process process = Process.GetProcessById(processId);
                process.Kill();
            }
            catch (ArgumentException) { }
        }

        private void Button_StopScript_Click(object sender, RoutedEventArgs e)
        {
            isCancleScript = true;
            Process process = Process.GetCurrentProcess();
            TerminateChildren(process.Id);
            //using (Py.GIL())
            //{
            //    PythonEngine.Interrupt(pythonThreadID);
                
            //}
        }

        private async void Button_Image2Video_Click(object sender, RoutedEventArgs e)
        {
            image2videoBar.IsIndeterminate = true;
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "Select a folder";
            dialog.ShowNewFolderButton = true;
            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string folderName = dialog.SelectedPath;
                Task task = Task.Run(() =>
                {
                    ExecuteDLP(folderName);
                });
                await task;
            }
            image2videoBar.IsIndeterminate = false;

        }
    }
}
