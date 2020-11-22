using CopyDep.Services;
using CopyDep.Services.CopyWorker;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CopyDep
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }
        //public IConfiguration Configuration { get; private set; }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            //+ config
            //var builder = new ConfigurationBuilder()
            // .SetBasePath(Directory.GetCurrentDirectory())
            // .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            //Configuration = builder.Build();
            //+ provider
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();
            //+ run
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }


        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ICopyWorkerService, CopyWorkerService>();

            services.AddTransient(typeof(MainWindow));
        }


    }
}
