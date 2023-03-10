using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Microsoft.VisualBasic.FileIO;
using System.Threading.Tasks;

namespace MainProgram.Windows
{
    /// <summary>
    /// ClassifyWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PPTClassifier : Window
    {
        DateTime LastOpen;
        bool DoubleClicked = false;
        public PPTClassifier()
        {
            InitializeComponent();
            Left = SystemParameters.PrimaryScreenWidth - Width;
            Top = (SystemParameters.PrimaryScreenHeight - Height) / 5 * 2;
        }

        private void SelectFiles(string subject)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择课件";
            openFileDialog.Filter = "课件|*.*";
            openFileDialog.FileName = string.Empty;
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = true;
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (Directory.Exists("E:\\"))
            {
                openFileDialog.InitialDirectory = "E:\\";
            }
            if (openFileDialog.ShowDialog() == false)
            {
                return;
            }
            string[] files = openFileDialog.FileNames;
            CopyFiles(subject, files);
        }

        private void GetDroppedFiles(string subject, DragEventArgs e)
        {
            try
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                CopyFiles(subject, files);
            }
            catch { }
        }

        private void CopyFiles(string subject, string[] files)
        {
            string folder = $"D:/{subject}/";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            foreach (string file in files)
            {
                string destination = folder + Path.GetFileName(file);
                if (destination == file)
                {
                    continue;
                }
                else if (File.Exists(destination))
                {
                    FileSystem.DeleteFile(destination, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                }
                File.Copy(file, destination);
                if (Directory.GetParent(file).FullName ==
                    Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory))
                {
                    File.Delete(file);
                }
            }
        }

        private void DropButton_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SelectFiles(((Button)sender).Content.ToString());
        }

        private void DropButton_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
                GetDroppedFiles(((Button)sender).Content.ToString(), e);
            }
        }

        private void Window_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            OverTop.ExtentedWindowOps.ToBottom(this);
        }

        private async void DropButton_Click(object sender, RoutedEventArgs e)
        {
            WaitProgressRing.IsActive = true;
            WaitProgressRing.Visibility = Visibility.Visible;
            await Task.Delay(500);
            if (DoubleClicked)
            {
                DoubleClicked = false;
                return;
            }

            WaitProgressRing.IsActive = false;
            WaitProgressRing.Visibility = Visibility.Collapsed;
            if (DateTime.Now - LastOpen <= TimeSpan.FromMilliseconds(1500))
            {
                return;
            }
            Process.Start("explorer.exe", $"D:\\{((Button)sender).Content}");
            LastOpen = DateTime.Now;
        }

        private void DropButton_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DoubleClicked = true;
            WaitProgressRing.IsActive = false;
            WaitProgressRing.Visibility = Visibility.Collapsed;
            SelectFiles(((Button)sender).Content.ToString());
        }
    }
}
