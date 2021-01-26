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
using Studio.Wizet.Library.Wz;
using Studio.Wizet.Library.Wz.WzProperties;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Wizet_Studio.Studio.Wizet.Wrapper.Wz.Foothold
{
    /// <summary>
    /// Interaction logic for RenderedMap.xaml
    /// </summary>
    public partial class RenderedMap : MetroWindow
    {
        public int xOffset = 0;
        public int yOffset = 0;
        public double scale = 1;
        public WzFile wzFile;
        public Image map;
        public List<SpawnPoint.Spawnpoint> MobSpawnPoints;
        public List<FootHold.Foothold> Footholds;
        public List<Portals.Portal> thePortals;
        public Save save;
        public RenderedMap(Save s, Image fullBmp, List<FootHold.Foothold> FHs, List<Portals.Portal> Ps, List<SpawnPoint.Spawnpoint> MSPs)
        {
            InitializeComponent();
            map = fullBmp;
            Footholds = FHs;
            thePortals = Ps;
            MobSpawnPoints = MSPs;
            save = s;
        }

        public void renderedMapWindow_Closed(object sender, EventArgs e)
        {
            save.DisplayMapClosed();
        }

        private void rectangle1_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Title = "Rendered Map (X: " + ((int)(xOffset + (e.GetPosition(rectangle1).X))).ToString() + ", Y: " + ((int)(yOffset + (e.GetPosition(rectangle1).Y))).ToString() + ")";
        }

        private void rectangle1_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            int index = 0;
            int tempX;
            int tempY;
            Rectangle tempRect = new Rectangle();
            foreach (FootHold.Foothold foothold in Footholds)
            {
                tempX = (int)(Footholds.ToArray()[index].Shape.X * scale);
                tempY = (int)(Footholds.ToArray()[index].Shape.Y * scale);
                tempRect = new Rectangle(tempX, tempY, (int)(foothold.Shape.Width * scale), (int)(foothold.Shape.Height * scale));
                if (tempRect.IntersectsWith(new Rectangle((int)e.GetPosition(rectangle1).X, (int)e.GetPosition(rectangle1).Y, 1, 1)))
                {
                    new MapUtil(Footholds.ToArray()[index], Int32.Parse(foothold.Data.Name)).ShowDialog();
                }
                index++;
            }
            index = 0;
            foreach (Portals.Portal portal in thePortals)
            {
                tempX = (int)(thePortals.ToArray()[index].Shape.X * scale);
                tempY = (int)(thePortals.ToArray()[index].Shape.Y * scale);
                tempRect = new Rectangle(tempX, tempY, (int)(portal.Shape.Width * scale), (int)(portal.Shape.Height * scale));
                if (tempRect.IntersectsWith(new Rectangle((int)e.GetPosition(rectangle1).X, (int)e.GetPosition(rectangle1).Y, 1, 1)))
                {
                    new MapUtil(thePortals.ToArray()[index], Int32.Parse(portal.Data.Name)).ShowDialog();
                }
                index++;
            }
            index = 0;
            foreach (SpawnPoint.Spawnpoint spawnpoint in MobSpawnPoints)
            {
                tempX = (int)(MobSpawnPoints.ToArray()[index].Shape.X * scale);
                tempY = (int)(MobSpawnPoints.ToArray()[index].Shape.Y * scale);
                tempRect = new Rectangle(tempX, tempY, (int)(spawnpoint.Shape.Width * scale), (int)(spawnpoint.Shape.Height * scale));
                if (tempRect.IntersectsWith(new Rectangle((int)e.GetPosition(rectangle1).X, (int)e.GetPosition(rectangle1).Y, 1, 1)))
                {
                    new MapUtil(MobSpawnPoints.ToArray()[index], Int32.Parse(spawnpoint.Data.Name)).ShowDialog();
                }
                index++;
            }
        }

        private void renderedMapWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Bitmap theMap = new Bitmap((int)(map.Width * scale), (int)(map.Height * scale));
            using (Graphics g = Graphics.FromImage(theMap))
            {
                g.DrawImage(new Bitmap(map), 0, 0, (int)(map.Width * scale), (int)(map.Height * scale));
            }
            rectangle1.Height = theMap.Height;
            rectangle1.Width = theMap.Width;
            rectangle1.Fill = Windows.Components.ImageConverter.BitmapToImageBrush(theMap);

            xOffset = (int)((((thePortals.ToArray()[0].Shape.X) + 20) - ((WzCompressedIntProperty)thePortals.ToArray()[0].Data["x"]).Value) * -1);
            yOffset = (int)((((thePortals.ToArray()[0].Shape.Y) + 20) - ((WzCompressedIntProperty)thePortals.ToArray()[0].Data["y"]).Value) * -1);
        }
    }
}
