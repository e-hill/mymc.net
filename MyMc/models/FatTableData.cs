namespace MyMc.Models;

public class FatTableData
{
    public List<List<int>> AllocatableClusters { get; set; } = [];

    public override string ToString()
    {
        return "{" + string.Join("}, {", AllocatableClusters.Select(x => string.Join(", ", x))) + "}";
    }
}