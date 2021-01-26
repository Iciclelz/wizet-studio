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
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Wizet_Studio.Studio.Wizet.Windows.Helper
{
    class Output
    {
        private TextBox textBox;
        public Output(TextBox txtBox)
        {
            textBox = txtBox;
            textBox.IsReadOnly = true;
        }

        public void Write(String line)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                textBox.Text += line;
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                  new Action(() => {
                      textBox.Text += line;
                  }));
            }

            
        }

        public void WriteLine(String line)
        {
            Write(line + "\r\n");
        }

        public void Clear()
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                textBox.Text = "";
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                  new Action(() => {
                      textBox.Text = "";
                  }));
            }

            
        }
    }
}
