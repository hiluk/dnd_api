namespace DndSolution.Neccessary;

public interface IPasswordHasher
{
    public string Generate(string password);

    bool Verify(string password, string hasherdPassword);
}