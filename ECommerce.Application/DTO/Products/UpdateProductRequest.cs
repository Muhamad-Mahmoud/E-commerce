using System.ComponentModel.DataAnnotations;
using ECommerce.Domain.Enums;

namespace ECommerce.Application.DTO.Products
{
    public class UpdateProductRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        [Required]
        public int CategoryId { get; set; }
        
        public ProductStatus Status { get; set; }

        // For simplicity in Phase 1: We might replace variants logic or handle separately
        // But let's allow updating basic info first. 
        // Updating variants is complex (add/remove/update logic). 
        // We will skip complex variant update in this DTO for now, or assume full replacement if provided.
    }
}
