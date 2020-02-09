namespace Terka.TinyFonts
{
    using System.Collections;

    internal class CharacterRangeComparer : IComparer
    {
        public static readonly CharacterRangeComparer Instance = new CharacterRangeComparer();

        public int Compare(object x, object y)
        {
            if (x is CharacterRangeDescription)
                return Compare((CharacterRangeDescription)x, (char)y);
            else
                return Compare((char)x, (CharacterRangeDescription)y);
        }

        public int Compare(CharacterRangeDescription range, char character)
        {
            if (character < range.FirstCharacter) return 1;
            if (character > range.LastCharacter) return -1;
            return 0;
        }

        public int Compare(char character, CharacterRangeDescription range)
        {
            if (character < range.FirstCharacter) return -1;
            if (character > range.LastCharacter) return 1;
            return 0;
        }
    }
}
