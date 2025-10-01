namespace Web.Models;

public record Actor(int Id, string Name, int BirthYear)
{
    public int Age => DateTime.Now.Year - BirthYear;
}
