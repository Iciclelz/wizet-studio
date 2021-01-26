﻿/*
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
using System.IO;

namespace Wizet_Studio.Studio.Wizet.Windows.Components
{
    class NewWzFile
    {
        private string filePath;
        public NewWzFile(string filePath)
        {
            this.filePath = filePath;
        }

        public void Create()
        {
            byte[] newFileBytes = new byte[]
            {
                0x50 ,0x4B ,0x47 ,0x31 ,0x68 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x00 ,0x1D ,0x00 ,0x00 ,0x00 ,0x4E ,0x65 ,0x77 ,0x57 ,0x7A ,0x46 ,0x69 ,0x6C ,0x65 ,0x2E ,0x77 ,0x7A ,0x00 ,0x88 ,0x00 ,0x01 ,0x04 ,0xF2 ,0xEE ,0xCE ,0xCA ,0xCC ,0xDB ,0xC3 ,0xC4 ,0xF8 ,0xDF ,0xD4 ,0x9A ,0xDC ,0xDB ,0xD0 ,0x4B ,0x80 ,0xBF ,0x2C ,0x00 ,0x00 ,0x1D ,0x8F ,0x1F ,0x2A ,0x73 ,0xF8 ,0xFA ,0xD9 ,0xC3 ,0xDD ,0xCB ,0xDD ,0xC4 ,0xC8 ,0x00 ,0x00 ,0x02 ,0x00 ,0xF6 ,0xEE ,0xCE ,0xCA ,0xCC ,0xDB ,0xC3 ,0xC4 ,0xF8 ,0xDC ,0xC7 ,0x03 ,0x00 ,0x00 ,0xF3 ,0xEE ,0xCE ,0xCA ,0xCC ,0xDB ,0xC3 ,0xC4 ,0xE7 ,0xD7 ,0xD0 ,0xC0 ,0xDA ,0xC4 ,0x09 ,0x1C ,0x00 ,0x00 ,0x00 ,0x73 ,0xF0 ,0xF9 ,0xC3 ,0xCD ,0xDD ,0xCB ,0x9D ,0xF4 ,0x92 ,0xE4 ,0xD6 ,0xD7 ,0xC1 ,0xD9 ,0xC5 ,0x8A ,0xFD ,0x80 ,0x20 ,0x03 ,0x00 ,0x00 ,0x80 ,0x58 ,0x02 ,0x00 ,0x00
            };
            File.WriteAllBytes(filePath, newFileBytes);
        }

        
    }
}
