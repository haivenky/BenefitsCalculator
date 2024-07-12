namespace Api.Models;

public class Employee
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public decimal Salary { get; set; }
    public DateTime DateOfBirth { get; set; }
    public ICollection<Dependent> Dependents { get; set; } = new List<Dependent>();
    public int Age => CalculateAge(DateOfBirth);

    private int CalculateAge(DateTime dateOfBirth)
    {
        var today = DateTime.Today;
        var age = today.Year - dateOfBirth.Year;

        if (dateOfBirth.Date > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }

    public bool HasValidDependents()
    {
        bool hasSpouse = Dependents.Any(d => d.Relationship == Relationship.Spouse);
        bool hasDomesticPartner = Dependents.Any(d => d.Relationship == Relationship.DomesticPartner);
        return !(hasSpouse && hasDomesticPartner);
    }
}
