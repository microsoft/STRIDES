public interface IDeallocatableService
{
    Task Down(string name, string resourceGroup);
    Task Up(string name, string resourceGroup);
}