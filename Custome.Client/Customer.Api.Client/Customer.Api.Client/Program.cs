using Customer.Api.Client;

public class Program
{
    static async Task Main()
    {
        var api = new Helper("http://localhost:8080/");

        while (true)
        {
            Console.WriteLine("\n=== Customer CLI ===");
            Console.WriteLine("1. List Customers");
            Console.WriteLine("2. Create Customer");
            Console.WriteLine("3. Update Customer");
            Console.WriteLine("4. Delete Customer");
            Console.WriteLine("5. Exit");
            Console.Write("Choose an option: ");
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    var customers = await api.GetAllAsync();
                    if (customers.Any())
                        foreach (var c in customers)
                            Console.WriteLine($"{c.Id}: {c.FirstName} {c.LastName} ({c.Email})");
                    else
                        Console.WriteLine("No customers found.");
                    break;

                case "2":
                    var newCustomer = new CustomerDto
                    {
                        FirstName = Prompt("First Name"),
                        MiddleName = Prompt("Middle Name (optional)"),
                        LastName = Prompt("Last Name"),
                        Email = Prompt("Email"),
                        PhoneNumber = Prompt("Phone")
                    };
                    var created = await api.CreateAsync(newCustomer);
                    Console.WriteLine(created != null
                        ? $"Created: {created.Id}"
                        : "Failed to create.");
                    break;

                case "3":
                    var updateId = Guid.Parse(Prompt("Enter ID to update"));
                    var existing = await api.GetByIdAsync(updateId);
                    if (existing is null) { Console.WriteLine("Not found."); break; }

                    existing.FirstName = Prompt("First Name", existing.FirstName);
                    existing.MiddleName = Prompt("Middle Name", existing.MiddleName);
                    existing.LastName = Prompt("Last Name", existing.LastName);
                    existing.Email = Prompt("Email", existing.Email);
                    existing.PhoneNumber = Prompt("Phone", existing.PhoneNumber);

                    var updated = await api.UpdateAsync(existing);
                    Console.WriteLine(updated ? "Updated." : "Failed.");
                    break;

                case "4":
                    var delId = Guid.Parse(Prompt("Enter ID to delete"));
                    var deleted = await api.DeleteAsync(delId);
                    Console.WriteLine(deleted ? "Deleted." : "Failed.");
                    break;

                case "5":
                    return;

                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
    }

    static string Prompt(string label, string? defaultVal = null)
    {
        Console.Write($"{label}{(defaultVal is not null ? $" [{defaultVal}]" : "")}: ");
        var input = Console.ReadLine();
        return string.IsNullOrWhiteSpace(input) && defaultVal is not null ? defaultVal : input!;
    }
}