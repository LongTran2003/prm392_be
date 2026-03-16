using FoodOrderSystem.Utilities.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderSystem.Models.Domains
{
    public class ShopOwner
    {
        [Key]
        public Guid ShopOwnerId { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; } = null!;

        public double WalletBalance { get; set; } = 0;

        public DateTime CreatedTime { get; set; } = StaticOperationStatus.Timezone.Vietnam;

        public string Status { get; set; } = StaticOperationStatus.BaseEntity.Active;
    }
}
