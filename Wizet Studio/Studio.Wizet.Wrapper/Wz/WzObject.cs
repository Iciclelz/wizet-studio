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

namespace Wizet_Studio.Studio.Wizet.Wrapper.Wz
{
    class WzObject
    {
        public enum WizetObject
        {
            WizetFile = 0,
            WizetDirectory = 1,
            WizetImage = 2,
            WizetNullProperty = 3,
            WizetSubProperty = 4,
            WizetConvexProperty = 5,
            WizetCanvasProperty = 6,
            WizetSoundProperty = 7,
            WizetCompressedIntProperty = 8,
            WizetDoubleProperty = 9,
            WizetByteFloatProperty = 10,
            WizetUnsignedShortProperty = 11,
            WizetUolProperty = 12,
            WizetStringProperty = 13,
            WizetVectorProperty = 14,
            Null = 15
        };

        public static WizetObject GetWizetObject(IWzObject WzObject)
        {
            if (WzObject is WzFile)
            {
                return WizetObject.WizetFile;
            }
            else if (WzObject is WzDirectory)
            {
                return WizetObject.WizetDirectory;
            }
            else if (WzObject is WzImage)
            {
                return WizetObject.WizetImage;
            }
            else if (WzObject is WzNullProperty)
            {
                return WizetObject.WizetNullProperty;
            }
            else if (WzObject is WzSubProperty)
            {
                return WizetObject.WizetSubProperty;
            }
            else if (WzObject is WzConvexProperty)
            {
                return WizetObject.WizetConvexProperty;
            }
            else if (WzObject is WzCanvasProperty)
            {
                return WizetObject.WizetCanvasProperty;
            }
            else if (WzObject is WzSoundProperty)
            {
                return WizetObject.WizetSoundProperty;
            }
            else if (WzObject is WzCompressedIntProperty)
            {
                return WizetObject.WizetCompressedIntProperty;
            }
            else if (WzObject is WzDoubleProperty)
            {
                return WizetObject.WizetDoubleProperty;
            }
            else if (WzObject is WzByteFloatProperty)
            {
                return WizetObject.WizetByteFloatProperty;
            }
            else if (WzObject is WzUnsignedShortProperty)
            {
                return WizetObject.WizetUnsignedShortProperty;
            }
            else if (WzObject is WzUOLProperty)
            {
                return WizetObject.WizetUolProperty;
            }
            else if (WzObject is WzStringProperty)
            {
                return WizetObject.WizetStringProperty;
            }
            else if (WzObject is WzVectorProperty)
            {
                return WizetObject.WizetVectorProperty;
            }

            return WizetObject.Null;

        }
    }
}
