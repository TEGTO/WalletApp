using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalletEntities.Domain.Entities
{
    public enum TransactionType
    {
        [Display(Name = "Payment")]
        Payment,
        [Display(Name = "Credit")]
        Credit
    }
    public class Icon
    {
        public string CssClass { get; set; } = default!;
        public string BackgroundColor { get; set; } = default!;
    }
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        public string CardId { get; set; } = default!;
        [ForeignKey("CardId")]
        public Card Card { get; set; } = default!;
        [Required]
        public TransactionType Type { get; set; }
        [Required]
        public decimal Sum { get; set; }
        [Required, MaxLength(512)]
        public string Name { get; set; } = default!;
        [Required, MaxLength(2048)]
        public string Description { get; set; } = default!;
        [Required]
        public DateTime Date { get; set; } = default!;
        [Required]
        public bool Pending { get; set; }
        public string? AuthorizedUserId { get; set; }
        [ForeignKey("AuthorizedUserId")]
        public AuthorizedUser? AuthorizedUser { get; set; }
        [NotMapped]
        public Icon TransactionIcon => GenerateIcon();

        private static Icon GenerateIcon()
        {
            return new Icon
            {
                CssClass = GetIconCssClass(),
                BackgroundColor = GetRandomDarkColor()
            };
        }
        private static string GetIconCssClass()
        {
            return "service-img";
        }
        private static string GetRandomDarkColor()
        {
            var darkColors = new List<string>
            {
                "bg-dark-blue",
                "bg-dark-green",
                "bg-dark-gray",
                "bg-dark-purple",
                "bg-dark-red"
            };

            var random = new Random();
            int index = random.Next(darkColors.Count);
            return darkColors[index];
        }
    }
}
