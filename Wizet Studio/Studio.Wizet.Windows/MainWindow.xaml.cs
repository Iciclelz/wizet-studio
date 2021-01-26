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
using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Studio.Wizet.Hexview;
using Studio.Wizet.Library.Wz;
using Studio.Wizet.Library.Wz.Serialization;
using Studio.Wizet.Library.Wz.Util;
using Studio.Wizet.Library.Wz.WzProperties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Wizet_Studio.Studio.Wizet.Windows.Components;
using Wizet_Studio.Studio.Wizet.Windows.Helper;
using Wizet_Studio.Studio.Wizet.Wrapper;
using Wizet_Studio.Studio.Wizet.Wrapper.Wz;
using Wizet_Studio.Studio.Wizet.Wrapper.Wz.Foothold;

namespace Wizet_Studio.Studio.Wizet.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private MainWindowWrapper mainWindowWrapper;
        private WzItemEdit wzEditManager;
        private Output output;
        private bool systemChangeTab;
        private int tabIndex;
        public bool userIsDraggingSlider;
        private IniReader iniReader;
        private IniWriter iniWriter;
        private string iniPath;

        public ByteEditor byteEditor;
        public MainWindow()
        {
            InitializeComponent();
            output = new Output(consoleTextBox);
            systemChangeTab = false;
            userIsDraggingSlider = false;
            wzEditManager = new WzItemEdit(treeView);
            mainWindowWrapper = new MainWindowWrapper(this, versionStatusBarItem, tabControl, treeView, pathStatusBarItem, bytesStatusBarItem, output);

            iniPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Wizet Studio", "Settings.ini");
            if (!Directory.Exists(Path.GetDirectoryName(iniPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(iniPath));
            }

            iniReader = new IniReader(iniPath);
            iniWriter = new IniWriter(iniPath);

            byteEditor = new ByteEditor()
            {
                LineInfoVisible = true,
                StringViewVisible = true,
                VScrollBarVisible = true
            };
            hexviewHost.Child = byteEditor;

            byteEditor.ForeColor = Color.White;
            byteEditor.BackColor = ColorTranslator.FromHtml("#2D2D2D");

            
        }

        public MainWindow(String[] args) : this()
        {
            if (args.Length > 0)
            {
                foreach (string fileName in args)
                {
                    mainWindowWrapper.Open(fileName);
                }
            }
        }

        private void newFileMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            mainWindowWrapper.New();
        }

        private void openFileMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            mainWindowWrapper.Open();
        }

        private void saveAsFileMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            mainWindowWrapper.SaveAs();
        }

        private void treeView_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            mainWindowWrapper.TreeView_SelectedItemChanged(e); 
        }


        private void openAndExpandWzImageItem_Click(object sender, RoutedEventArgs e)
        {
            mainWindowWrapper.TreeView_MouseDoubleClick();
        }

        private void treeView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            mainWindowWrapper.TreeView_MouseDoubleClick();
        }

        private void tabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.OriginalSource == tabControl)
            {
                if (systemChangeTab)
                {
                    tabIndex = tabControl.SelectedIndex;
                }
                else if (tabControl.SelectedIndex != tabIndex)
                {
                    e.Handled = true;
                    tabControl.SelectedIndex = tabIndex;
                }

            }
        }

        public void changeTabControlSelectedItem(int index)
        {
            systemChangeTab = true;
            tabControl.SelectedIndex = index;
            systemChangeTab = false;
        }

        private void newWindowMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new MainWindow().Show();
        }

        private void closeWindowMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Close();
        }

        private void exitMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void playMusicButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            mainWindowWrapper.PlayButtonClicked();
        }

        private void slider_DragStarted(object sender, DragStartedEventArgs e)
        {
            userIsDraggingSlider = true;
        }

        private void slider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (mainWindowWrapper.WzAudioStream != null)
            {
                if (userIsDraggingSlider)
                {
                    mainWindowWrapper.WzAudioStream.Position = (int)slider.Value;
                }
            }
            userIsDraggingSlider = false;
        }

        private void loopCheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                mainWindowWrapper.WzAudioStream.Repeat = true;
            }
            catch
            {
                loopCheckBox.IsChecked = false;
            }
        }

        private void loopCheckBox_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                mainWindowWrapper.WzAudioStream.Repeat = false;
            }
            catch
            {
                loopCheckBox.IsChecked = false;
            }
        }

        private async void newWzDirectoryMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!((treeView.SelectedItem as TreeViewItem).Tag is WzDirectory) && !((treeView.SelectedItem as TreeViewItem).Tag is WzFile))
            {
                await this.ShowMessageAsync("Wizet Studio", "Error: Unable to insert the specified object into this type of Wizet Node.");
                return;
            }
            string name = await mainWindow.ShowInputAsync("Wizet Studio", "Enter a Directory Name: ");
            if (!String.IsNullOrEmpty(name))
            {
                ((WzItem)treeView.SelectedItem).AddObject(new WzDirectory(name), wzEditManager);
            }
            
        }

        private async void newWzImageMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!((treeView.SelectedItem as TreeViewItem).Tag is WzDirectory) && !((treeView.SelectedItem as TreeViewItem).Tag is WzFile))
            {
                await this.ShowMessageAsync("Wizet Studio", "Error: Unable to insert the specified object into this type of Wizet Node.");
                return;
            }
            string name = await mainWindow.ShowInputAsync("Wizet Studio", "Enter a Image Name: ");
            if (!String.IsNullOrEmpty(name))
            {
                ((WzItem)treeView.SelectedItem).AddObject(new WzImage(name) { Changed = true }, wzEditManager);
            }           
        }

        private async void newWzByteFloatMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!((treeView.SelectedItem as TreeViewItem).Tag is IPropertyContainer))
            {
                await this.ShowMessageAsync("Wizet Studio", "Error: Unable to insert the specified object into this type of Wizet Node.");
                return;
            }
            string name = await mainWindow.ShowInputAsync("Wizet Studio", "Enter a Name for the Byte Float: ");
            if (String.IsNullOrEmpty(name))
            {
                return;
            }
            try
            {
                ((WzItem)(treeView.SelectedItem as TreeViewItem)).AddObject(new WzByteFloatProperty(name, Convert.ToSingle(Double.Parse(await mainWindow.ShowInputAsync("Wizet Studio", "Enter a Value for the BytesFloat: ")))), wzEditManager);
            }
            catch (Exception exception)
            {
                tabControl.SelectedIndex = 0;
                output.WriteLine(exception.Message);
            }
        }

        private async void newWzCanvasMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!((treeView.SelectedItem as TreeViewItem).Tag is IPropertyContainer))
            {
                await this.ShowMessageAsync("Wizet Studio", "Error: Unable to insert the specified object into this type of Wizet Node.");
                return;
            }
            string name = await mainWindow.ShowInputAsync("Wizet Studio", "Enter a Name for the Canvas: ");
            if (String.IsNullOrEmpty(name))
            {
                return;
            }
            try
            {
                Bitmap bmp = (Bitmap)System.Drawing.Image.FromFile(await mainWindow.ShowInputAsync("Wizet Studio", "Enter the Canvas Path: "));
                ((WzItem)(treeView.SelectedItem as TreeViewItem)).AddObject(new WzCanvasProperty(name) { PngProperty = new WzPngProperty() { PNG = bmp } }, wzEditManager);
            }
            catch (Exception exception)
            {
                tabControl.SelectedIndex = 0;
                output.WriteLine(exception.Message);
            }
        }

        private async void newWzCompressedIntMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!((treeView.SelectedItem as TreeViewItem).Tag is IPropertyContainer))
            {
                await this.ShowMessageAsync("Wizet Studio", "Error: Unable to insert the specified object into this type of Wizet Node.");
                return;
            }
            string name = await mainWindow.ShowInputAsync("Wizet Studio", "Enter a Name for the CompressedInt: ");
            if (String.IsNullOrEmpty(name))
            {
                return;
            }
            try
            {
                ((WzItem)(treeView.SelectedItem as TreeViewItem)).AddObject(new WzCompressedIntProperty(name, Convert.ToInt32(await mainWindow.ShowInputAsync("Wizet Studio", "Enter a Value for the CompressedInt: "))), wzEditManager);
            }
            catch (Exception exception)
            {
                tabControl.SelectedIndex = 0;
                output.WriteLine(exception.Message);
            }
        }

        private async void newWzConvexMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!((treeView.SelectedItem as TreeViewItem).Tag is IPropertyContainer))
            {
                await this.ShowMessageAsync("Wizet Studio", "Error: Unable to insert the specified object into this type of Wizet Node.");
                return;
            }
            string name = await mainWindow.ShowInputAsync("Wizet Studio", "Enter a Name for the Convex: ");
            if (String.IsNullOrEmpty(name))
            {
                return;
            }
            ((WzItem)(treeView.SelectedItem as TreeViewItem)).AddObject(new WzConvexProperty(name), wzEditManager);
        }

        private async void newWzDoubleMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!((treeView.SelectedItem as TreeViewItem).Tag is IPropertyContainer))
            {
                await this.ShowMessageAsync("Wizet Studio", "Error: Unable to insert the specified object into this type of Wizet Node.");
                return;
            }
            string name = await mainWindow.ShowInputAsync("Wizet Studio", "Enter a Name for the Double: ");
            if (String.IsNullOrEmpty(name))
            {
                return;
            }
            try
            {
                ((WzItem)(treeView.SelectedItem as TreeViewItem)).AddObject(new WzDoubleProperty(name, Double.Parse(await mainWindow.ShowInputAsync("Wizet Studio", "Enter a Value for the Double: "))), wzEditManager);
            }
            catch (Exception exception)
            {
                tabControl.SelectedIndex = 0;
                output.WriteLine(exception.Message);
            }
        }

        private async void newWzNullMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!((treeView.SelectedItem as TreeViewItem).Tag is IPropertyContainer))
            {
                await this.ShowMessageAsync("Wizet Studio", "Error: Unable to insert the specified object into this type of Wizet Node.");
                return;
            }
            string name = await mainWindow.ShowInputAsync("Wizet Studio", "Enter a Name for the Null: ");
            if (String.IsNullOrEmpty(name))
            {
                return;
            }
            ((WzItem)(treeView.SelectedItem as TreeViewItem)).AddObject(new WzNullProperty(name), wzEditManager);
        }

        private async void newWzSoundMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!((treeView.SelectedItem as TreeViewItem).Tag is IPropertyContainer))
            {
                await this.ShowMessageAsync("Wizet Studio", "Error: Unable to insert the specified object into this type of Wizet Node.");
                return;
            }
            string name = await mainWindow.ShowInputAsync("Wizet Studio", "Enter a Name for the Sound: ");
            string path = await mainWindow.ShowInputAsync("Wizet Studio", "Enter the Path for the Sound: ");
            if (String.IsNullOrEmpty(name) || !File.Exists(path))
            {
                return;
            }
            try
            {
                ((WzItem)(treeView.SelectedItem as TreeViewItem)).AddObject(new WzSoundProperty(name, path), wzEditManager);
            }
            catch (Exception exception)
            {
                tabControl.SelectedIndex = 0;
                output.WriteLine(exception.Message);
            }
        }

        private async void newWzStringMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!((treeView.SelectedItem as TreeViewItem).Tag is IPropertyContainer))
            {
                await this.ShowMessageAsync("Wizet Studio", "Error: Unable to insert the specified object into this type of Wizet Node.");
                return;
            }
            string name = await mainWindow.ShowInputAsync("Wizet Studio", "Enter a Name for the String: ");
            string str = await mainWindow.ShowInputAsync("Wizet Studio", "Enter a Value for the String: ");
            if (String.IsNullOrEmpty(name) || str == "")
            {
                return;
            }
            try
            {
                ((WzItem)(treeView.SelectedItem as TreeViewItem)).AddObject(new WzStringProperty(name, str), wzEditManager);
            }
            catch (Exception exception)
            {
                tabControl.SelectedIndex = 0;
                output.WriteLine(exception.Message);
            }
        }

        private async void newWzSubMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!((treeView.SelectedItem as TreeViewItem).Tag is IPropertyContainer))
            {
                await this.ShowMessageAsync("Wizet Studio", "Error: Unable to insert the specified object into this type of Wizet Node.");
                return;
            }
            string name = await mainWindow.ShowInputAsync("Wizet Studio", "Enter a Name for the Sub: ");
            if (String.IsNullOrEmpty(name))
            {
                return;
            }
            ((WzItem)(treeView.SelectedItem as TreeViewItem)).AddObject(new WzSubProperty(name), wzEditManager);
        }

        private async void newWzUnsignedShortMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!((treeView.SelectedItem as TreeViewItem).Tag is IPropertyContainer))
            {
                await this.ShowMessageAsync("Wizet Studio", "Error: Unable to insert the specified object into this type of Wizet Node.");
                return;
            }
            string name = await mainWindow.ShowInputAsync("Wizet Studio", "Enter a Name for the Unsigned Short: ");
            if (String.IsNullOrEmpty(name))
            {
                return;
            }
            try
            {
                ((WzItem)(treeView.SelectedItem as TreeViewItem)).AddObject(new WzUnsignedShortProperty(name, UInt16.Parse(await mainWindow.ShowInputAsync("Wizet Studio", "Enter a Value for the Unsigned Short: "))), wzEditManager);
            }
            catch (Exception exception)
            {
                tabControl.SelectedIndex = 0;
                output.WriteLine(exception.Message);
            }
        }

        private async void newWzUolMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!((treeView.SelectedItem as TreeViewItem).Tag is IPropertyContainer))
            {
                await this.ShowMessageAsync("Wizet Studio", "Error: Unable to insert the specified object into this type of Wizet Node.");
                return;
            }
            string name = await mainWindow.ShowInputAsync("Wizet Studio", "Enter a Name for the Uol: ");
            string str = await mainWindow.ShowInputAsync("Wizet Studio", "Enter a Value for the Uol: ");
            if (String.IsNullOrEmpty(name) || str == "")
            {
                return;
            }
            try
            {
                ((WzItem)(treeView.SelectedItem as TreeViewItem)).AddObject(new WzUOLProperty(name, str), wzEditManager);
            }
            catch (Exception exception)
            {
                tabControl.SelectedIndex = 0;
                output.WriteLine(exception.Message);
            }
        }

        private async void newWzVectorMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!((treeView.SelectedItem as TreeViewItem).Tag is IPropertyContainer))
            {
                await this.ShowMessageAsync("Wizet Studio", "Error: Unable to insert the specified object into this type of Wizet Node.");
                return;
            }
            string name = await mainWindow.ShowInputAsync("Wizet Studio", "Enter a Name for the Vector: ");
            string str = await mainWindow.ShowInputAsync("Wizet Studio", "Enter the Vector Points (0,0): ");
            if (String.IsNullOrEmpty(name) || str == "")
            {
                return;
            }
            try
            {
                string[] points = str.Split(',');
                System.Drawing.Point point = new System.Drawing.Point(Int32.Parse(points[0]), Int32.Parse(points[1]));
                ((WzItem)(treeView.SelectedItem as TreeViewItem)).AddObject(new WzVectorProperty(name, new WzCompressedIntProperty("X", point.X), new WzCompressedIntProperty("Y", point.Y)), wzEditManager);
            }
            catch (Exception exception)
            {
                tabControl.SelectedIndex = 0;
                output.WriteLine(exception.Message);
            }
        }

        private async void removeMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (treeView.SelectedItem != null)
            {
                if (await this.ShowMessageAsync("Wizet Studio", "Are you sure you would like to delete " + textBox.Text + "?", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings() { AffirmativeButtonText = "Yes", NegativeButtonText = "No", ColorScheme = MetroDialogColorScheme.Theme }) == MessageDialogResult.Affirmative)
                {
                    List<WzItemEditAction> actions = new List<WzItemEditAction>();

                    if (!((treeView.SelectedItem as WzItem).Tag is WzFile) && (treeView.SelectedItem as WzItem).Parent != null)
                    {
                        actions.Add(WzItemEditAction.ObjectRemoved((WzItem)(((WzItem)treeView.SelectedItem).Parent), (WzItem)treeView.SelectedItem));
                        ((WzItem)treeView.SelectedItem).Delete((treeView.SelectedItem as WzItem).Parent as TreeViewItem);
                    }
                    wzEditManager.AddUndoBatch(actions);
                }
            }
            
        }

        private void deleteAllMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try {
                TreeViewItem parent = (treeView.SelectedItem as TreeViewItem).Parent as TreeViewItem;
                (parent.Parent as TreeViewItem).Items.Remove(parent);
            }
            catch
            {
                try
                {
                    treeView.Items.Remove((treeView.SelectedItem as TreeViewItem).Parent as TreeViewItem);
                }
                catch
                {

                }
            }
        }

        private void ExpandAllTreeViewItems(TreeViewItem treeViewItem, bool IsExpanded)
        {
            treeViewItem.IsExpanded = IsExpanded;
            foreach (TreeViewItem tvi in treeViewItem.Items)
            {
                ExpandAllTreeViewItems(tvi, IsExpanded);
            }
        }

        private void expandAllMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (TreeViewItem tvi in treeView.Items)
            {
                ExpandAllTreeViewItems(tvi, true);
            }
        }

        private void collapseAllMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (TreeViewItem tvi in treeView.Items)
            {
                ExpandAllTreeViewItems(tvi, false);
            }
        }

        private async void button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try {
                if (treeView.SelectedItem != null)
                {
                    if ((treeView.SelectedItem as WzItem).Tag is WzFile)
                    {
                        ((treeView.SelectedItem as WzItem).Tag as WzFile).Header.Copyright = textBox.Text;
                        ((treeView.SelectedItem as WzItem).Tag as WzFile).Header.RecalculateFileStart();
                        (treeView.SelectedItem as WzItem).Header = textBox.Text;
                    }
                    else if (WzItem.CanNodeBeInserted(((treeView.SelectedItem as TreeViewItem).Parent as WzItem), textBox.Text))
                    {
                        (treeView.SelectedItem as WzItem).ChangeName(textBox.Text);
                        button.IsEnabled = false;
                    }
                    else
                    {
                        await this.ShowMessageAsync("Wizet Studio", "Error: A WzNode with the specified name already exists.");
                    }
                }
            }
            catch (Exception ex)
            {
                output.WriteLine(ex.Message);
            }
            

        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (textBox.Text != (treeView.SelectedItem as TreeViewItem).Header.ToString())
                {
                    button.IsEnabled = true;
                    return;
                }
                button.IsEnabled = false;
            }
            catch
            {

            }
        }

        private void changeImageButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if ((treeView.SelectedItem as TreeViewItem).Tag is WzCanvasProperty)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    Title = "Wizet Studio - Import an Image",
                    Filter = "Supported Image Formats (*.png;*.bmp;*.jpg;*.gif;*.jpeg;*.tif;*.tiff)|*.png;*.bmp;*.jpg;*.gif;*.jpeg;*.tif;*.tiff"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    Bitmap bitmap = (Bitmap)System.Drawing.Image.FromFile(openFileDialog.FileName);
                    ((WzCanvasProperty)(treeView.SelectedItem as TreeViewItem).Tag).PngProperty.SetPNG(bitmap);
                    ((WzCanvasProperty)(treeView.SelectedItem as TreeViewItem).Tag).ParentImage.Changed = true;
                    canvasRectangle.Height = bitmap.Height;
                    canvasRectangle.Width = bitmap.Width;
                    canvasRectangle.Fill = Components.ImageConverter.BitmapToImageBrush(bitmap);
                }
            }
        }

        private void exportImageButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if ((treeView.SelectedItem as TreeViewItem).Tag is WzCanvasProperty)
            {
                Bitmap bitmap = ((WzCanvasProperty)(treeView.SelectedItem as TreeViewItem).Tag).PngProperty.GetPNG(false);

                SaveFileDialog saveFileDialog = new SaveFileDialog()
                {
                    Title = "Wizet Studio - Export WzCanvas as Image",
                    Filter = "Portable Network Graphics (*.png)|*.png|Graphics Interchange Format (*.gif)|*.gif|Bitmap (*.bmp)|*.bmp|Joint Photographic Experts Group Format (*.jpg)|*.jpg|Tagged Image File Format (*.tif)|*.tif|Icon format (*.ico)|*.ico",
                    FileName = textBox.Text
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    switch (saveFileDialog.FilterIndex)
                    {
                        case 1:
                            bitmap.Save(saveFileDialog.FileName, ImageFormat.Png);
                            break;
                        case 2:
                            bitmap.Save(saveFileDialog.FileName, ImageFormat.Gif);
                            break;
                        case 3:
                            bitmap.Save(saveFileDialog.FileName, ImageFormat.Bmp);
                            break;
                        case 4:
                            bitmap.Save(saveFileDialog.FileName, ImageFormat.Jpeg);
                            break;
                        case 5:
                            bitmap.Save(saveFileDialog.FileName, ImageFormat.Tiff);
                            break;
                        case 6:
                            bitmap.Save(saveFileDialog.FileName, ImageFormat.Icon);
                            break;
                        default:
                            bitmap.Save(saveFileDialog.FileName, ImageFormat.Png);
                            break;
                    }
                }
            } 
        }

        private void changeMusicButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if ((treeView.SelectedItem as TreeViewItem).Tag is WzSoundProperty)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    Title = "Wizet Studio - Import an MP3",
                    Filter = "Moving Pictures Experts Group Format 1 Audio Layer 3(*.mp3)|*.mp3"
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    WzSoundProperty soundProperty = new WzSoundProperty(((WzSoundProperty)(treeView.SelectedItem as TreeViewItem).Tag).Name, openFileDialog.FileName);
                    IPropertyContainer Parent = (IPropertyContainer)((WzSoundProperty)(treeView.SelectedItem as TreeViewItem).Tag).Parent;
                    ((WzSoundProperty)(treeView.SelectedItem as TreeViewItem).Tag).ParentImage.Changed = true;
                    ((WzSoundProperty)(treeView.SelectedItem as TreeViewItem).Tag).Remove();
                    (treeView.SelectedItem as TreeViewItem).Tag = soundProperty;
                    Parent.AddProperty(soundProperty);
                    mainWindowWrapper.wzSoundProperty = soundProperty;
                }
            }
        }

        private void exportMusicButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if ((treeView.SelectedItem as TreeViewItem).Tag is WzSoundProperty)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog()
                {
                    Title = "Wizet Studio: Export WzSound to MP3",
                    Filter = "Moving Pictures Experts Group Format 1 Audio Layer 3 (*.mp3)|*.mp3",
                    FileName = textBox.Text
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    ((treeView.SelectedItem as TreeViewItem).Tag as WzSoundProperty).SaveToFile(saveFileDialog.FileName);
                }
            }
            
        }

        private void applyValueButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (treeView.SelectedItem != null)
            {
                IWzObject IWzObject = (IWzObject)((treeView.SelectedItem as TreeViewItem).Tag);
                if (IWzObject is IWzImageProperty)
                {
                    ((IWzImageProperty)IWzObject).ParentImage.Changed = true;
                }
                WzObject.WizetObject wzobject = WzObject.GetWizetObject(IWzObject);
                switch (wzobject)
                {
                    case WzObject.WizetObject.WizetCompressedIntProperty:
                        {
                            int value = 0;
                            if (Int32.TryParse(valueTextBox.Text, out value))
                            {
                                output.WriteLine("WzCompressedIntProperty: Value '" + ((WzCompressedIntProperty)IWzObject).Value + "' has changed to '" + value + "'.");

                                ((WzCompressedIntProperty)IWzObject).Value = value;
                            }
                        }
                        break;
                    case WzObject.WizetObject.WizetDoubleProperty:
                        {
                            double value = 0.0;
                            if (Double.TryParse(valueTextBox.Text, out value))
                            {

                                output.WriteLine("WzDoubleProperty: Value '" + ((WzDoubleProperty)IWzObject).Value + "' has changed to '" + value + "'.");
                                ((WzDoubleProperty)IWzObject).Value = value;
                            }
                        }
                        break;
                    case WzObject.WizetObject.WizetByteFloatProperty:
                        {
                            float value = 0;
                            if (float.TryParse(valueTextBox.Text, out value))
                            {
                                output.WriteLine("WzByteFloatProperty: Value '" + ((WzByteFloatProperty)IWzObject).Value + "' has changed to '" + value + "'.");
                                ((WzByteFloatProperty)IWzObject).Value = value;
                            }
                        }
                        break;
                    case WzObject.WizetObject.WizetUnsignedShortProperty:
                        {
                            ushort value = 0;
                            if (UInt16.TryParse(valueTextBox.Text, out value))
                            {
                                output.WriteLine("WzUnsignedShortProperty: Value '" + ((WzUnsignedShortProperty)IWzObject).Value + "' has changed to '" + value + "'.");
                                ((WzUnsignedShortProperty)IWzObject).Value = value;
                            }
                        }
                        break;
                    case WzObject.WizetObject.WizetUolProperty:
                        output.WriteLine("WzUOLProperty: Value '" + ((WzUOLProperty)IWzObject).Value + "' has changed to '" + valueTextBox.Text + "'.");
                        ((WzUOLProperty)IWzObject).Value = valueTextBox.Text;
                        break;
                    case WzObject.WizetObject.WizetStringProperty:
                        output.WriteLine("WzStringProperty: Value '" + ((WzStringProperty)IWzObject).Value + "' has changed to '" + valueTextBox.Text + "'.");
                        ((WzStringProperty)IWzObject).Value = valueTextBox.Text;
                        break;
                    default:
                        break;
                }
            }

            
        }

        private void vectorApplyValuesButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (treeView.SelectedItem != null)
            {
                IWzObject WzObject = (IWzObject)((treeView.SelectedItem as TreeViewItem).Tag);
                if (WzObject is WzVectorProperty)
                {
                    ((WzVectorProperty)WzObject).X.Value = Convert.ToInt32(xNumericUpDown.Value);
                    ((WzVectorProperty)WzObject).Y.Value = Convert.ToInt32(xNumericUpDown.Value);
                }
            }

            
        }

        private async void aboutMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            await this.ShowMessageAsync("About Wizet Studio", 
                "Product: Wizet Studio" + Environment.NewLine +
                "Version: 2.0.1.0" + Environment.NewLine +
                "Creator: Iciclez");
        }

        private void wizetFileExtractorMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new ExtractorWindow().ShowDialog();
        }

        private void wizetFileMassEditorMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new MassEditor().ShowDialog();
        }
        
        private void RunWzImgDirsExtraction(object param)
        {
            IWzImageSerializer serializer = (IWzImageSerializer)((object[])param)[3];
            foreach (WzImage img in (List<WzImage>)((object[])param)[1])
            {
                serializer.SerializeImage(img, Path.Combine((string)((object[])param)[2], img.Name));
            }
            foreach (WzDirectory dir in (List<WzDirectory>)((object[])param)[0])
            {
                serializer.SerializeDirectory(dir, Path.Combine((string)((object[])param)[2], dir.Name));
            }

            output.WriteLine("The XML extraction task to \"" + (string)((object[])param)[2] + "\" has been completed.");

        }

        private void RunWzObjExtraction(object param)
        {
            ProgressingWzSerializer serializer = (ProgressingWzSerializer)((object[])param)[2];
            if (serializer is IWzObjectSerializer)
            {
                foreach (IWzObject obj in (List<IWzObject>)((object[])param)[0])
                {
                    ((IWzObjectSerializer)serializer).SerializeObject(obj, (string)((object[])param)[1]);
                }
            }
            else if (serializer is WzNewXmlSerializer)
            {
                ((WzNewXmlSerializer)serializer).ExportCombinedXml((List<IWzObject>)((object[])param)[0], (string)((object[])param)[1]);
            }

            output.WriteLine("The XML extraction task to \"" + (string)((object[])param)[2] + "\" has been completed.");
        }
        private delegate void InsertWzNode(WzItem node, WzItem parent);
        private async void InsertWzNodeCallback(WzItem node, WzItem parent)
        {
            WzItem child = WzItem.GetChildNode(parent, node.Header.ToString());
            if (child != null)
            {
                if (await this.ShowMessageAsync("Wizet Studio", "The node '" + node.Header.ToString() + "' already exists. Do you want it to be replaced?", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings() { AffirmativeButtonText = "Yes", NegativeButtonText = "No", ColorScheme = MetroDialogColorScheme.Theme }) == MessageDialogResult.Affirmative)
                {
                    child.Delete(child.Parent as TreeViewItem);
                }
                else
                {
                    return;
                }
            }
            parent.AddNode(node);
        }
   
        private void WzImporterThread(object param)
        {
            object[] arr = (object[])param;
            ProgressingWzSerializer deserializer = (ProgressingWzSerializer)arr[0];
            WzItem parent = (WzItem)arr[2];
            IWzObject parentObj = (IWzObject)parent.Tag;
            if (parentObj is WzFile)
            {
                parentObj = ((WzFile)parentObj).WzDirectory;
            }
            foreach (string file in (string[])arr[1])
            {
                List<IWzObject> objs;
                try
                {
                    if (deserializer is WzXmlDeserializer)
                    {
                        objs = ((WzXmlDeserializer)deserializer).ParseXML(file);
                    }
                    else
                    {
                        objs = new List<IWzObject> { ((WzImgDeserializer)deserializer).WzImageFromIMGFile(file, (byte[])arr[3], Path.GetFileName(file)) };
                    }

                }
                catch (ThreadAbortException)
                {
                    return;
                }
                catch
                {
                    continue;
                }
                foreach (IWzObject obj in objs)
                {
                    if (((obj is WzDirectory || obj is WzImage) && parentObj is WzDirectory) || (obj is IWzImageProperty && parentObj is IPropertyContainer))
                    {
                        WzItem node = new WzItem(obj);
                        if (!Dispatcher.CheckAccess())
                        {
                            Dispatcher.Invoke(new InsertWzNode(InsertWzNodeCallback), node, parent);
                        }
                        else
                        {
                            InsertWzNodeCallback(node, parent);
                        }
                    }
                }
            }
            output.WriteLine("The XML insertion task has been completed.");
        }

        private void classicDataExportMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog()
            {
                Title = "Wizet Studio - Select a Directory for the Selection Export",
            };
            if (openFolderDialog.ShowDialog() == true)
            {
                if (openFolderDialog.FileName != "")
                {
                    List<WzDirectory> dirs = new List<WzDirectory>();
                    List<WzImage> imgs = new List<WzImage>();
                    if ((treeView.SelectedItem as TreeViewItem).Tag is WzDirectory)
                    {
                        dirs.Add((WzDirectory)(treeView.SelectedItem as TreeViewItem).Tag);
                    }
                    else if ((treeView.SelectedItem as TreeViewItem).Tag is WzImage)
                    {
                        imgs.Add((WzImage)(treeView.SelectedItem as TreeViewItem).Tag);
                    }

                    new Thread(new ParameterizedThreadStart(RunWzImgDirsExtraction)).Start((object)new object[] { dirs, imgs, openFolderDialog.FileName, new WzClassicXmlSerializer(0, LineBreak.None, true) });
                }
            }
        }

        private void newDataExportMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title = "Wizet Studio - Select a Location to Save the XML",
                Filter = "eXtended Markup Language (*.xml)|*.xml",
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                if (saveFileDialog.FileName != "")
                {
                    List<IWzObject> objs = new List<IWzObject>();
                    if ((treeView.SelectedItem as TreeViewItem).Tag is IWzObject)
                    {
                        objs.Add((IWzObject)(treeView.SelectedItem as TreeViewItem).Tag);
                    }

                    new Thread(new ParameterizedThreadStart(RunWzObjExtraction)).Start((object)new object[] { objs, saveFileDialog.FileName, new WzNewXmlSerializer(0, LineBreak.None) });
                }
            }            
        }

        private void privateServerDataExportMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog()
            {
                Title = "Wizet Studio - Select a Directory for the Selection Export"
            };
            if (openFolderDialog.ShowDialog())
            {
                if (openFolderDialog.FileName != "")
                {
                    List<WzDirectory> dirs = new List<WzDirectory>();
                    List<WzImage> imgs = new List<WzImage>();
                    if ((treeView.SelectedItem as TreeViewItem).Tag is WzDirectory)
                    {
                        dirs.Add((WzDirectory)(treeView.SelectedItem as TreeViewItem).Tag);
                    }
                    else if ((treeView.SelectedItem as TreeViewItem).Tag is WzImage)
                    {
                        imgs.Add((WzImage)(treeView.SelectedItem as TreeViewItem).Tag);
                    }
                    new Thread(new ParameterizedThreadStart(RunWzImgDirsExtraction)).Start((object)new object[] { dirs, imgs, openFolderDialog.FileName, new WzClassicXmlSerializer(0, LineBreak.None, false) });
                }
            }
        }

        private void resourceExportMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog()
            {
                Title = "Wizet Studio - Select a Directory for the Selection Export",
            };
            if (openFolderDialog.ShowDialog() == true)
            {
                if (openFolderDialog.FileName != "")
                {
                    List<IWzObject> objs = new List<IWzObject>();
                    if ((treeView.SelectedItem as TreeViewItem).Tag is IWzObject)
                    {
                        objs.Add((IWzObject)(treeView.SelectedItem as TreeViewItem).Tag);
                    }

                    new Thread(new ParameterizedThreadStart(RunWzObjExtraction)).Start((object)new object[] { objs, openFolderDialog.FileName, new WzPngMp3Serializer() });
                }
            }           
        }

        private void imageExportMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog()
            {
                Title = "Wizet Studio - Select a Directory for the Selection Export",
            };
            if (openFolderDialog.ShowDialog() == true)
            {
                if (openFolderDialog.FileName != "")
                {
                    List<WzDirectory> dirs = new List<WzDirectory>();
                    List<WzImage> imgs = new List<WzImage>();
                    if ((treeView.SelectedItem as TreeViewItem).Tag is WzDirectory)
                    {
                        dirs.Add((WzDirectory)(treeView.SelectedItem as TreeViewItem).Tag);
                    }
                    else if ((treeView.SelectedItem as TreeViewItem).Tag is WzImage)
                    {
                        imgs.Add((WzImage)(treeView.SelectedItem as TreeViewItem).Tag);
                    }

                    new Thread(new ParameterizedThreadStart(RunWzImgDirsExtraction)).Start((object)new object[] { dirs, imgs, openFolderDialog.FileName, new WzImgSerializer() });
                }
            }
        }

        private void xmlImportMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if ((treeView.SelectedItem as TreeViewItem) == null || (!((treeView.SelectedItem as TreeViewItem).Tag is WzDirectory) && !((treeView.SelectedItem as TreeViewItem).Tag is WzFile) && !((treeView.SelectedItem as TreeViewItem).Tag is IPropertyContainer)))
            {
                return;
            }
            WzFile wzFile = ((IWzObject)(treeView.SelectedItem as TreeViewItem).Tag).WzFileParent;
            if (!(wzFile is WzFile))
            {
                return;
            }
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Wizet Studio - Select the XMLs",
                Filter = "eXtended Markup Language (*.xml)|*.xml",
                Multiselect = true
            };
            if (openFileDialog.ShowDialog() == true)
            {
                new Thread(new ParameterizedThreadStart(WzImporterThread)).Start(new object[] { new WzXmlDeserializer(true, WzTool.GetIvByMapleVersion(wzFile.MapleVersion)), openFileDialog.FileNames, (treeView.SelectedItem as TreeViewItem), null });
            }
        }

        private void imgImportMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if ((treeView.SelectedItem as TreeViewItem) == null || (!((treeView.SelectedItem as TreeViewItem).Tag is WzDirectory) && !((treeView.SelectedItem as TreeViewItem).Tag is WzFile) && !((treeView.SelectedItem as TreeViewItem).Tag is IPropertyContainer)))
            {
                return;
            }
            WzFile wzFile = ((IWzObject)(treeView.SelectedItem as TreeViewItem).Tag).WzFileParent;
            if (!(wzFile is WzFile))
            {
                return;
            }
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Wizet Studio - Select the Images",
                Filter = "Image Files (*.img)|*.img",
                Multiselect = true
            };
            if (openFileDialog.ShowDialog() == true)
            {
                new Thread(new ParameterizedThreadStart(WzImporterThread)).Start(new object[] { new WzImgDeserializer(false), openFileDialog.FileNames, (treeView.SelectedItem as TreeViewItem), WzTool.GetIvByMapleVersion(wzFile.MapleVersion) });
            }
        }

        private async void renderFootholdMapMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if ((treeView.SelectedItem as TreeViewItem).Tag is WzImage)
            {
                WzImage img = (WzImage)(treeView.SelectedItem as TreeViewItem).Tag;
                if (!Directory.Exists("Renders\\" + img.Name.Substring(0, img.Name.Length - 4)))
                {
                    Directory.CreateDirectory("Renders\\" + img.Name.Substring(0, img.Name.Length - 4));
                }
                new Save(img, treeView).SaveMap();
            }
            else
            {
                await this.ShowMessageAsync("Wizet Studio", "Error: You can only render a Map WzImage.");
            }
        }

        private void ChangeDarkColors()
        {
            nullGrid.Background = new System.Windows.Media.BrushConverter().ConvertFrom("#FF1A1A1A") as System.Windows.Media.Brush;
            canvasGrid.Background = new System.Windows.Media.BrushConverter().ConvertFrom("#FF1A1A1A") as System.Windows.Media.Brush;
            soundGrid.Background = new System.Windows.Media.BrushConverter().ConvertFrom("#FF1A1A1A") as System.Windows.Media.Brush;
            valueGrid.Background = new System.Windows.Media.BrushConverter().ConvertFrom("#FF1A1A1A") as System.Windows.Media.Brush;
            vectorGrid.Background = new System.Windows.Media.BrushConverter().ConvertFrom("#FF1A1A1A") as System.Windows.Media.Brush;

            //mainStatusBar.Background = new System.Windows.Media.BrushConverter().ConvertFrom("#CC000000") as System.Windows.Media.Brush;
            //selectionStatusBar.Background = new System.Windows.Media.BrushConverter().ConvertFrom("#CC000000") as System.Windows.Media.Brush;
            //versionStatusBar.Background = new System.Windows.Media.BrushConverter().ConvertFrom("#CC000000") as System.Windows.Media.Brush;

            //versionStatusBarItem.Foreground = new System.Windows.Media.BrushConverter().ConvertFrom("#FFFFFFFF") as System.Windows.Media.Brush;
            //selectionStatusBarItem.Foreground = new System.Windows.Media.BrushConverter().ConvertFrom("#FFFFFFFF") as System.Windows.Media.Brush;
            //bytesStatusBarItem.Foreground = new System.Windows.Media.BrushConverter().ConvertFrom("#FFFFFFFF") as System.Windows.Media.Brush;
            //pathStatusBarItem.Foreground = new System.Windows.Media.BrushConverter().ConvertFrom("#FFFFFFFF") as System.Windows.Media.Brush;
        }

        private void ChangeLightColors()
        {
            nullGrid.Background = new System.Windows.Media.BrushConverter().ConvertFrom("#FFE5E5E5") as System.Windows.Media.Brush;
            canvasGrid.Background = new System.Windows.Media.BrushConverter().ConvertFrom("#FFE5E5E5") as System.Windows.Media.Brush;
            soundGrid.Background = new System.Windows.Media.BrushConverter().ConvertFrom("#FFE5E5E5") as System.Windows.Media.Brush;
            valueGrid.Background = new System.Windows.Media.BrushConverter().ConvertFrom("#FFE5E5E5") as System.Windows.Media.Brush;
            vectorGrid.Background = new System.Windows.Media.BrushConverter().ConvertFrom("#FFE5E5E5") as System.Windows.Media.Brush;

            //mainStatusBar.Background = new System.Windows.Media.BrushConverter().ConvertFrom("#CCFFFFFF") as System.Windows.Media.Brush;
            //selectionStatusBar.Background = new System.Windows.Media.BrushConverter().ConvertFrom("#CCFFFFFF") as System.Windows.Media.Brush;
            //versionStatusBar.Background = new System.Windows.Media.BrushConverter().ConvertFrom("#CCFFFFFF") as System.Windows.Media.Brush;

            //versionStatusBarItem.Foreground = new System.Windows.Media.BrushConverter().ConvertFrom("#00000000") as System.Windows.Media.Brush;
            //selectionStatusBarItem.Foreground = new System.Windows.Media.BrushConverter().ConvertFrom("#00000000") as System.Windows.Media.Brush;
            //bytesStatusBarItem.Foreground = new System.Windows.Media.BrushConverter().ConvertFrom("#00000000") as System.Windows.Media.Brush;
            //pathStatusBarItem.Foreground = new System.Windows.Media.BrushConverter().ConvertFrom("#00000000") as System.Windows.Media.Brush;
        }

        private void darkThemeMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            lightThemeMenuItem.IsChecked = false;
            darkThemeMenuItem.IsChecked = true;
            iniWriter.WriteInt("Settings", "Theme", 1);
            ThemeManager.ChangeAppTheme(Application.Current, "BaseDark");
            ChangeDarkColors();
        }

        private void darkThemeMenuItem_Unchecked(object sender, RoutedEventArgs e)
        {
            if (lightThemeMenuItem.IsChecked == false)
            {
                darkThemeMenuItem.IsChecked = true;
            }
        }

        private void lightThemeMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            lightThemeMenuItem.IsChecked = true;
            darkThemeMenuItem.IsChecked = false;
            iniWriter.WriteInt("Settings", "Theme", 0);
            ThemeManager.ChangeAppTheme(Application.Current, "BaseLight");
            ChangeLightColors();
        }

        private void lightThemeMenuItem_Unchecked(object sender, RoutedEventArgs e)
        {
            if (darkThemeMenuItem.IsChecked == false)
            {
                lightThemeMenuItem.IsChecked = true;
            }
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            int theme = iniReader.ReadInt("Settings", "Theme", -1);

            switch (theme)
            {
                case -1:
                    iniWriter.WriteInt("Settings", "Theme", 1);
                    ThemeManager.ChangeAppTheme(Application.Current, "BaseDark");
                    lightThemeMenuItem.IsChecked = false;
                    darkThemeMenuItem.IsChecked = true;
                    break;
                case 0:
                    ThemeManager.ChangeAppTheme(Application.Current, "BaseLight");
                    lightThemeMenuItem.IsChecked = true;
                    darkThemeMenuItem.IsChecked = false;
                    break;
                case 1:
                    ThemeManager.ChangeAppTheme(Application.Current, "BaseDark");
                    lightThemeMenuItem.IsChecked = false;
                    darkThemeMenuItem.IsChecked = true;
                    break;

            }
        }

        private void RunExportAndViewWzImgDirs(object param)
        {
            IWzImageSerializer serializer = (IWzImageSerializer)((object[])param)[3];
            foreach (WzImage img in (List<WzImage>)((object[])param)[1])
            {
                serializer.SerializeImage(img, Path.Combine((string)((object[])param)[2], img.Name));
            }
            foreach (WzDirectory dir in (List<WzDirectory>)((object[])param)[0])
            {
                serializer.SerializeDirectory(dir, Path.Combine((string)((object[])param)[2], dir.Name));
            }

            output.WriteLine("The XML extraction task to \"" + (string)((object[])param)[2] + "\" has been completed.");

            Process.Start(Path.Combine((string)((object[])param)[2]));
        }

        private void exportAndViewExportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog()
            {
                Title = "Wizet Studio - Select a Directory for the Selection Export",
            };
            if (openFolderDialog.ShowDialog() == true)
            {
                if (openFolderDialog.FileName != "")
                {
                    List<WzDirectory> dirs = new List<WzDirectory>();
                    List<WzImage> imgs = new List<WzImage>();
                    if ((treeView.SelectedItem as TreeViewItem).Tag is WzDirectory)
                    {
                        dirs.Add((WzDirectory)(treeView.SelectedItem as TreeViewItem).Tag);
                    }
                    else if ((treeView.SelectedItem as TreeViewItem).Tag is WzImage)
                    {
                        imgs.Add((WzImage)(treeView.SelectedItem as TreeViewItem).Tag);
                    }

                    new Thread(new ParameterizedThreadStart(RunExportAndViewWzImgDirs)).Start((object)new object[] { dirs, imgs, openFolderDialog.FileName, new WzClassicXmlSerializer(0, LineBreak.None, true) });
                }
            }
        }

        private void treeView_Drop(object sender, DragEventArgs e)
        {
            String[] fileNames = (String[])e.Data.GetData(DataFormats.FileDrop, true);
            foreach (String fileName in fileNames)
            {
                if (Path.GetExtension(fileName).ToLower() == ".wz")
                {
                    mainWindowWrapper.Open(fileName);
                }
            }
        }
    }
}
