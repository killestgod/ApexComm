using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MyHelper;

namespace ApexComm
{
    /// <summary>
        /// App.xaml 的交互逻辑
        /// </summary>
    public partial class App : Application
    {
        private static bool _isFatalWindowShow = false;

        public App()
        {
            this.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                if (e.ExceptionObject is System.Exception)
                {
                    HandleException((System.Exception)e.ExceptionObject);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        public static void HandleException(Exception ex)
        {
            Console.WriteLine(ex.Message.ToString());

            ////记录日志
            //if (!System.IO.Directory.Exists("Log"))
            //{
            //    System.IO.Directory.CreateDirectory("Log");
            //}
            //DateTime now = DateTime.Now;
            //string logpath = string.Format(@"Log\fatal_{0}{1}{2}.log", now.Year, now.Month, now.Day);
            //System.IO.File.AppendAllText(logpath, string.Format("\r\n----------------------{0}--------------------------\r\n", now.ToString("yyyy-MM-dd HH:mm:ss")));
            //System.IO.File.AppendAllText(logpath, ex.Message);
            //System.IO.File.AppendAllText(logpath, "\r\n");
            //System.IO.File.AppendAllText(logpath, ex.StackTrace);
            //System.IO.File.AppendAllText(logpath, "\r\n");
            //System.IO.File.AppendAllText(logpath, "\r\n----------------------footer--------------------------\r\n");
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                HandleException(e.Exception);
                e.Handled = true;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }
    }
}