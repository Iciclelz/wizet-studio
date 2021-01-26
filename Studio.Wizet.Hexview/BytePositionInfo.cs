namespace Studio.Wizet.Hexview
{
    /// <summary>
    /// Represents a position in the ByteEditor control
    /// </summary>
    struct BytePositionInfo
    {
        public BytePositionInfo(long index, int characterPosition)
        {
            Index = index;
            CharacterPosition = characterPosition;
        }

        public int CharacterPosition { get; }

        public long Index { get; }
    }
}
