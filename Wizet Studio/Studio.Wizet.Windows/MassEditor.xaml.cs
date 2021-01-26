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
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Studio.Wizet.Library.Wz;
using Studio.Wizet.Library.Wz.WzProperties;
using System;
using System.Windows.Controls;
using Wizet_Studio.Studio.Wizet.Windows.Components;

namespace Wizet_Studio.Studio.Wizet.Windows
{
    /// <summary>
    /// Interaction logic for MassEditor.xaml
    /// </summary>
    public partial class MassEditor : MetroWindow
    {
        public MassEditor()
        {
            InitializeComponent();
        }

        private void mobRadioButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            listView.Items.Clear();
            listView.Items.Add(new{ IsCheckBoxChecked = false, IsTextBoxEnabled = false, PropertyName = "acc", PropertyValue = 1 });
            listView.Items.Add(new { IsCheckBoxChecked = false, IsTextBoxEnabled = false, PropertyName = "bodyAttack", PropertyValue = 0 });
            listView.Items.Add(new { IsCheckBoxChecked = false, IsTextBoxEnabled = false, PropertyName = "PADamage", PropertyValue = 1 });
            listView.Items.Add(new { IsCheckBoxChecked = false, IsTextBoxEnabled = false, PropertyName = "MADamage", PropertyValue = 1 });
            listView.Items.Add(new { IsCheckBoxChecked = false, IsTextBoxEnabled = false, PropertyName = "firstAttack", PropertyValue = 0 });
            listView.Items.Add(new { IsCheckBoxChecked = false, IsTextBoxEnabled = false, PropertyName = "speed", PropertyValue = -100 });
            listView.Items.Add(new { IsCheckBoxChecked = false, IsTextBoxEnabled = false, PropertyName = "chaseSpeed", PropertyValue = -100 });
            listView.Items.Add(new { IsCheckBoxChecked = false, IsTextBoxEnabled = false, PropertyName = "flySpeed", PropertyValue = -100 });
            listView.Items.Add(new { IsCheckBoxChecked = false, IsTextBoxEnabled = true, PropertyName = "", PropertyValue = 0});
        }

        private void wizetFileButton_Click(object sender, System.Windows.RoutedEventArgs e)
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

        private void outputDirectoryButton_Click(object sender, System.Windows.RoutedEventArgs e)
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

        private async void patchButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (mobRadioButton.IsChecked == true)
            {
                patchButton.IsEnabled = false;
                WzFile f = new WzFile(wizetFilePathTextBox.Text, WzMapleVersion.CLASSIC);
                f.ParseWzFile();
    
                foreach (WzImage mob in f.WzDirectory.WzImages)
                {
                    mob.ParseImage();
                    WzSubProperty info = (WzSubProperty)mob["info"];
                    if (info != null)
                    {
                        for (int i = 0; i < listView.Items.Count; i++)
                        {
                            if (((dynamic)listView.Items[i]).IsCheckBoxChecked == true)
                            {
                                WzCompressedIntProperty property = (WzCompressedIntProperty)info[((dynamic)listView.Items[i]).PropertyName];
                                if (property != null)
                                {
                                    property.Value = (int)((dynamic)listView.Items[i]).PropertyValue;
                                    mob.Changed = true;
                                }
                            }
                        }
                    }
                }
                f.SaveToDisk(outputDirectoryPathTextBox.Text);
                patchButton.IsEnabled = true;

                await this.ShowMessageAsync("Wizet Studio", "The XML extraction has been completed.");
            }
        }

        private void massEditorWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (patchButton.IsEnabled == false)
            {
                e.Cancel = true;
            }
        }
    }
}
