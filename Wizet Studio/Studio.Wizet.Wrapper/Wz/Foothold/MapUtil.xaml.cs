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
using Studio.Wizet.Library.Wz.WzProperties;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Wizet_Studio.Studio.Wizet.Wrapper.Wz.Foothold
{
    /// <summary>
    /// Interaction logic for MapUtil.xaml
    /// </summary>
    public partial class MapUtil : MetroWindow
    {
        private bool systemChangeTab;
        private int tabIndex;
        private FootHold.Foothold fh;
        private Portals.Portal portal;
        private SpawnPoint.Spawnpoint spawnpoint;
        private int Id;
        public MapUtil(FootHold.Foothold f, int id)
        {
            InitializeComponent();
            Id = id;
            fh = f;
            changeTabControlSelectedItem(0);

            mapFootholdIdNumericUpDown.Value = Convert.ToDouble(Id);
            previousValueNumericUpDown.Value = Convert.ToDouble(((WzCompressedIntProperty)fh.Data["prev"]).Value);
            nextValueNumericUpDown.Value = Convert.ToDouble(((WzCompressedIntProperty)fh.Data["next"]).Value);
            try { forceValueNumericUpDown.Value = Convert.ToDouble(((WzCompressedIntProperty)fh.Data["force"]).Value); }
            catch { forceValueNumericUpDown.Value = Convert.ToDouble(-1); }
        }

        public MapUtil(Portals.Portal p, int id)
        {
            InitializeComponent();
            Id = id;
            portal = p;
            changeTabControlSelectedItem(1);

            portalMapIdNumericUpDown.Value = Convert.ToDouble(Id);
            portalTypeNumericUpDown.Value = Convert.ToDouble(((WzCompressedIntProperty)portal.Data["pt"]).Value);
            destinationNumericUpDown.Value = Convert.ToDouble(((WzCompressedIntProperty)portal.Data["tm"]).Value);
            mapXNumericUpDown.Value = Convert.ToDouble(((WzCompressedIntProperty)portal.Data["x"]).Value);
            mapYNumericUpDown.Value = Convert.ToDouble(((WzCompressedIntProperty)portal.Data["y"]).Value);
        }

        public MapUtil(SpawnPoint.Spawnpoint s, int id)
        {
            InitializeComponent();
            Id = id;
            spawnpoint = s;
            changeTabControlSelectedItem(2);

            mobSpawnPointIdNumericUpDown.Value = Convert.ToDouble(id);
            mobIdNumericUpDown.Value = Convert.ToDouble(((WzStringProperty)spawnpoint.Data["id"]).Value);
            buddyFootholdIdNumericUpDown.Value = Convert.ToDouble(((WzCompressedIntProperty)spawnpoint.Data["fh"]).Value);
            spawnPointMapXNumericUpDown.Value = Convert.ToDouble(((WzCompressedIntProperty)spawnpoint.Data["x"]).Value);
            spawnPointMapYNumericUpDown.Value = Convert.ToDouble(((WzCompressedIntProperty)spawnpoint.Data["y"]).Value);
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        private void button_Click(object sender, RoutedEventArgs e)
        {
            switch (tabControl.SelectedIndex)
            {
                case 0:
                    {
                        try
                        {
                            if (((WzCompressedIntProperty)fh.Data["prev"]).Value != (Int32)previousValueNumericUpDown.Value)
                            {
                                ((WzCompressedIntProperty)fh.Data["prev"]).Value = (Int32)previousValueNumericUpDown.Value;
                                fh.Data["prev"].ParentImage.Changed = true;
                            }
                            if (((WzCompressedIntProperty)fh.Data["next"]).Value != (Int32)nextValueNumericUpDown.Value)
                            {
                                ((WzCompressedIntProperty)fh.Data["next"]).Value = (Int32)nextValueNumericUpDown.Value;
                                fh.Data["next"].ParentImage.Changed = true;
                            }
                            if ((Int32)forceValueNumericUpDown.Value != -1)
                            {
                                try
                                {
                                    if (((WzCompressedIntProperty)fh.Data["force"]).Value != (Int32)forceValueNumericUpDown.Value)
                                    {
                                        ((WzCompressedIntProperty)fh.Data["force"]).Value = (Int32)forceValueNumericUpDown.Value;
                                        fh.Data["force"].ParentImage.Changed = true;
                                    }
                                }
                                catch
                                {
                                    fh.Data.AddProperty(new WzCompressedIntProperty("force", (Int32)forceValueNumericUpDown.Value));
                                    fh.Data["force"].ParentImage.Changed = true;
                                }

                            }
                            this.Close();
                        }
                        catch
                        {
                            MessageBox.Show("An error has occured while processing the specified values.", "Wizet Studio: Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        break;
                    }
                case 1:
                    {
                        try
                        {
                            if ((Int32)portalTypeNumericUpDown.Value != ((WzCompressedIntProperty)portal.Data["pt"]).Value)
                            {
                                ((WzCompressedIntProperty)portal.Data["pt"]).Value = (Int32)portalTypeNumericUpDown.Value;
                                portal.Data["pt"].ParentImage.Changed = true;
                            }
                            if ((Int32)mapXNumericUpDown.Value != ((WzCompressedIntProperty)portal.Data["x"]).Value)
                            {
                                ((WzCompressedIntProperty)portal.Data["x"]).Value = (Int32)mapXNumericUpDown.Value;
                                portal.Data["x"].ParentImage.Changed = true;
                            }
                            if ((Int32)mapYNumericUpDown.Value != ((WzCompressedIntProperty)portal.Data["y"]).Value)
                            {
                                ((WzCompressedIntProperty)portal.Data["y"]).Value = (Int32)mapYNumericUpDown.Value;
                                portal.Data["y"].ParentImage.Changed = true;
                            }
                            this.Close();
                        }
                        catch
                        {
                            MessageBox.Show("An error has occured while processing the specified values.", "Wizet Studio: Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        break;
                    }
                case 2:
                    {
                        this.Close();
                        break;
                    }
                default:
                    {
                        this.Close();
                        break;
                    }
            }
        }
    }
}
