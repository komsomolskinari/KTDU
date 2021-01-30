using System.Collections.Immutable;

namespace KTDU.Parser
{
    public static class Parser
    {
        public static ImmutableArray<IEntry> Parse(string text)
        {
            ImmutableArray<IEntry>.Builder builder = ImmutableArray.CreateBuilder<IEntry>();
            ParserState state = new ParserState(text);
            while (state.Next() != null)
            {
                string line = state.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;
                switch (line[0])
                {
                    case ';':
                        builder.Add(CommentEntry.Parse(line));
                        break;

                    case '*':
                        builder.Add(AnchorEntry.Parse(line));
                        break;

                    case '[':
                    case '@':
                        builder.Add(CallEntry.Parse(line));
                        break;

                    default:
                        builder.Add(TextEntry.Parse(line));
                        break;
                }
            }

            return builder.ToImmutable();
        }
    }
}