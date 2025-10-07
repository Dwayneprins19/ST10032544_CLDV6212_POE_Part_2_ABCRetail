using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Azure.Storage.Files.Shares;
using ABCRetail.Services;

namespace ABCRetail
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            string connectionString = builder.Configuration["AzureStorage:ConnectionString"];
            if (string.IsNullOrEmpty(connectionString)) 
            {
                throw new InvalidOperationException("AzureStorage:ConnectionString is missing from configuration.");
            }

            builder.Services.AddSingleton(new BlobServiceClient(connectionString)); 
			builder.Services.AddSingleton(new QueueServiceClient(connectionString)); 
			builder.Services.AddSingleton(new ShareServiceClient(connectionString)); 
			builder.Services.AddSingleton(new TableServiceClient(connectionString)); 

			builder.Services.AddScoped<BlobStorageService>();
            builder.Services.AddScoped<CustomerTableService>();
            builder.Services.AddScoped<ProductTableService>();
            builder.Services.AddScoped<QueueService>(); 
            builder.Services.AddScoped<FileShareService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
