using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace Invoicing
{
    public class Program
    {
        static readonly List<Product> productsList;
        static readonly List<string> VAT_exmpeted_Categories;

        static Program()
        {
            IConfigurationRoot configuration = ConfigureServices();
            productsList = configuration.GetSection("products").Get<List<Product>>();
            VAT_exmpeted_Categories = new List<string>() { "food", "toys", "medicines" };
        }

        static void Main(string[] args)
        {
            Invoice invoice = new Invoice(productsList, VAT_exmpeted_Categories);
            var text = File.ReadAllLines("Files/input.txt");
            invoice.PrintInvoice(text);
        }


        private static IConfigurationRoot ConfigureServices()
        {
            return new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
        }
    }
}
