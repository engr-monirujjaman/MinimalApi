using WebApi.DomainModels;

namespace WebApi.Seeds;

public static class CustomerDataSeed
{
    public static IEnumerable<Customer> Customers() => new List<Customer>
    {
        new()
        {
            Id = 1,
            FistName = "Rayhan",
            LastName = "Akond",
            Age = 19,
            EmailAddress = "rayhan@gmail.com"
        },
        new()
        {
            Id = 2,
            FistName = "Nahid",
            LastName = "Hossain",
            Age = 25,
            EmailAddress = "nahid@gmail.com"
        },
        new()
        {
            Id = 3,
            FistName = "Mohammad",
            LastName = "Monirujjaman",
            Age = 27,
            EmailAddress = "monirujjaman@gmail.com"
        },
        new()
        {
            Id = 4,
            FistName = "Rajin",
            LastName = "Khan",
            Age = 26,
            EmailAddress = "rajin@gmail.com"
        }
    };
}