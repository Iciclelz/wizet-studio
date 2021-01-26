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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;

namespace Wizet_Studio.Studio.Wizet.Wrapper.Wz
{
    class WzFileManager
    {
        private List<WzFile> wzFiles;
        public WzFileManager()
        {
            wzFiles = new List<WzFile>();
        }

        public void InsertWzFileUnsafe(WzFile f, TreeView panel)
        {
            this.wzFiles.Add(f);
            WzItem WzItem = new WzItem(f);
            panel.Items.Add(WzItem);
            this.SortNodesRecursively(WzItem);
        }

        public WzFile LoadWzFile(string path, WzMapleVersion encVersion, TreeView panel)
        {
            return this.LoadWzFile(path, encVersion, (short)-1, panel);
        }

        private WzFile LoadWzFile(string path, WzMapleVersion encVersion, short version, TreeView panel)
        {
            try
            {
                WzFile wzFile = new WzFile(path, version, encVersion);
                wzFiles.Add(wzFile);
                wzFile.ParseWzFile();
                SortNodesRecursively(new WzItem(wzFile, panel) { PathTag = path });
                return wzFile;
            }
            catch
            {
                return null;
            }
        }


        public void ReloadAll(TreeView panel)
        {
            for (int i = 0; i < this.wzFiles.Count; i++)
            {
                this.ReloadWzFile(this.wzFiles[i], panel);
            }
        }

        public void ReloadWzFile(WzFile file, TreeView panel)
        {
            WzMapleVersion mapleVersion = file.MapleVersion;
            string filePath = file.FilePath;
            short version = file.Version;
            ((WzItem)file.HRTag).Delete((TreeViewItem)(((WzItem)file.HRTag).Parent));
            this.wzFiles.Remove(file);
            this.LoadWzFile(filePath, mapleVersion, -1, panel);
        }

        private void SortNodesRecursively(WzItem parent)
        {
            parent.Items.SortDescriptions.Clear();
            parent.Items.SortDescriptions.Add(new SortDescription("Header", ListSortDirection.Ascending));
            parent.Items.Refresh(); 
            
            foreach (WzItem item in parent.Items)
            {
                SortNodesRecursively(item);
            }   
        }

        public void Terminate()
        {
            foreach (WzFile wzFile in this.wzFiles)
            {
                wzFile.Dispose();
            }
        }

        public void UnloadAll()
        {
            while (this.wzFiles.Count > 0)
            {
                this.UnloadWzFile(this.wzFiles[0]);
            }
        }

        public void UnloadWzFile(WzFile file)
        {
            ((WzItem)file.HRTag).Delete((TreeViewItem)((WzItem)file.HRTag).Parent);
            this.wzFiles.Remove(file);
        }
    }
}

