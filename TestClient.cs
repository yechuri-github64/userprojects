using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services;
using Models;

class TestClient
{
    static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("local.settings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var host = new HostBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<IConfiguration>(config);
                services.AddSingleton<IAccountService, AccountService>();
                services.AddLogging(builder =>
                {
                    builder.AddConsole();
                    builder.SetMinimumLevel(LogLevel.Information);
                });
            })
            .Build();

        var accountService = host.Services.GetRequiredService<IAccountService>();

        Console.WriteLine("\n=== Testing Account Service ===\n");

        // Test 1: List Accounts
        Console.WriteLine("TEST 1: List Accounts");
        var accounts = await accountService.ListAccountsAsync();
        Console.WriteLine($"✓ Found {accounts.Count} accounts");
        foreach (var acc in accounts)
        {
            Console.WriteLine($"  - ID: {acc.Id}, Name: {acc.Name}, Email: {acc.Email}");
        }

        // Test 2: Get Account
        Console.WriteLine("\nTEST 2: Get Account");
        var firstId = accounts[0].Id;
        var account = await accountService.GetAccountAsync(firstId);
        if (account != null)
        {
            Console.WriteLine($"✓ Retrieved account: {account.Name} ({account.Id})");
        }

        // Test 3: Create Account
        Console.WriteLine("\nTEST 3: Create Account");
        var newAccounts = new List<Account>
        {
            new Account { Name = "Test Company", Email = "test@company.com", Address = "100 Test St" }
        };
        var created = await accountService.CreateAccountsAsync(newAccounts);
        Console.WriteLine($"✓ Created {created.Count} account(s)");
        var newId = created[0].Id;
        Console.WriteLine($"  New Account ID: {newId}");

        // Test 4: Update Account
        Console.WriteLine("\nTEST 4: Update Account");
        var updateData = new Account { Name = "Updated Test Company", Email = "updated@company.com", Address = "200 Test Ave" };
        var updated = await accountService.UpdateAccountAsync(newId, updateData);
        if (updated != null)
        {
            Console.WriteLine($"✓ Updated account: {updated.Name}");
        }

        // Test 5: Delete Account
        Console.WriteLine("\nTEST 5: Delete Account");
        var deleted = await accountService.DeleteAccountAsync(newId);
        Console.WriteLine($"✓ Delete result: {(deleted ? "Success" : "Failed")}");

        // Test 6: Verify Deletion
        Console.WriteLine("\nTEST 6: Verify Deletion");
        var notFound = await accountService.GetAccountAsync(newId);
        if (notFound == null)
        {
            Console.WriteLine($"✓ Account successfully deleted (no longer found)");
        }

        Console.WriteLine("\n=== All Tests Complete ===\n");
    }
}
