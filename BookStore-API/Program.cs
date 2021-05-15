using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using System.Text;

namespace BookStore_API {
  public class Program {
    public static void Main(string[] args) {

      // Simple way, and inline way, to connect to SQL db
      // Uses NuGet Microsoft.Data.SqlClient
      // See the Startup file where we will use the Microsoft.EntityFrameworkCore instead

      //try {
      //  SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
      //  builder.DataSource = "emilsqlserver.database.windows.net";
      //  builder.UserID = "emiladmin";
      //  builder.Password = "EMilPassword12!";
      //  builder.InitialCatalog = "mySampleDatabase";

      //  using (SqlConnection connection = new SqlConnection(builder.ConnectionString)) {
      //    Console.WriteLine("\nQuery data example:");
      //    Console.WriteLine("=========================================\n");

      //    String sql = "SELECT name, collation_name FROM sys.databases";

      //    using (SqlCommand command = new SqlCommand(sql, connection)) {
      //      connection.Open();
      //      using (SqlDataReader reader = command.ExecuteReader()) {
      //        while (reader.Read()) {
      //          Console.WriteLine("{0} {1}", reader.GetString(0), reader.GetString(1));
      //        }
      //      }
      //    }
      //  }
      //}
      //catch (SqlException e) {
      //  Console.WriteLine(e.ToString());
      //}

      //Console.ReadLine(); // This pauses the entire program and waits for user input on the console

      CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => {
              webBuilder.UseStartup<Startup>();
            });
  }
}
