using System;
using System.Collections.Generic;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure.Exceptions.Services;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Services
{
    public class ProductService : IProductService
    {
        private static readonly IDictionary<string, Product> products = new Dictionary<string, Product> {
            {
                "MonthlyKatusha", new Product() {
                    Id = 1,
                    Guid = Guid.Parse("1000b22b-0683-4dd6-ac2f-396046fc0001"),
                    FriendlyName = "MonthlyKatusha",

                    Name = "Katusha Membership for 30 days",
                    Description = "Your monthly subscription for MS.Katusha",
                    Notes = "Ms.Katusha notes",
                    RecurringInformation = "Every month MS.Katusha",
                    Amount = "9.95",
                    Tax = "0.00",
                    ExecutionData = "{Value:31,TimeFrame:3,Recurring:false,Membership:3}",

                    CreationDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    Deleted = false,
                    DeletionDate = new DateTime(1900, 1, 1),
                }
            }
        };

        public Product GetProductByName(ProductNames productName)
        {
            if (!products.ContainsKey(productName.ToString()))
                throw new KatushaProductNameNotFoundException(productName.ToString());
            return products[productName.ToString()];
        }
    }

}
