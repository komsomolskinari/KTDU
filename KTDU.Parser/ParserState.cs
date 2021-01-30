using System.Linq;
using System.Text.RegularExpressions;

namespace KTDU.Parser
{
    public struct ParserState
    {
        private readonly string _text;
        private int _pointer;

        public ParserState(string text)
        {
            _text = new Regex("\r\n?").Replace(text, "\n");
            _pointer = 0;
        }

        public char? Next()
        {
            if (_pointer >= _text.Length) return null;
            return _text[_pointer];
        }

        public char? Advance()
        {
            char? r = Next();
            if (r != null) _pointer++;
            return r;
        }

        public string ReadLine()
        {
            string r = ReadUntil(new[] {'\n'});
            _pointer++;
            return r;
        }

        public string ReadUntil(char[] c)
        {
            int eol = _text.IndexOfAny(c, _pointer);
            string line = eol == -1
                ? _text.Substring(_pointer)
                : _text.Substring(_pointer, eol - _pointer);
            int newPtr = eol == -1
                ? _text.Length
                : eol;
            _pointer = newPtr;
            return line;
        }

        public void Skip(char[] c)
        {
            while (true)
            {
                char? next = Next();
                if (next == null) return;
                if (!c.Contains(next.Value)) return;
                Advance();
            }
        }
    }
}