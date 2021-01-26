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
using Studio.Wizet.Library.Wz;
using Studio.Wizet.Library.Wz.WzProperties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace Wizet_Studio.Studio.Wizet.Wrapper.Wz.Foothold
{
    public class FootHold
    {
        public struct Foothold
        {
            public Rectangle Shape;
            public WzSubProperty Data;
        }
    }
    public class Portals
    {
        public struct Portal
        {
            public Rectangle Shape;
            public WzSubProperty Data;
        }
    }
    public class SpawnPoint
    {
        public struct Spawnpoint
        {
            public Rectangle Shape;
            public WzSubProperty Data;
        }
    }
    public class Save
    {
        private WzImage img = null;
        private TreeView treeView1;

        public Save(WzImage wzimage, TreeView tv)
        {
            img = wzimage;
            treeView1 = tv;
        }
        public void SaveMap()
        {
            WzFile wzFile = (WzFile)((IWzObject)((treeView1.SelectedItem as TreeViewItem).Tag)).WzFileParent;
            List<SpawnPoint.Spawnpoint> MSPs = new List<SpawnPoint.Spawnpoint>();
            List<FootHold.Foothold> FHs = new List<FootHold.Foothold>();
            List<Portals.Portal> Ps = new List<Portals.Portal>();
            System.Drawing.Size bmpSize;
            System.Drawing.Point center;
            try
            {
                bmpSize = new System.Drawing.Size(((WzCompressedIntProperty)((WzSubProperty)img["miniMap"])["width"]).Value, ((WzCompressedIntProperty)((WzSubProperty)img["miniMap"])["height"]).Value);
                center = new System.Drawing.Point(((WzCompressedIntProperty)((WzSubProperty)img["miniMap"])["centerX"]).Value, ((WzCompressedIntProperty)((WzSubProperty)img["miniMap"])["centerY"]).Value);
            }
            catch (KeyNotFoundException)
            {
                try
                {
                    bmpSize = new System.Drawing.Size(((WzCompressedIntProperty)((WzSubProperty)img["info"])["VRRight"]).Value - ((WzCompressedIntProperty)((WzSubProperty)img["info"])["VRLeft"]).Value, ((WzCompressedIntProperty)((WzSubProperty)img["info"])["VRBottom"]).Value - ((WzCompressedIntProperty)((WzSubProperty)img["info"])["VRTop"]).Value);
                    center = new System.Drawing.Point(((WzCompressedIntProperty)((WzSubProperty)img["info"])["VRRight"]).Value, ((WzCompressedIntProperty)((WzSubProperty)img["info"])["VRBottom"]).Value);
                }
                catch
                {
                    return;
                }
            }
            catch
            {
                return;
            }
            Bitmap mapRender = new Bitmap(bmpSize.Width, bmpSize.Height + 10);
            using (Graphics drawBuf = Graphics.FromImage(mapRender))
            {
                drawBuf.DrawString("Map Id: " + img.Name.Substring(0, img.Name.Length - 4), new Font("Segoe UI", 18), new SolidBrush(Color.Black), new PointF(10, 10));
                try
                {
                    drawBuf.DrawImage(((WzCanvasProperty)((WzSubProperty)img["miniMap"])["canvas"]).PngProperty.GetPNG(false), 10, 45);
                }
                catch (KeyNotFoundException)
                {
                    drawBuf.DrawString("Minimap not availible", new Font("Segoe UI", 18), new SolidBrush(Color.Black), new PointF(10, 45));
                }
                WzSubProperty ps = (WzSubProperty)img["portal"];
                foreach (WzSubProperty p in ps.WzProperties)
                {
                    int x = ((WzCompressedIntProperty)p["x"]).Value + center.X;
                    int y = ((WzCompressedIntProperty)p["y"]).Value + center.Y;
                    int type = ((WzCompressedIntProperty)p["pt"]).Value;
                    Color pColor = Color.Red;
                    if (type == 0)
                        pColor = Color.Orange;
                    else if (type == 2 || type == 7)
                        pColor = Color.Blue;
                    else if (type == 3)
                        pColor = Color.Magenta;
                    else if (type == 1 || type == 8)
                        pColor = Color.BlueViolet;
                    else
                        pColor = Color.IndianRed;
                    drawBuf.FillRectangle(new SolidBrush(Color.FromArgb(95, pColor.R, pColor.G, pColor.B)), x - 20, y - 20, 40, 40);
                    drawBuf.DrawRectangle(new Pen(Color.Black, 1F), x - 20, y - 20, 40, 40);
                    drawBuf.DrawString(p.Name, new Font("Pragmata", 8), new SolidBrush(Color.Red), x - 8, y - 7.7F);
                    Portals.Portal portal = new Portals.Portal();
                    portal.Shape = new Rectangle(x - 20, y - 20, 40, 40);
                    portal.Data = p;
                    Ps.Add(portal);
                }
                try
                {
                    WzSubProperty SPs = (WzSubProperty)img["life"];
                    foreach (WzSubProperty sp in SPs.WzProperties)
                    {
                        Color MSPColor = Color.ForestGreen;
                        if (((WzStringProperty)sp["type"]).Value == "m")
                        {
                            int x = ((WzCompressedIntProperty)sp["x"]).Value + center.X - 15;
                            int y = ((WzCompressedIntProperty)sp["y"]).Value + center.Y - 15;
                            drawBuf.FillRectangle(new SolidBrush(Color.FromArgb(95, MSPColor.R, MSPColor.G, MSPColor.B)), x, y, 30, 30);
                            drawBuf.DrawRectangle(new Pen(Color.Black, 1F), x, y, 30, 30);
                            drawBuf.DrawString(sp.Name, new Font("Pragmata", 8), new SolidBrush(Color.Red), x + 7, y + 7.3F);
                            SpawnPoint.Spawnpoint MSP = new SpawnPoint.Spawnpoint();
                            MSP.Shape = new Rectangle(x, y, 30, 30);
                            MSP.Data = sp;
                            MSPs.Add(MSP);
                        }
                    }
                }
                catch
                {
                }
                WzSubProperty fhs = (WzSubProperty)img["foothold"];
                foreach (IWzImageProperty fhspl0 in fhs.WzProperties)
                    foreach (IWzImageProperty fhspl1 in fhspl0.WzProperties)
                        foreach (WzSubProperty fh in fhspl1.WzProperties)
                        {
                            int x = ((WzCompressedIntProperty)fh["x1"]).Value + center.X;
                            int y = ((WzCompressedIntProperty)fh["y1"]).Value + center.Y;
                            int width = ((((WzCompressedIntProperty)fh["x2"]).Value + center.X) - x);
                            int height = ((((WzCompressedIntProperty)fh["y2"]).Value + center.Y) - y);
                            if (width < 0)
                            {
                                x += width;
                                width = -width;
                            }
                            if (height < 0)
                            {
                                y += height;
                                height = -height;
                            }
                            if (width == 0 || width < 15)
                                width = 15;
                            height += 10;
                            FootHold.Foothold nFH = new FootHold.Foothold();
                            nFH.Shape = new Rectangle(x, y, width, height);
                            nFH.Data = fh;
                            FHs.Add(nFH);
                            drawBuf.FillRectangle(new SolidBrush(Color.FromArgb(95, Color.Gray.R, Color.Gray.G, Color.Gray.B)), x, y, width, height);
                            drawBuf.DrawRectangle(new Pen(Color.Black, 1F), x, y, width, height);
                            drawBuf.DrawString(fh.Name, new Font("Pragmata", 8), new SolidBrush(Color.Red), new PointF(x + (width / 2) - 8, y + (height / 2) - 7.7F));
                        }
            }

            mapRender.Save("Renders\\" + img.Name.Substring(0, img.Name.Length - 4) + "\\" + img.Name.Substring(0, img.Name.Length - 4) + "_footholdRender.bmp");

            Bitmap tileRender = new Bitmap(bmpSize.Width, bmpSize.Height);

            using (Graphics tileBuf = Graphics.FromImage(tileRender))
            {

                for (int i = 0; i < 7; i++)
                {

                    if (((WzSubProperty)((WzSubProperty)img[i.ToString()])["obj"]).WzProperties.Count > 0)
                    {
                        foreach (WzSubProperty obj in ((WzSubProperty)((WzSubProperty)img[i.ToString()])["obj"]).WzProperties)
                        {
                            string imgName = ((WzStringProperty)obj["oS"]).Value + ".img";
                            string l0 = ((WzStringProperty)obj["l0"]).Value;
                            string l1 = ((WzStringProperty)obj["l1"]).Value;
                            string l2 = ((WzStringProperty)obj["l2"]).Value;
                            int x = ((WzCompressedIntProperty)obj["x"]).Value + center.X;
                            int y = ((WzCompressedIntProperty)obj["y"]).Value + center.Y;
                            WzVectorProperty origin;
                            WzPngProperty png;
                            IWzImageProperty objData = (IWzImageProperty)wzFile.GetObjectFromPath(wzFile.WzDirectory.Name + "/Obj/" + imgName + "/" + l0 + "/" + l1 + "/" + l2 + "/0");
                            tryagain:
                            if (objData is WzCanvasProperty)
                            {
                                png = ((WzCanvasProperty)objData).PngProperty;
                                origin = (WzVectorProperty)((WzCanvasProperty)objData)["origin"];
                            }
                            else if (objData is WzUOLProperty)
                            {
                                IWzObject currProp = objData.Parent;
                                foreach (string directive in ((WzUOLProperty)objData).Value.Split("/".ToCharArray()))
                                {
                                    if (directive == "..") currProp = currProp.Parent;
                                    else
                                    {
                                        switch (currProp.GetType().Name)
                                        {
                                            case "WzSubProperty":
                                                currProp = ((WzSubProperty)currProp)[directive];
                                                break;
                                            case "WzCanvasProperty":
                                                currProp = ((WzCanvasProperty)currProp)[directive];
                                                break;
                                            case "WzImage":
                                                currProp = ((WzImage)currProp)[directive];
                                                break;
                                            case "WzConvexProperty":
                                                currProp = ((WzConvexProperty)currProp)[directive];
                                                break;
                                            default:
                                                throw new Exception("UOL error at map renderer");
                                        }
                                    }
                                }
                                objData = (IWzImageProperty)currProp;
                                goto tryagain;
                            }
                            else throw new Exception("unknown type at map renderer");

                            tileBuf.DrawImage(png.GetPNG(false), x - origin.X.Value, y - origin.Y.Value);
                        }
                    }
                    if (((WzSubProperty)((WzSubProperty)img[i.ToString()])["info"]).WzProperties.Count == 0)

                        continue;

                    if (((WzSubProperty)((WzSubProperty)img[i.ToString()])["tile"]).WzProperties.Count == 0)

                        continue;

                    string tileSetName = ((WzStringProperty)((WzSubProperty)((WzSubProperty)img[i.ToString()])["info"])["tS"]).Value;


                    WzImage tileSet = (WzImage)wzFile.GetObjectFromPath(wzFile.WzDirectory.Name + "/Tile/" + tileSetName + ".img");
                    if (!tileSet.Parsed)
                        tileSet.ParseImage();

                    foreach (WzSubProperty tile in ((WzSubProperty)((WzSubProperty)img[i.ToString()])["tile"]).WzProperties)
                    {

                        int x = ((WzCompressedIntProperty)tile["x"]).Value + center.X;

                        int y = ((WzCompressedIntProperty)tile["y"]).Value + center.Y;

                        string tilePackName = ((WzStringProperty)tile["u"]).Value;

                        string tileID = ((WzCompressedIntProperty)tile["no"]).Value.ToString();

                        System.Drawing.Point origin = new System.Drawing.Point(((WzVectorProperty)((WzCanvasProperty)((WzSubProperty)tileSet[tilePackName])[tileID])["origin"]).X.Value, ((WzVectorProperty)((WzCanvasProperty)((WzSubProperty)tileSet[tilePackName])[tileID])["origin"]).Y.Value);

                        tileBuf.DrawImage(((WzCanvasProperty)((WzSubProperty)tileSet[tilePackName])[tileID]).PngProperty.GetPNG(false), x - origin.X, y - origin.Y);

                    }

                }
            }

            tileRender.Save("Renders\\" + img.Name.Substring(0, img.Name.Length - 4) + "\\" + img.Name.Substring(0, img.Name.Length - 4) + "_tileRender.bmp");

            Bitmap fullBmp = new Bitmap(bmpSize.Width, bmpSize.Height + 10);

            using (Graphics fullBuf = Graphics.FromImage(fullBmp))
            {

                fullBuf.FillRectangle(new SolidBrush(Color.CornflowerBlue), 0, 0, bmpSize.Width, bmpSize.Height + 10);

                fullBuf.DrawImage(tileRender, 0, 0);

                fullBuf.DrawImage(mapRender, 0, 0);

            }

            fullBmp.Save("Renders\\" + img.Name.Substring(0, img.Name.Length - 4) + "\\" + img.Name.Substring(0, img.Name.Length - 4) + "_fullRender.bmp");

            RenderedMap showMap = new RenderedMap(this, fullBmp, FHs, Ps, MSPs);
            
            try
            {
                showMap.ShowDialog();
            }
            catch 
            {
                MessageBox.Show("You must set the render scale to a valid number.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        public void DisplayMapClosed()
        {
            (treeView1.SelectedItem as WzItem).Reparse();
        }

    }
}
