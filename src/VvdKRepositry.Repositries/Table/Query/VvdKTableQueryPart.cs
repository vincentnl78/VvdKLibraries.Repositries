using System.Globalization;

namespace VvdKRepositry.Repositries.Table.Query;

public class VvdKTableQueryPart(string name, VvdKTableQueryPart.Comparison comparison, string value)
{
    public enum Comparison
    {
        Equal,
        Greater,
        GreaterEqual,
        Less,
        LessEqual
    }

    private readonly string _value = "'" + value + "'";

    public VvdKTableQueryPart(string name, Comparison comparison, double value)
        : this(name, comparison, value.ToString(CultureInfo.InvariantCulture))
    {
        _value = value.ToString(CultureInfo.InvariantCulture);
    }

    public VvdKTableQueryPart(string name, Comparison comparison, int value)
        : this(name, comparison, value.ToString())
    {
        _value = value.ToString();
    }

    public VvdKTableQueryPart(string name, Comparison comparison, decimal value)
        : this(name, comparison, value.ToString(CultureInfo.InvariantCulture))
    {
        _value = value.ToString(CultureInfo.InvariantCulture);
    }

    public VvdKTableQueryPart(string name, Comparison comparison, DateOnly value)
        : this(name, comparison, value.ToString())
    {
        _value = "'" + value + "'";
    }

    public VvdKTableQueryPart(string name, Comparison comparison, DateTimeOffset value)
        : this(name, comparison, value.ToString())
    {
        _value = "'" + value + "'";
    }

    public Comparison Compare { get; } = comparison;

    public string Name { get; } = name;

    public override string ToString()
    {
        var sign = Compare switch
        {
            Comparison.Equal => "eq",
            Comparison.LessEqual => "le",
            Comparison.Less => "lt",
            Comparison.Greater => "gt",
            Comparison.GreaterEqual => "ge",
            _ => throw new ArgumentException("QueryPart VvdKAzure")
        };
        return $"{Name} {sign} {_value}";
    }

    public static VvdKTableQueryPart Partition(Comparison c, string p)
    {
        return new VvdKTableQueryPart("PartitionKey", c, p);
    }

    public static VvdKTableQueryPart RowKey(Comparison c, string p)
    {
        return new VvdKTableQueryPart("RowKey", c, p);
    }
}