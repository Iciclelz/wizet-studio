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
using System;
using System.Windows;
using Wizet_Studio.Studio.Wizet.Windows;

namespace Wizet_Studio
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            new MainWindow(e.Args).Show();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            ThemeManager.AddAccent("BaseLight", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml"));
            ThemeManager.AddAccent("BaseDark", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseDark.xaml"));

            base.OnStartup(e);
        }
    }
}
