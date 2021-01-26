using System;

namespace Studio.Wizet.Hexview
{
    internal abstract class DataBlock
    {
        public abstract long Length
        {
            get;
        }

        public DataMap Map
        {
            get; set;
        }

        public DataBlock NextBlock
        {
            get; set;
        }

        public DataBlock PreviousBlock
        {
            get; set;
        }

        public abstract void RemoveBytes(long position, long count);
    }
}
