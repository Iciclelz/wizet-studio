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
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Studio.Wizet.Hexview;
using Studio.Wizet.Library.Wz;
using Studio.Wizet.Library.Wz.WzProperties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Wizet_Studio.Studio.Wizet.Windows;
using Wizet_Studio.Studio.Wizet.Windows.Components;
using Wizet_Studio.Studio.Wizet.Windows.Helper;
using Wizet_Studio.Studio.Wizet.Wrapper.Wz;

namespace Wizet_Studio.Studio.Wizet.Wrapper
{
    class MainWindowWrapper
    {
        private List<String> WzList;
        public List<WzFile> wzFilesCollection;

        private MainWindow mainWindow;
        private StatusBarItem versionStatusBarItem;
        private TabControl tabControl;
        private TreeView treeView;
        private Output output;
        private WzFileManager fileManager;
        private StatusBarItem bytesStatusBarItem;
        private StatusBarItem pathStatusBarItem;

        public WzMp3Stream WzAudioStream;
        public WzSoundProperty wzSoundProperty;
        private DispatcherTimer playMusicDispatcherTimer;
        public string TempPath;
        public MainWindowWrapper(MainWindow mainWindow, StatusBarItem versionStatusBarItem, TabControl tabControl, TreeView treeView, StatusBarItem pathStatusBarItem, StatusBarItem bytesStatusBarItem, Output output)
        {
            WzList = new List<String>();
            wzFilesCollection = new List<WzFile>();

            this.mainWindow = mainWindow;
            this.versionStatusBarItem = versionStatusBarItem;
            this.tabControl = tabControl;
            this.treeView = treeView;
            this.pathStatusBarItem = pathStatusBarItem;
            this.bytesStatusBarItem = bytesStatusBarItem;
            this.output = output;
            

            fileManager = new WzFileManager();

            playMusicDispatcherTimer = new DispatcherTimer();
            playMusicDispatcherTimer.Interval = TimeSpan.FromMilliseconds(1);
            playMusicDispatcherTimer.Tick += playMusicDispatcherTimer_Tick;

            TempPath = Path.Combine(Directory.GetCurrentDirectory(), "Temporary Files");
            if (!Directory.Exists(TempPath))
            {
                Directory.CreateDirectory(TempPath);
            }
        }

        /*public async void New()
        {
            string name = await mainWindow.ShowInputAsync("Wizet Studio - Create a New File", "Enter a name for the Wizet File (*.wz): ");
            if (String.IsNullOrEmpty(name))
            {
                return;
            }
            short version = 1;
            if (!Int16.TryParse(await mainWindow.ShowInputAsync("Wizet Studio - Create a New File", "Enter a version for the Wizet File: "), out version))
            {
                version = 1;
            }
            WzFile file = new WzFile(version, WzMapleVersion.BMS) { Name = name };
            file.Header.Copyright = "Copyright Wizet Studio";
            file.Header.RecalculateFileStart();
            file.WzDirectory.Name = name;
            treeView.Items.Add(new WzItem(file));
        }*/

        public async void New()
        {
            string filePath = await mainWindow.ShowInputAsync("Wizet Studio - Create a New File", "Enter a Name for your Wz File: ");
            if (String.IsNullOrEmpty(filePath))
            {
                filePath = Path.GetRandomFileName().Replace(".", "") + ".wz";
            }
            if (!filePath.Contains(".wz"))
            {
                filePath += ".wz";
            }
            new NewWzFile(Path.Combine(TempPath, filePath)).Create();
            Open(Path.Combine(TempPath, filePath));
        }

        public void Open()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Wizet Files (*.wz)|*.wz",
                Multiselect = true
            };
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (String fileName in openFileDialog.FileNames)
                {
                    Open(fileName);
                }
            }
        }

        public async void Open(String filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    await mainWindow.ShowMessageAsync("Wizet Studio", Path.GetFileName(filePath) + " does not exist. You may not open this.");
                    return;
                }
                if (WzList.Contains(filePath))
                {
                    await mainWindow.ShowMessageAsync("Wizet Studio", Path.GetFileName(filePath) + " is already opened. You may not reopen this.");
                    return;
                }

                WzFile file = fileManager.LoadWzFile(filePath, WzMapleVersion.BMS, treeView);
                WzList.Add(filePath);
                wzFilesCollection.Add(file);

                try { versionStatusBarItem.Content = "VERSION " + file.Version; }
                catch { }

                tabControl.SelectedIndex = 0;
                output.WriteLine(Path.GetFileName(filePath) + " has been successfully loaded.");

                
            }
            catch (Exception exception)
            {
                tabControl.SelectedIndex = 0;
                output.WriteLine(exception.Message);
            }
            
        }

        public async void SaveAs()
        {
            
            try {
                WzItem Node = null;
                if (this.treeView.SelectedItem != null)
                {
                    Node = (treeView.SelectedItem as WzItem).Tag is WzFile ? treeView.SelectedItem as WzItem : WzItem.getTopLevelNode(treeView.SelectedItem as WzItem);
                }
                else
                {
                    if (this.treeView.Items.Count != 1) { await mainWindow.ShowMessageAsync("Wizet Studio", "Please select a Wizet file to save."); return; }
                    Node = (WzItem)treeView.Items[0];
                }
                if (Node.Tag is WzFile)
                {
                    WzFile wzFile = Node.Tag as WzFile;
                    if (wzFile.Version < 0)
                    {
                        wzFile.Version = 1;
                    }
                    wzFile.MapleVersion = WzMapleVersion.BMS;
                    SaveFileDialog saveFileDialog = new SaveFileDialog()
                    {
                        Title = "Wizet Studio - Save a Wizet File",
                        Filter = "Wizet Files (*.wz)|*.wz"
                    };
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        if (File.Exists(saveFileDialog.FileName))
                        {
                            wzFile.SaveToDisk(saveFileDialog.FileName + "$tmp");
                            treeView.Items.Remove(Node);
                            File.Delete(saveFileDialog.FileName);
                            wzFilesCollection.Remove(wzFile);
                            WzList.Remove(wzFile.FilePath);
                            File.Move(saveFileDialog.FileName + "$tmp", saveFileDialog.FileName);
                        }
                        else
                        {
                            wzFile.SaveToDisk(saveFileDialog.FileName);
                            treeView.Items.Remove(Node);
                            wzFilesCollection.Remove(wzFile);
                            WzList.Remove(wzFile.FilePath);
                        }
 
                        fileManager.LoadWzFile(saveFileDialog.FileName, WzMapleVersion.BMS, treeView);
                    }
                }

            }
            catch (Exception ex)
            {
                output.WriteLine(ex.Message);
            }
            
        }

        public void PlayButtonClicked()
        {
            if (mainWindow.playMusicButton.Content.ToString().Equals("Play"))
            {
                if (WzAudioStream != null)
                {
                    WzAudioStream.Play();
                }
                else
                {
                    this.WzAudioStream = new WzMp3Stream(wzSoundProperty, (bool)mainWindow.loopCheckBox.IsChecked);
                    mainWindow.slider.Minimum = 0;
                    mainWindow.slider.Maximum = this.WzAudioStream.Length;
                    this.WzAudioStream.Play();
                }
                mainWindow.playMusicButton.Content = "Pause";
                playMusicDispatcherTimer.Start();
            }
            else if (mainWindow.playMusicButton.Content.Equals("Pause"))
            {
                mainWindow.playMusicButton.Content = "Play";
                this.WzAudioStream.Pause();
                playMusicDispatcherTimer.Stop();
            }
        }

        private void playMusicDispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (this.WzAudioStream == null || mainWindow.userIsDraggingSlider)
            {
                return;
            }
            mainWindow.slider.Value = this.WzAudioStream.Position;
            TimeSpan PositionTimeSpan = TimeSpan.FromSeconds((double)this.WzAudioStream.Position);
            TimeSpan LengthTimeSpan = TimeSpan.FromSeconds((double)this.WzAudioStream.Length);
            mainWindow.currentSecondLabel.Content = Convert.ToString(PositionTimeSpan.Minutes).PadLeft(2, '0') + ":" + Convert.ToString(PositionTimeSpan.Seconds).PadLeft(2, '0');
            mainWindow.totalSecondsLabel.Content = Convert.ToString(LengthTimeSpan.Minutes).PadLeft(2, '0') + ":" + Convert.ToString(LengthTimeSpan.Seconds).PadLeft(2, '0');
        }


        public void TreeView_SelectedItemChanged(System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            if (treeView.SelectedItem == null)
            {
                return;
            }
            try
            {
                if (mainWindow.playMusicButton.Content.ToString().Equals("Pause"))
                {
                    PlayButtonClicked();
                }

                mainWindow.currentSecondLabel.Content = "0:00";
                mainWindow.totalSecondsLabel.Content = "0:00";

                IWzObject IWzObject = (IWzObject)((treeView.SelectedItem as TreeViewItem).Tag);
                WzObject.WizetObject wzobject = WzObject.GetWizetObject(IWzObject);
                switch (wzobject)
                {
                    case WzObject.WizetObject.WizetFile:
                    case WzObject.WizetObject.WizetDirectory:
                    case WzObject.WizetObject.WizetImage:
                    case WzObject.WizetObject.WizetNullProperty:
                    case WzObject.WizetObject.WizetSubProperty:
                    case WzObject.WizetObject.WizetConvexProperty:
                        mainWindow.changeTabControlSelectedItem(0);
                        break;

                    case WzObject.WizetObject.WizetCanvasProperty:
                        Bitmap ObjectBitmap = (Bitmap)IWzObject;
                        mainWindow.canvasRectangle.Height = ObjectBitmap.Height;
                        mainWindow.canvasRectangle.Width = ObjectBitmap.Width;
                        using (MemoryStream memory = new MemoryStream())
                        {
                            ObjectBitmap.Save(memory, ImageFormat.Png);
                            memory.Position = 0;
                            BitmapImage bitmapImage = new BitmapImage();
                            bitmapImage.BeginInit();
                            bitmapImage.StreamSource = memory;
                            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                            bitmapImage.EndInit();

                            mainWindow.canvasRectangle.Fill = new ImageBrush() { ImageSource = bitmapImage };
                        }

                        mainWindow.changeTabControlSelectedItem(1);

                        break;
                    case WzObject.WizetObject.WizetSoundProperty:
                        mainWindow.loopCheckBox.IsChecked = false;
                        WzAudioStream = null;
                        wzSoundProperty = (WzSoundProperty)IWzObject;

                        mainWindow.changeTabControlSelectedItem(2);
                        break;
                    case WzObject.WizetObject.WizetCompressedIntProperty:
                    case WzObject.WizetObject.WizetDoubleProperty:
                    case WzObject.WizetObject.WizetByteFloatProperty:
                    case WzObject.WizetObject.WizetUnsignedShortProperty:
                    case WzObject.WizetObject.WizetUolProperty:
                    case WzObject.WizetObject.WizetStringProperty:
                        mainWindow.valueTextBox.Text = IWzObject.ToString();

                        mainWindow.changeTabControlSelectedItem(3);

                        break;
                    case WzObject.WizetObject.WizetVectorProperty:
                        mainWindow.xNumericUpDown.Value = Convert.ToDouble(((WzVectorProperty)IWzObject).X.Value);
                        mainWindow.yNumericUpDown.Value = Convert.ToDouble(((WzVectorProperty)IWzObject).Y.Value);

                        mainWindow.changeTabControlSelectedItem(4);
                        break;
                    default:
                        return;
                }
            }
            catch
            {

            }

            try
            {
                mainWindow.selectionStatusBarItem.Content = "Selection: " + ((WzItem)treeView.SelectedItem).GetTypeName();
                mainWindow.textBox.Text = (treeView.SelectedItem as TreeViewItem).Header.ToString();
                mainWindow.versionStatusBarItem.Content = "VERSION " + ((WzItem.getTopLevelNode(treeView.SelectedItem as WzItem)).Tag as WzFile).Version;

            }
            catch { }
            try {
                TreeView_UpdatePathAndBytes(treeView.SelectedItem as TreeViewItem);
            }
            catch { }

        }

        private TreeViewItem TreeView_GetParent(TreeViewItem treeViewItem)
        {
            if (treeViewItem.Parent != null)
            {
                return TreeView_GetParent(treeViewItem.Parent as TreeViewItem);
            }
            return treeViewItem;
        }

        private void TreeView_UpdatePathAndBytes(TreeViewItem treeViewItem)
        {
            try
            {
                pathStatusBarItem.Content = (treeViewItem as WzItem).PathTag;
                bytesStatusBarItem.Content = BytesManager.GetDisplayBytes(new FileInfo((treeViewItem as WzItem).PathTag.ToString()).Length);
                mainWindow.byteEditor.ByteProvider = new DynamicFileByteProvider(pathStatusBarItem.Content.ToString(), true);
                mainWindow.byteEditor.Refresh();
            }
            catch //(Exception e)
            {
                //output.WriteLine(e.ToString());
                try
                {
                    if (treeViewItem.Parent != null)
                    {
                        TreeView_UpdatePathAndBytes(treeViewItem.Parent as TreeViewItem);
                    }
                }
                catch
                {

                }
            }
        }

        public void TreeView_MouseDoubleClick()
        {
            try
            {
                if (treeView.SelectedItem != null && (treeView.SelectedItem as TreeViewItem).Tag is WzImage)
                {
                    if (!((WzImage)(treeView.SelectedItem as TreeViewItem).Tag).Parsed)
                    {
                        ((WzImage)(treeView.SelectedItem as TreeViewItem).Tag).ParseImage();
                    }
                    ((WzItem)(treeView.SelectedItem as TreeViewItem)).Reparse();
                    (treeView.SelectedItem as WzItem).IsExpanded = true;
                }

                
            }
            catch (Exception exception)
            {
                tabControl.SelectedItem = 1;
                output.WriteLine(exception.Message);
            }
        }
    }
}
