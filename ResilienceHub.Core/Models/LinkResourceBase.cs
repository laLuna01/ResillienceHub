namespace ResilienceHub.Core.Models;

public abstract class LinkResourceBase
{
    public List<Link> Links { get; set; } = new List<Link>();
}

public class Link
{
    public string Href { get; set; }
    public string Rel { get; set; }
    public string Method { get; set; }

    public Link(string href, string rel, string method)
    {
        Href = href;
        Rel = rel;
        Method = method;
    }
}