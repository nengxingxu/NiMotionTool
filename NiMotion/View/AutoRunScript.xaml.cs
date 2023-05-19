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



namespace NiMotion.View
{
    /// <summary>
    /// RunMatlab.xaml 的交互逻辑
    /// </summary>
    public partial class AutoRunScript
    {
        private string pyScriptPath = AppDomain.CurrentDomain.BaseDirectory + "runmat.py";
        private List<string> matList = new List<string>();
        private CancellationTokenSource source = new CancellationTokenSource();
        private ListViewModel context = new ListViewModel();
        IntPtr threadState = IntPtr.Zero;
        ulong pythonThreadID;

        public AutoRunScript()
        {
            InitializeComponent();
            PythonRuntimeInit();
            DataContext = context;
            listView.ItemsSource = context.DataList;
            //CreaetPythonRunScript();
        }

        private void PythonRuntimeInit()
        {
            Runtime.PythonDLL = @"C:\Program Files\Python39\python39.dll";
            PythonEngine.Initialize();
            threadState = PythonEngine.BeginAllowThreads();

            //PythonEngine.EndAllowThreads(threadState);
            //PythonEngine.Shutdown();
        }

        private void CreaetPythonRunScript()
        {
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
                for (int i=0; i < context.GetListDataItemCount(); i++)
                {
                    var item = context.GetListItem(i);
                    string matscript_path = item.Path;
                    string path = System.IO.Path.GetDirectoryName(matscript_path);
                    path = path.Replace('\\', '/');
                    string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(matscript_path);

                    writer.WriteLine($"    eng.cd('{path}')");
                    writer.WriteLine($"    eng.{fileNameWithoutExtension}(nargout=0)");
                }
                writer.WriteLine("    eng.quit()");
                writer.WriteLine("\n\n\n");
                writer.WriteLine("run_mat()");
            }
        }


        public async Task ExecutePythonScript()
        {
            await Task.Run(() =>
            {
                using (Py.GIL())
                {
                    pythonThreadID = PythonEngine.GetPythonThreadID();
                    PyModule scope = Py.CreateScope();
                    try
                    {
                        string code = File.ReadAllText(pyScriptPath); // Get the python file as raw text
                        var scriptCompiled = PythonEngine.Compile(code, pyScriptPath); // Compile the code/file
                        PyObject py_object = scope.Execute(scriptCompiled); // Execute the compiled python
                        MessageBox.Show("end");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("end");
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        MessageBox.Show("end");
                        scope.Dispose();
                    }
                }
            });
        }

        private void ExecuteFFmpeg(string inputPath)
        {
            string ffmpegPath = AppDomain.CurrentDomain.BaseDirectory + "ffmpeg.exe";
            string inputFilePath = inputPath + "\\David_step-%05d.tiff";
            string outputFilePath = AppDomain.CurrentDomain.BaseDirectory + "ouput.mp4";

            string ffmpegArgs = $"-framerate 30 -i \"{inputFilePath}\" -c:v libx264 -r 30 -pix_fmt yuv420p \"{outputFilePath}\"";
            System.Diagnostics.Process process = new System.Diagnostics.Process();
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
                    MessageBox.Show("FFmpeg 执行成功！");
                }
                else
                {
                    MessageBox.Show("FFmpeg 执行失败 :" + error_output);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void Button_AddScript_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "matlab script file (*.mat)|*.m"; // 可以根据需要设置文件类型过滤器

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                context.AddListDataItem(System.IO.Path.GetFileName(selectedFilePath), selectedFilePath);
            }
        }

        private void Button_DeleteScript_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_ClearScriptList_Click(object sender, RoutedEventArgs e)
        {
            context.ClearListData();
        }

        private async void Button_RunScript_Click(object sender, RoutedEventArgs e)
        {
            CreaetPythonRunScript();
            await ExecutePythonScript();
        }

        private void Button_StopScript_Click(object sender, RoutedEventArgs e)
        {
            using (Py.GIL())
            {
                int interruptReturnValue = PythonEngine.Interrupt(pythonThreadID);
                MessageBox.Show(interruptReturnValue.ToString());
            }
        }

        private void Button_Image2Video_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "Select a folder";
            dialog.ShowNewFolderButton = true;
   
            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string folderName = dialog.SelectedPath;
                ExecuteFFmpeg(folderName);
            }
            
        }
    }
}
