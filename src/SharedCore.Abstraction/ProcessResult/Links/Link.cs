namespace SharedCore.Abstraction.ProcessResult.Links;

public sealed record Link(string Type, string Rel, string Href)
{
    public static Link Get(string rel, string href) => new(LinkType.Get.ToString(), rel, href);
    public static Link Post(string rel, string href) => new(LinkType.Post.ToString(), rel, href);
    public static Link Patch(string rel, string href) => new(LinkType.Patch.ToString(), rel, href);
    public static Link Put(string rel, string href) => new(LinkType.Put.ToString(), rel, href);
    public static Link Delete(string rel, string href) => new(LinkType.Delete.ToString(), rel, href);
}