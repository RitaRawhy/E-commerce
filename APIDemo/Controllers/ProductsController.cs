using APIDemo.Dtos;
using APIDemo.Helper;
using APIDemo.ResponseModule;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIDemo.Controllers
{
    public class ProductsController : BaseController
    {
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<ProductBrand> _productBrandRepository;
        private readonly IGenericRepository<ProductType> _productTypeRepository;
        private readonly IMapper _mapper;

        //private readonly IProductRepository _productRepository;

        public ProductsController(
            //IProductRepository productRepository
            IGenericRepository<Product> productRepository,
            IGenericRepository<ProductBrand> productBrandRepository,
            IGenericRepository<ProductType> productTypeRepository ,
            IMapper mapper )
        {
            //_productRepository = productRepository;
            _productRepository = productRepository;
            _productBrandRepository = productBrandRepository;
            _productTypeRepository = productTypeRepository;
            _mapper = mapper;
        }

        [Cached(100)]
        [HttpGet("GetProducts")]
        public async Task<ActionResult<Pagination<ProductDto>>> GetProducts([FromQuery] productSpecParams productSpec)
        {
            var spec = new ProductsWithTypeAndBrandSpecifications(productSpec);

            var countSpec = new productsWithFilterForCountSpecification(productSpec);

            var totatProducts = await _productRepository.CountAsync(countSpec); 

            var products = await _productRepository.ListAsync(spec);

            var mappedProducts = _mapper.Map<IReadOnlyList<ProductDto>>(products);

            var paginationData = new Pagination<ProductDto>(productSpec.PageIndex, productSpec.PageSize, totatProducts, mappedProducts);

            return Ok(paginationData);
        }

        [HttpGet("GetProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse),StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var spec = new ProductsWithTypeAndBrandSpecifications(id);
            var product = await _productRepository.GetEntityWithSpecifications(spec);

            if (product is null)
                return NotFound(new ApiResponse(404));

            var mappedProducts = _mapper.Map<ProductDto>(product);

            return Ok(mappedProducts);
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            return Ok(await _productBrandRepository.ListAllAsync());            
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GeyProductTypes()
        {
            return Ok(await _productTypeRepository.ListAllAsync());
        }
    }
}
