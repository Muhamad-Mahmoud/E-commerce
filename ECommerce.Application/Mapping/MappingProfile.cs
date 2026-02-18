using AutoMapper;
using ECommerce.Application.DTO.Addresses.Requests;
using ECommerce.Application.DTO.Addresses.Responses;
using ECommerce.Application.DTO.Cart.Responses;
using ECommerce.Application.DTO.Categories.Requests;
using ECommerce.Application.DTO.Categories.Responses;
using ECommerce.Application.DTO.Orders.Responses;
using ECommerce.Application.DTO.Products.Requests;
using ECommerce.Application.DTO.Products.Responses;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Address Mappings
            CreateMap<Address, AddressDto>();
            CreateMap<CreateAddressDto, Address>();
            CreateMap<AddressDto, Address>();

            // Category Mappings
            CreateMap<Category, CategoryResponse>()
                .ForMember(dest => dest.ParentCategoryName, opt => opt.MapFrom(src => src.ParentCategory != null ? src.ParentCategory.Name : null));

            CreateMap<CreateCategoryRequest, Category>();
            CreateMap<UpdateCategoryRequest, Category>();

            // Product Mappings
            CreateMap<Product, ProductResponse>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Variants.Any() ? src.Variants.Min(v => v.Price) : 0))
                .ForMember(dest => dest.MainImageUrl, opt => opt.MapFrom(src => src.Images.Any(i => i.IsPrimary) ? src.Images.First(i => i.IsPrimary).ImageUrl : (src.Images.Any() ? src.Images.First().ImageUrl : null)));

            CreateMap<Product, ProductDetailsResponse>()
                 .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
                 .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.OrderBy(i => i.SortOrder).Select(i => i.ImageUrl)));

            CreateMap<ProductVariant, ProductVariantResponse>();

            CreateMap<CreateProductRequest, Product>();
            CreateMap<UpdateProductRequest, Product>();
            CreateMap<CreateVariantRequest, ProductVariant>();

            // Cart Mappings
            CreateMap<ShoppingCartItem, ShoppingCartItemResponse>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductVariant.ProductId))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductVariant.Product.Name))
                .ForMember(dest => dest.VariantName, opt => opt.MapFrom(src => src.ProductVariant.VariantName))
                .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.ProductVariant.SKU))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Quantity * src.UnitPrice))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src =>
                    src.ProductVariant.Images.Any() ? src.ProductVariant.Images.First().ImageUrl :
                    (src.ProductVariant.Product.Images.Any(i => i.IsPrimary) ? src.ProductVariant.Product.Images.First(i => i.IsPrimary).ImageUrl :
                    (src.ProductVariant.Product.Images.Any() ? src.ProductVariant.Product.Images.First().ImageUrl : null))));

            CreateMap<ShoppingCart, ShoppingCartResponse>()
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.Items.Sum(i => i.Quantity * i.UnitPrice)));

            // Order Mappings
            CreateMap<OrderItem, OrderItemResponse>()
                .ForMember(dest => dest.ProductSku, opt => opt.MapFrom(src => src.ProductVariant.SKU)); // Assuming ProductVariant might be included or we just leave it null if not loaded

            CreateMap<Order, OrderResponse>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus.ToString()));
        }
    }
}
