using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tauron.SqliteDataBaseHelper
{
    public static class Helper
    {
        public static Task AddDefaultDatabase<TDataBase>(this IServiceCollection sc) 
            where TDataBase : DbContext
        {
            sc.AddDbContext<TDataBase>((provider, builder) =>
                                          {
                                              var config = provider.GetRequiredService<IConfiguration>();
                                              var conBuilder = new SqliteConnectionStringBuilder
                                                               {
                                                                   DataSource = Path.Combine(config.GetValue<string>("Root"), config.GetValue<string>("ConnectionString")),
                                                                   Cache = SqliteCacheMode.Shared
                                                               };

                                              builder.UseSqlite(conBuilder.ConnectionString);
                                          });
            return Task.CompletedTask;
        }

        private static string GetRoot(string value)
        {
            return value switch
            {
                "ApplicationData" => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "LocalApplicationData" => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                _ => Environment.GetEnvironmentVariable(value) ?? Environment.CurrentDirectory,
            };
        }
    }
}
