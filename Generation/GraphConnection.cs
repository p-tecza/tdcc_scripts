public class GraphConnection
{
    public int parentNode { get; set; }
    public int childNode { get; set; }
    public int weight { get; set; }
    public GraphConnection(int parentNode, int childNode, int weight)
    {
        this.weight = weight;
        this.parentNode = parentNode;
        this.childNode = childNode;
    }

    public override string ToString()
    {
        return "|PATH: " + parentNode + "-" + childNode + " W:" + weight + "|\n";
    }

}