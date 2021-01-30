using KTDU.Parser;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace KTDU.Interpreter
{
    public delegate Task<dynamic> ExpressionEvaluate(string expr);

    public delegate Task<ScriptLocation?> CallEntryHandler(CallEntry entry, ExpressionEvaluate evaluateFunc);

    internal struct ScriptFile
    {
        public readonly ImmutableDictionary<string, int> AnchorPositions;
        public readonly ImmutableArray<IEntry> Entries;

        public ScriptFile(string name, ImmutableArray<IEntry> entries)
        {
            Entries = entries;
            ImmutableDictionary<string, int>.Builder b = ImmutableDictionary.CreateBuilder<string, int>();
            for (int i = 0; i < Entries.Length; i++)
            {
                if (Entries[i] is AnchorEntry a)
                {
                    b[a.Name] = i;
                }
            }

            AnchorPositions = b.ToImmutable();
        }
    }

    public class Interpreter
    {
        private Dictionary<string, ScriptFile> _code;
        private ScriptLocation _pc;

        public ExpressionEvaluate ExpressionEvaluateFunction;
        public CallEntryHandler RuntimeCallHandler;
        public CallEntryHandler RuntimeTextHandler;

        private ScriptLocation ValidateLocation(ScriptLocation location)
        {
            string f = string.IsNullOrWhiteSpace(location.File) ? _pc.File : location.File;
            if (!_code.TryGetValue(f, out ScriptFile sf))
            {
                throw new FileNotFoundException($"script file {f} not found");
            }

            int p = location.Line;
            if (!string.IsNullOrWhiteSpace(location.Anchor))
            {
                if (!sf.AnchorPositions.TryGetValue(location.Anchor, out p))
                {
                    throw new KeyNotFoundException($"anchor {location.Anchor} not founded in file {f}");
                }
            }

            if (p >= sf.Entries.Length || p < 0)
            {
                throw new IndexOutOfRangeException($"location is set to file {f} line {p}, out of range");
            }

            return new ScriptLocation(f, p);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IEntry ReadEntry(ScriptLocation? location = null)
        {
            ScriptLocation l = location.HasValue ? ValidateLocation(location.Value) : _pc;
            return _code[l.File].Entries[l.Line];
        }

        public void Goto(ScriptLocation location)
        {
            _pc = ValidateLocation(location);
        }

        public async Task<bool> Step()
        {
            bool ret = false;
            IEntry entry = ReadEntry();
            ScriptLocation? next = null;
            if (entry is CommentEntry)
            {
            }
            else if (entry is AnchorEntry)
            {
            }
            else if (entry is TextEntry t)
            {
                CallEntry vce = new CallEntry("_text", new[]
                {
                    ("speaker", t.Speaker),
                    ("display", t.DisplayName),
                    ("content", t.Content),
                });
                next = await RuntimeTextHandler(vce, ExpressionEvaluateFunction);
            }
            else if (entry is CallEntry c)
            {
                // return at this point
                if (c.Name == "_exit")
                {
                    ret = true;
                }
                else
                {
                    next = await RuntimeCallHandler(c, ExpressionEvaluateFunction);
                }
            }
            else
            {
                throw new NotSupportedException();
            }

            if (!next.HasValue)
            {
                _pc.Step();
                if (_pc.Line >= _code[_pc.File].Entries.Length) throw new Exception("script reached eof");
            }
            else
            {
                _pc = ValidateLocation(next.Value);
            }

            return ret;
        }

        public async Task Run()
        {
            while (true)
            {
                bool yieldReturn = await Step();
                if (yieldReturn) break;
            }
        }
    }
}