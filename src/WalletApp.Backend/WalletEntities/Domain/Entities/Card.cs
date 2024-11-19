using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalletEntities.Domain.Entities
{
    public class Card
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        [Required]
        public string UserId { get; set; } = default!;
        [ForeignKey("UserId")]
        public AuthorizedUser User { get; set; } = default!;
    }
}
