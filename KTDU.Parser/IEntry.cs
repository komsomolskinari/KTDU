﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace KTDU.Parser
{
    public interface IEntry
    {
    }

    public struct CallEntry : IEntry
    {
        public string Name { get; }
        public ImmutableDictionary<string, string> Parameters { get; }
        public ImmutableArray<(string key, string value)> ParametersList { get; }

        public CallEntry(string name, IEnumerable<(string, string)> param)
        {
            ImmutableDictionary<string, string>.Builder d = ImmutableDictionary.CreateBuilder<string, string>();
            ImmutableArray<(string, string)>.Builder a = ImmutableArray.CreateBuilder<(string, string)>();

            foreach ((string k, string v) kv in param)
            {
                a.Add(kv);
                d.Add(kv.k, kv.v);
            }

            Name = name;
            Parameters = d.ToImmutable();
            ParametersList = a.ToImmutable();
        }

        public static CallEntry Parse(string line)
        {
            line = line.Trim();
            if (line.Length == 0) throw new FormatException();
            string raw;
            switch (line[0])
            {
                case '@':
                    raw = line.Substring(1);
                    break;

                case '[':
                    raw = line.Substring(1, line.Length - 2);
                    break;

                default:
                    throw new FormatException();
            }

            string s = raw.Trim();
            ParserState state = new ParserState(s);
            string name = state.ReadUntil(new[] {' '});
            state.Skip(new[] {' '});

            List<(string, string)> l = new List<(string, string)>();
            while (state.Next() != null)
            {
                // key key=val key="val"
                string key = state.ReadUntil(new[] {'=', ' '});
                if (string.IsNullOrWhiteSpace(key)) throw new FormatException();
                state.Skip(new[] {' '});
                if (state.Next() != '=')
                {
                    l.Add((key, null));
                    continue;
                }

                // eat =
                state.Advance();
                state.Skip(new[] {' '});
                string val;
                switch (state.Next())
                {
                    case '"':
                        state.Advance();
                        val = state.ReadUntil(new[] {'"'});
                        state.Advance();
                        break;

                    case '\'':
                        state.Advance();
                        val = state.ReadUntil(new[] {'\''});
                        state.Advance();
                        break;

                    case null:
                        throw new FormatException();

                    default:
                        val = state.ReadUntil(new[] {' '});
                        break;
                }

                state.Skip(new[] {' '});
                l.Add((key, val));
            }

            return new CallEntry(name, l);
        }
    }

    public struct AnchorEntry : IEntry
    {
        public string Name { get; private set; }

        private static readonly Regex AnchorRegex =
            new Regex(@"^\*(?<name>(?:\p{L}|_)+)(?:\|.*)?$", RegexOptions.CultureInvariant);

        public static AnchorEntry Parse(string line)
        {
            Match m = AnchorRegex.Match(line.Trim());
            if (!m.Success)
            {
                throw new FormatException();
            }

            return new AnchorEntry
            {
                Name = m.Groups["name"].Value,
            };
        }
    }

    public struct TextEntry : IEntry
    {
        public string Speaker { get; private set; }
        public string DisplayName { get; private set; }
        public string Content { get; private set; }

        private static readonly Regex TextRegex = new Regex(
            @"^(?:<(?<speaker>.+?)(?:\/(?<as>.+?))?>)?(?<text>[^\<].*)$",
            RegexOptions.CultureInvariant);

        private static readonly Regex TextRegex2 = new Regex(
            @"^(?:【(?<speaker>.+?)(?:\/(?<as>.+?))?】)?(?<text>[^【].*)$",
            RegexOptions.CultureInvariant);

        public static TextEntry Parse(string line)
        {
            string l = line.Trim();
            if (string.IsNullOrWhiteSpace(l)) throw new FormatException();
            Regex re = l[0] == '<' ? TextRegex : TextRegex2;
            Match m = re.Match(l);
            if (!m.Success) throw new FormatException();

            string d = m.Groups["speaker"].Value;
            string r = m.Groups["as"].Value;
            string t = m.Groups["text"].Value;
            if (string.IsNullOrWhiteSpace(r)) r = d;
            return new TextEntry
            {
                Speaker = d.Trim(),
                DisplayName = r.Trim(),
                Content = t.Trim(),
            };
        }
    }

    public struct CommentEntry : IEntry
    {
        public string Comment { get; private set; }

        public static CommentEntry Parse(string line)
        {
            line = line.Trim();
            if (!line.StartsWith(";")) throw new FormatException();
            return new CommentEntry
            {
                Comment = line.Substring(1).Trim(),
            };
        }
    }
}