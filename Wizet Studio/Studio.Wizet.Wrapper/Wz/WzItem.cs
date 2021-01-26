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
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Wizet_Studio.Studio.Wizet.Wrapper.Wz
{
    class WzItem : TreeViewItem
    {
        public String PathTag;
        public WzItem(IWzObject SourceObject) : base()
        {
            Header = SourceObject.Name;
            SetResourceReference(StyleProperty, "WzTreeViewItem");
            ParseChilds(SourceObject);  
        }

        public WzItem(IWzObject SourceObject, TreeView treeView) : this(SourceObject)
        {
            treeView.Items.Add(this);
        }

        private void ParseChilds(IWzObject SourceObject)
        {
            if (SourceObject == null)
                throw new NullReferenceException("Cannot create a null WzItem");
            Tag = SourceObject;
            SourceObject.HRTag = this;
            if (SourceObject is WzFile)
                SourceObject = ((WzFile)SourceObject).WzDirectory;
            if (SourceObject is WzDirectory)
            {
                foreach (WzDirectory dir in ((WzDirectory)SourceObject).WzDirectories)
                    Items.Add(new WzItem(dir));
                foreach (WzImage img in ((WzDirectory)SourceObject).WzImages)
                    Items.Add(new WzItem(img));
            }
            else if (SourceObject is WzImage)
            {
                if (((WzImage)SourceObject).Parsed)
                    foreach (IWzImageProperty prop in ((WzImage)SourceObject).WzProperties)
                    {
                        Items.Add(new WzItem(prop));
                        //Items.Add(new TreeViewItem() { Header = prop.Name });

                    }
            }
            else if (SourceObject is IPropertyContainer)
            {
                foreach (IWzImageProperty prop in ((IPropertyContainer)SourceObject).WzProperties)
                {
                    Items.Add(new WzItem(prop));
                    //Items.Add(new TreeViewItem() { Header = prop.Name });
                }
            }
        }


        public void Delete(TreeViewItem parent)
        {
            parent.Items.Remove(this);
            if (Tag is IWzImageProperty)
            {
                ((IWzImageProperty)Tag).ParentImage.Changed = true;
            }
            ((IWzObject)Tag).Remove();
        }

        public bool CanHaveChilds
        {
            get
            {
                return (Tag is WzFile ||
                    Tag is WzDirectory ||
                    Tag is WzImage ||
                    Tag is IPropertyContainer);
            }
        }

        public static WzItem GetChildNode(WzItem parentNode, string name)
        {
            foreach (WzItem node in parentNode.Items)
                if (node.Header.ToString().Equals(name))
                    return node;
            return null;
        }

        public static bool CanNodeBeInserted(WzItem parentNode, string name)
        {
            IWzObject obj = parentNode.Tag as IWzObject;
            if (obj is IPropertyContainer) return ((IPropertyContainer)obj)[name] == null;
            else if (obj is WzDirectory) return ((WzDirectory)obj)[name] == null;
            else if (obj is WzFile) return ((WzFile)obj)[name] == null;
            else return false;
        }

        private void addObjInternal(IWzObject obj)
        {
            IWzObject TaggedObject = (IWzObject)Tag;
            if (TaggedObject is WzFile) TaggedObject = ((WzFile)TaggedObject);
            if (TaggedObject is WzDirectory)
            {
                if (obj is WzDirectory)
                    ((WzDirectory)TaggedObject).AddDirectory((WzDirectory)obj);
                else if (obj is WzImage)
                    ((WzDirectory)TaggedObject).AddImage((WzImage)obj);
                else return;
            }
            else if (TaggedObject is WzImage)
            {
                if (!((WzImage)TaggedObject).Parsed) ((WzImage)TaggedObject).ParseImage();
                if (obj is IWzImageProperty)
                    ((WzImage)TaggedObject).AddProperty((IWzImageProperty)obj);
                else return;
            }
            else if (TaggedObject is IPropertyContainer)
            {
                if (obj is IWzImageProperty)
                    ((IPropertyContainer)TaggedObject).AddProperty((IWzImageProperty)obj);
                else return;
            }
            else return;
        }

        public bool MoveWzObjectInto(WzItem newParent)
        {
            if (CanNodeBeInserted(newParent, Header.ToString()))
            {
                addObjInternal((IWzObject)newParent.Tag);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AddNode(WzItem node)
        {
            if (CanNodeBeInserted(this, node.Header.ToString()))
            {
                TryParseImage();
                Items.Add(node);
                addObjInternal((IWzObject)node.Tag);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void TryParseImage()
        {
            if (Tag is WzImage && !((WzImage)Tag).Parsed)
            {
                ((WzImage)Tag).ParseImage();
                Reparse();
            }
        }

        public bool AddObject(IWzObject obj, WzItemEdit nodeEdit)
        {
            if (CanNodeBeInserted(this, obj.Name))
            {
                TryParseImage();
                addObjInternal(obj);
                WzItem node = new WzItem(obj);
                Items.Add(node);
                if (node.Tag is IWzImageProperty)
                {
                    ((IWzImageProperty)node.Tag).ParentImage.Changed = true;
                }
                List<WzItemEditAction> wzEdit = new List<WzItemEditAction> { WzItemEditAction.ObjectAdded(this, node) };
                nodeEdit.AddUndoBatch(wzEdit);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Reparse()
        {
            Items.Clear();
            ParseChilds((IWzObject)Tag);
        }

        public string GetTypeName()
        {
            return Tag.GetType().Name;
        }

        public void ChangeName(string name)
        {
            Header = name;
            ((IWzObject)Tag).Name = name;
            if (Tag is IWzImageProperty)
            {
                ((IWzImageProperty)Tag).ParentImage.Changed = true;
            }
        }

        public static WzItem getTopLevelNode(WzItem treeViewItem)
        {
            if ((treeViewItem as WzItem).Tag is WzFile || treeViewItem.Parent == null)
            {
                return treeViewItem;
            }
            else 
            {
                return getTopLevelNode(treeViewItem.Parent as WzItem);
            }
        }

    }
}
