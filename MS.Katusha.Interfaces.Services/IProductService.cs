using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Interfaces.Services
{
    public interface IProductService
    {
        Product GetProductByName(ProductNames productName);
    }
}