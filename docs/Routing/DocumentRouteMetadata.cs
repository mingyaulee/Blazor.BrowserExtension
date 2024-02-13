namespace Docs.Routing
{
    public class DocumentRouteMetadata
    {
        public required string FileName { get; init; }
        public required string Name { get; init; }
        public required string WebPath { get; init; }
        public required int Depth { get; init; }
        public required string ResourceName { get; init; }
    }
}
