using System;
using System.Collections;
using System.Text;

namespace Studio.Wizet.Hexview
{
    internal class DataMap : ICollection, IEnumerable
    {
        readonly object _syncRoot = new object();
        internal int _count;
        internal DataBlock _firstBlock;
        internal int _version;

        public DataMap()
        {
        }

        public DataMap(IEnumerable collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            foreach (DataBlock item in collection)
            {
                AddLast(item);
            }
        }

        public DataBlock FirstBlock
        {
            get
            {
                return _firstBlock;
            }
        }

        public void AddAfter(DataBlock block, DataBlock newBlock)
        {
            AddAfterInternal(block, newBlock);
        }

        public void AddBefore(DataBlock block, DataBlock newBlock)
        {
            AddBeforeInternal(block, newBlock);
        }

        public void AddFirst(DataBlock block)
        {
            if (_firstBlock == null)
            {
                AddBlockToEmptyMap(block);
            }
            else
            {
                AddBeforeInternal(_firstBlock, block);
            }
        }

        public void AddLast(DataBlock block)
        {
            if (_firstBlock == null)
            {
                AddBlockToEmptyMap(block);
            }
            else
            {
                AddAfterInternal(GetLastBlock(), block);
            }
        }

        public void Remove(DataBlock block)
        {
            RemoveInternal(block);
        }

        public void RemoveFirst()
        {
            if (_firstBlock == null)
            {
                throw new InvalidOperationException("The collection is empty.");
            }
            RemoveInternal(_firstBlock);
        }

        public void RemoveLast()
        {
            if (_firstBlock == null)
            {
                throw new InvalidOperationException("The collection is empty.");
            }
            RemoveInternal(GetLastBlock());
		}

		public DataBlock Replace(DataBlock block, DataBlock newBlock)
		{
			AddAfterInternal(block, newBlock);
			RemoveInternal(block);
			return newBlock;
		}

        public void Clear()
        {
            DataBlock block = FirstBlock;
            while (block != null)
            {
                DataBlock nextBlock = block.NextBlock;
                InvalidateBlock(block);
                block = nextBlock;
            }
            _firstBlock = null;
            _count = 0;
            _version++;
        }

        void AddAfterInternal(DataBlock block, DataBlock newBlock)
        {
            newBlock.PreviousBlock = block;
            newBlock.NextBlock = block.NextBlock;
            newBlock.Map = this;

            if (block.NextBlock != null)
            {
                block.NextBlock.PreviousBlock = newBlock;
            }
            block.NextBlock = newBlock;

            this._version++;
            this._count++;
        }

        void AddBeforeInternal(DataBlock block, DataBlock newBlock)
        {
            newBlock.NextBlock = block;
            newBlock.PreviousBlock = block.PreviousBlock;
            newBlock.Map = this;

            if (block.PreviousBlock != null)
            {
                block.PreviousBlock.NextBlock = newBlock;
            }
            block.PreviousBlock = newBlock;

            if (_firstBlock == block)
            {
                _firstBlock = newBlock;
            }
            this._version++;
            this._count++;
        }

        void RemoveInternal(DataBlock block)
        {
            DataBlock previousBlock = block.PreviousBlock;
            DataBlock nextBlock = block.NextBlock;

            if (previousBlock != null)
            {
                previousBlock.NextBlock = nextBlock;
            }

            if (nextBlock != null)
            {
                nextBlock.PreviousBlock = previousBlock;
            }

            if (_firstBlock == block)
            {
                _firstBlock = nextBlock;
            }

            InvalidateBlock(block);

            _count--;
            _version++;
        }

        DataBlock GetLastBlock()
        {
            DataBlock lastBlock = null;
            for (DataBlock block = FirstBlock; block != null; block = block.NextBlock)
            {
                lastBlock = block;
            }
            return lastBlock;
        }

        void InvalidateBlock(DataBlock block)
        {
            block.Map = null;
            block.NextBlock = null;
            block.PreviousBlock = null;
        }

        void AddBlockToEmptyMap(DataBlock block)
        {
            block.Map = this;
            block.NextBlock = null;
            block.PreviousBlock = null;

            _firstBlock = block;
            _version++;
            _count++;
        }

        #region ICollection Members
        public void CopyTo(Array array, int index)
        {
            DataBlock[] blockArray = array as DataBlock[];
            for (DataBlock block = FirstBlock; block != null; block = block.NextBlock)
            {
                blockArray[index++] = block;
            }
        }

        public int Count
        {
            get
            {
                return _count;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public object SyncRoot
        {
            get
            {
                return _syncRoot;
            }
        }
        #endregion

        #region IEnumerable Members
        public IEnumerator GetEnumerator()
        {
            return new Enumerator(this);
        }
        #endregion

        #region Enumerator Nested Type
        internal class Enumerator : IEnumerator, IDisposable
        {
            DataMap Map;
            DataBlock _current;
            int _index;
            int _version;

            internal Enumerator(DataMap map)
            {
                Map = map;
                _version = map._version;
                _current = null;
                _index = -1;
            }

            object IEnumerator.Current
            {
                get
                {
                    if (_index < 0 || _index > Map.Count)
                    {
                        throw new InvalidOperationException("Enumerator is positioned before the first element or after the last element of the collection.");
                    }
                    return _current;
                }
            }

            public bool MoveNext()
            {
                if (this._version != Map._version)
                {
                    throw new InvalidOperationException("Collection was modified after the enumerator was instantiated.");
                }

                if (_index >= Map.Count)
                {
                    return false;
                }

                if (++_index == 0)
                {
                    _current = Map.FirstBlock;
                }
                else
                {
                    _current = _current.NextBlock;
                }

                return (_index < Map.Count);
            }

            void IEnumerator.Reset()
            {
                if (this._version != this.Map._version)
                {
                    throw new InvalidOperationException("Collection was modified after the enumerator was instantiated.");
                }

                this._index = -1;
                this._current = null;
            }

            public void Dispose()
            {
            }
        }
        #endregion
    }
}
