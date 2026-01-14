using AutoMapper;
using ECommerce.Application.DTO.Cart;
using ECommerce.Application.DTO.Categories;
using ECommerce.Application.DTO.Products;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Category Mappings
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.ParentCategoryName, opt => opt.MapFrom(src => src.ParentCategory != null ? src.ParentCategory.Name : null));

            CreateMap<CreateCategoryRequest, Category>();
            CreateMap<UpdateCategoryRequest, Category>();

            // Product Mappings
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Variants.Any() ? src.Variants.Min(v => v.Price) : 0))
                .ForMember(dest => dest.MainImageUrl, opt => opt.MapFrom(src => src.Images.Any(i => i.IsPrimary) ? src.Images.First(i => i.IsPrimary).ImageUrl : (src.Images.Any() ? src.Images.First().ImageUrl : null)));

            CreateMap<Product, ProductDetailsDto>()
                 .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
                 .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.OrderBy(i => i.SortOrder).Select(i => i.ImageUrl)));

            CreateMap<ProductVariant, ProductVariantDto>();

            CreateMap<CreateProductRequest, Product>();
            CreateMap<UpdateProductRequest, Product>();
            CreateMap<CreateVariantRequest, ProductVariant>();

             // Cart Mappings
            CreateMap<ShoppingCartItem, ShoppingCartItemDto>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductVariant.ProductId))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductVariant.Product.Name))
                .ForMember(dest => dest.VariantName, opt => opt.MapFrom(src => src.ProductVariant.VariantName))
                .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.ProductVariant.SKU))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Quantity * src.UnitPrice))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => 
                    src.ProductVariant.Images.Any() ? src.ProductVariant.Images.First().ImageUrl : 
                    (src.ProductVariant.Product.Images.Any(i => i.IsPrimary) ? src.ProductVariant.Product.Images.First(i => i.IsPrimary).ImageUrl : 
                    (src.ProductVariant.Product.Images.Any() ? src.ProductVariant.Product.Images.First().ImageUrl : null))));

            CreateMap<ShoppingCart, ShoppingCartDto>()
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.Items.Sum(i => i.Quantity * i.UnitPrice)));
        }
    }
}
