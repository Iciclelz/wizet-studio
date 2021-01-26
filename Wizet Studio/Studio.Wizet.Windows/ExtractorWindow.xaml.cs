/*
    Copyright (C) 2016 Ryukuo

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
using MahApps.Metro.Controls;
using Microsoft.Win32;
using Studio.Wizet.Library.Wz;
using Studio.Wizet.Library.Wz.Serialization;
using System.IO;
using System.Threading;
using System.Windows;
using Wizet_Studio.Studio.Wizet.Windows.Components;

namespace Wizet_Studio.Studio.Wizet.Windows
{
    /// <summary>
    /// Interaction logic for ExtractorWindow.xaml
    /// </summary>
    public partial class ExtractorWindow : MetroWindow
    {
        private bool IsExporting = false;
        public ExtractorWindow()
        {
            InitializeComponent();
        }

        private void wizetFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Wizet Studio - Select A Wizet File",
                Filter = "Wizet File (*.wz)|*.wz"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                wizetFilePathTextBox.Text = openFileDialog.FileName;
            }
        }

        private void outputDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog openDirectoryDialog = new OpenFolderDialog()
            {
                Title = "Wizet Studio - Select a Directory to Export the Wizet File To"
            };
            if (openDirectoryDialog.ShowDialog())
            {
                outputDirectoryPathTextBox.Text = openDirectoryDialog.FileName;
            }
        }


        private delegate void ExtractionComplete();
        private void ExtractionCallback()
        {
            button.IsEnabled = true;
            progressRing.IsActive = false;
            IsExporting = false;
        }

        private void RunWzFilesExtraction(object param)
        {
            IWzFileSerializer serializer = (IWzFileSerializer)((object[])param)[3];
            WzFile f = new WzFile((string)((object[])param)[0], (WzMapleVersion)((object[])param)[2]);
            f.ParseWzFile();
            serializer.SerializeFile(f, Path.Combine((string)((object[])param)[1], Path.GetFileName(f.Name)));
            f.Dispose();

            MessageBox.Show("The XML extraction task has been completed.", "Wizet Studio", MessageBoxButton.OK, MessageBoxImage.Information);

            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new ExtractionComplete(ExtractionCallback));
            }
            else
            {
                ExtractionCallback();
            }
        }


        private void button_Click(object sender, RoutedEventArgs e)
        {
            if ((wizetFilePathTextBox.Text != "") && (outputDirectoryPathTextBox.Text != ""))
            {
                button.IsEnabled = false;
                IsExporting = true;

                if (dataRadioButton.IsChecked == true)
                {
                    new Thread(new ParameterizedThreadStart(RunWzFilesExtraction)).Start((object)new object[] { wizetFilePathTextBox.Text, outputDirectoryPathTextBox.Text, WzMapleVersion.CLASSIC, new WzClassicXmlSerializer(0, LineBreak.None, false) });
                    progressRing.IsActive = true;
                }
                else if (resourceRadioButton.IsChecked == true)
                {
 
                    new Thread(new ParameterizedThreadStart(RunWzFilesExtraction)).Start((object)new object[] { wizetFilePathTextBox.Text, outputDirectoryPathTextBox.Text, WzMapleVersion.CLASSIC, new WzPngMp3Serializer() });
                    progressRing.IsActive = true;
                }
                else if (imageRadioButton.IsChecked == true)
                {
                    new Thread(new ParameterizedThreadStart(RunWzFilesExtraction)).Start((object)new object[] { wizetFilePathTextBox.Text, outputDirectoryPathTextBox.Text, WzMapleVersion.CLASSIC, new WzImgSerializer() });
                    progressRing.IsActive = true;
                }
            }
        }

        private void extractorWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsExporting)
            {
                e.Cancel = true;
            }
        }
    }
}
