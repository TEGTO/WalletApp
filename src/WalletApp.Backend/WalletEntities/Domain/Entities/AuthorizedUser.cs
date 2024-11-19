using System.ComponentModel.DataAnnotations;

namespace WalletEntities.Domain.Entities
{
    public class AuthorizedUser
    {
        [Key]
        public string Id { get; set; } = default!;
        [Required, MaxLength(256)]
        public string Name { get; set; } = default!;
        public List<Card> Cards { get; set; } = default!;
    }
}
