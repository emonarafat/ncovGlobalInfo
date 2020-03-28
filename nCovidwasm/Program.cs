using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
namespace ncovid
{
    using System.Net.Http;
    using NCovid.Service.AutoMapper;
    using Service;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            builder.Services.AddAutoMapper(typeof(AutoMapping));
            builder.Services.AddTransient<ApiService>(provider => new ApiService(new HttpClient()
                {BaseAddress = new Uri("https://localhost:44357")}));
            builder.Services.AddBaseAddressHttpClient();

            await builder.Build().RunAsync();
        }
    }
}
