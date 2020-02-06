using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Program
{
    public class RunProcess
    {
        public event EventHandler<string> ProcessExitedHandler;
        public event EventHandler<string> OnDataReceivedHandler;
        public event EventHandler<string> onErrorReceivedHandler;

        public bool HasExited => _process.HasExited;

        private Process _process;

        public RunProcess()
        {
            _process = new Process();
        }

        ///<summary>
        /// 执行外部程序
        /// </summary>
        /// <param name="filePath">程序路径</param>
        /// <param name="arguments">参数</param>
        public void RunExe(string filePath, string arguments)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath), "外部程序路径为空");
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"文件 [{filePath}] 不存在", filePath);
            }

            try
            {
                _process.StartInfo.FileName = filePath;
                _process.StartInfo.Arguments = arguments;
                _process.StartInfo.WorkingDirectory = Path.GetDirectoryName(filePath);
                _process.StartInfo.ErrorDialog = false;
                _process.StartInfo.UseShellExecute = false;
                _process.StartInfo.CreateNoWindow = true;
                _process.EnableRaisingEvents = true;
                _process.StartInfo.RedirectStandardOutput = true;
                _process.StartInfo.RedirectStandardInput = true;
                _process.StartInfo.RedirectStandardError = true;
                _process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                _process.StartInfo.StandardErrorEncoding = Encoding.UTF8;

                _process.Exited += (s, e) => ProcessExitedHandler?.Invoke(this, "进程已退出");
                _process.OutputDataReceived += ProcessOnOutputDataReceived;
                _process.ErrorDataReceived += ProcessOnErrorDataReceived;

                _process.Start();
                _process.BeginErrorReadLine();
                _process.BeginOutputReadLine();
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                onErrorReceivedHandler?.Invoke(this, e.Data);
            }
        }

        private void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                OnDataReceivedHandler?.Invoke(this, e.Data);
            }
        }

        public void WriteLine(string input)
        {
            try
            {
                _process.StandardInput.WriteLine(input);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
