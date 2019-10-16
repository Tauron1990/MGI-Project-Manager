using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tauron.MgiManager.User.Service.Data;
using Tauron.ServiceBootstrapper;
using Tauron.SqliteDataBaseHelper;

namespace Tauron.MgiManager.User.Service
{
    class Program
    {
        static async Task Main(string[] args) 
            => await BootStrapper.Run<Program>(args, config: sc => sc.AddDefaultDatabase<UserDatabase>());


    }
}
