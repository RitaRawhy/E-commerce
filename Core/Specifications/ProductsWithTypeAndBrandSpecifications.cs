using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class ProductsWithTypeAndBrandSpecifications :BaseSpecification<Product>
    {
        public ProductsWithTypeAndBrandSpecifications(productSpecParams productSpecParams) 
            : base(product =>
            (string.IsNullOrEmpty(productSpecParams.Search) || product.Name.ToLower().Contains(productSpecParams.Search))
            &&(!productSpecParams.BrandId.HasValue || product.ProductBrandId == productSpecParams.BrandId)
            && (!productSpecParams.TypeId.HasValue || product.ProductTypeId == productSpecParams.TypeId))
        {
            AddInclude(product => product.ProductType);
            AddInclude(product => product.ProductBrand);
            AddOrederBy(product => product.Name);
            ApplyPanging(productSpecParams.PageSize * (productSpecParams.PageIndex-1),productSpecParams.PageSize);

            if(!string.IsNullOrEmpty(productSpecParams.Sort))
            {
                switch(productSpecParams.Sort)
                {
                    case "priceAsc":
                        AddOrederBy(product => product.Price);                  
                        break;
                    case "priceDesc":
                        AddOrederByDescending(product => product.Price);
                        break;
                    default:
                        AddOrederBy(product => product.Name);
                        break;
                }
            }
        }

        public ProductsWithTypeAndBrandSpecifications(int id)
            :base(product => product.Id==id)
        {
            AddInclude(product => product.ProductType);
            AddInclude(product => product.ProductBrand);
        }
    }
}
