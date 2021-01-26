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
using System.Runtime.InteropServices;
using System.Text;

namespace Wizet_Studio.Studio.Wizet.Windows.Components
{
    class IniHelper
    {
    }

    class IniReader
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, uint nSize, string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        static extern uint GetPrivateProfileInt(string lpAppName, string lpKeyName, int nDefault, string lpFileName);

        private string filePath;
        public IniReader(string path)
        {
            filePath = path;
        }

        public string Read(string lpAppName, string lpKeyName)
        {
            StringBuilder readString = new StringBuilder(512);
            GetPrivateProfileString(lpAppName, lpKeyName, "", readString, (uint)readString.Capacity, filePath);
            return readString.ToString();
        }

        public int ReadInt(string lpAppName, string lpKeyName)
        {
            return (int)GetPrivateProfileInt(lpAppName, lpKeyName, 0, filePath);
        }

        public int ReadInt(string lpAppName, string lpKeyName, int defaultInt)
        {
            return (int)GetPrivateProfileInt(lpAppName, lpKeyName, defaultInt, filePath);
        }
    }

    class IniWriter
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);

        private string filePath;
        public IniWriter(string path)
        {
            filePath = path;
        }
        public bool Write(string lpAppName, string lpKeyName, string lpString)
        {
            return WritePrivateProfileString(lpAppName, lpKeyName, lpString, filePath);
        }

        public bool WriteInt(string lpAppName, string lpKeyName, int iInt)
        {
            return WritePrivateProfileString(lpAppName, lpKeyName, iInt.ToString(), filePath);
        }
    }
}
