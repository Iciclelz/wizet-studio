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
using System.Collections.Generic;
using System.Windows.Controls;

namespace Wizet_Studio.Studio.Wizet.Wrapper.Wz
{
    class WzItemEdit
    {
        public List<WzItemEditBatch> UndoList = new List<WzItemEditBatch>();
        public List<WzItemEditBatch> RedoList = new List<WzItemEditBatch>();
        private TreeView parentPanel;

        public WzItemEdit(TreeView parentPanel)
        {
            this.parentPanel = parentPanel;
        }

        public void AddUndoBatch(List<WzItemEditAction> actions)
        {
            WzItemEditBatch batch = new WzItemEditBatch() { Actions = actions };
            UndoList.Add(batch);
            RedoList.Clear();
        }


        public void Undo()
        {
            WzItemEditBatch action = UndoList[UndoList.Count - 1];
            action.WzItemEdit();
            action.SwitchActions();
            UndoList.RemoveAt(UndoList.Count - 1);
            RedoList.Add(action);
        }

        public void Redo()
        {
            WzItemEditBatch action = RedoList[RedoList.Count - 1];
            action.WzItemEdit();
            action.SwitchActions();
            RedoList.RemoveAt(RedoList.Count - 1);
            UndoList.Add(action);
        }
    }

    class WzItemEditAction
    {
        private WzItem item;
        private WzItem parent;
        private WzItemEditType type;

        public WzItemEditAction(WzItem item, WzItem parent, WzItemEditType type)
        {
            this.item = item;
            this.parent = parent;
            this.type = type;
        }

        public void WzItemEdit()
        {
            switch (type)
            {
                case WzItemEditType.ObjectAdded:
                    parent.Items.Remove(item);
                    break;
                case WzItemEditType.ObjectRemoved:
                    parent.Items.Add(item);
                    break;
            }
        }


        public unsafe void SwitchAction()
        {
            switch (type)
            {
                case WzItemEditType.ObjectAdded:
                    type = WzItemEditType.ObjectRemoved;
                    break;
                case WzItemEditType.ObjectRemoved:
                    type = WzItemEditType.ObjectAdded;
                    break;

            }
        }

        public static WzItemEditAction ObjectAdded(WzItem parent, WzItem item)
        {
            return new WzItemEditAction(item, parent, WzItemEditType.ObjectAdded);
        }

        public static WzItemEditAction ObjectRemoved(WzItem parent, WzItem item)
        {
            return new WzItemEditAction(item, parent, WzItemEditType.ObjectRemoved);
        }

    }

    class WzItemEditBatch
    {
        public List<WzItemEditAction> Actions = new List<WzItemEditAction>();

        public void WzItemEdit()
        {
            foreach (WzItemEditAction action in Actions)
            {
                action.WzItemEdit();
            }
        }

        public void SwitchActions()
        {
            foreach (WzItemEditAction action in Actions)
            {
                action.SwitchAction();
            }
        }
    }

    public enum WzItemEditType
    {
        ObjectAdded,
        ObjectRemoved,
        ObjectChanged
    }
}
