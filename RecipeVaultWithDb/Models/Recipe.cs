using System.ComponentModel.DataAnnotations;

namespace RecipeVaultWithDb.Models;

public class Recipe
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Заглавието е задължително.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Заглавието трябва да е между 3 и 100 символа.")]
    [Display(Name = "Заглавие")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Категорията е задължителна.")]
    [Display(Name = "Категория")]
    public string Category { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Инструкциите са задължителни.")]
    [StringLength(5000, MinimumLength = 10, ErrorMessage = "Инструкциите трябва да са между 10 и 5000 символа.")]
    [Display(Name = "Инструкции за приготвяне")]
    public string Instructions { get; set; } = string.Empty;
    
    [Range(1, 600, ErrorMessage = "Времето трябва да е между 1 и 600 минути.")]
    [Display(Name = "Време за готвене (минути)")]
    public int? CookTimeMinutes { get; set; }

    [Range(1, 100, ErrorMessage = "Броят порции трябва да е между 1 и 100.")]
    [Display(Name = "Брой порции")]
    public int Servings { get; set; } = 4;

    [Url(ErrorMessage = "Моля въведете валиден URL адрес.")]
    [StringLength(500)]
    [Display(Name = "Снимка (URL)")]
    public string? ImageUrl { get; set; }
    
    [Display(Name = "Дата на добавяне")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}