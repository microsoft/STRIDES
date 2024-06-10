public class DeallocatableResourceResponse
{
    public string? Action { get; set; }
    public int ResourceCount { get; set; }
    public List<DeallocatableResource>? Resources { get; set; }
    public string? Error { get; set; }
}