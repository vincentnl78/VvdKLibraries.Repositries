using System.Text;

namespace VvdKRepositry.Repositries.Table.Query;

public class VvdKTableQuery(VvdKTableQuery.ComboType c)
{
    public enum ComboType
    {
        And,
        Or
    }

    public ComboType Combo { get; } = c;

    public List<VvdKTableQueryPart> Parts { get; } = [];

    public List<VvdKTableQuery> Queries { get; } = [];

    public override string ToString()
    {
        StringBuilder sb = new();
        var combo = Combo == ComboType.And
            ? " and "
            : " or ";

        var first = true;
        foreach (var part in Parts)
        {
            if (!first) sb.Append(combo);

            sb.Append(part);
            first = false;
        }

        foreach (var q in Queries)
        {
            if (!first) sb.Append(combo);

            first = false;
            if (q.Combo == Combo)
                sb.Append($" {q}");
            else
                sb.Append($" ({q})");
        }

        return sb.ToString();
    }
}