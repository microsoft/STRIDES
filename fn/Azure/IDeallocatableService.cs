public interface IDeallocatableService
{
    Task Down(string name);
    Task Up(string name);
}