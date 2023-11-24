using MVCRealEstateData;
using System.ComponentModel.DataAnnotations;

namespace MVCRealEstate.Models;

public enum PostTypesView
{
    [Display(Name = "Satılık")] ForSale, [Display(Name = "Kiralık")] ForRent
}

public class PostViewModel
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    [Display(Name = "İl/İlçe")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
    public int DistrictId { get; set; }

    [Display(Name = "Kategori")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
    public Guid CategoryId { get; set; }

    [Display(Name = "İlan Başlığı")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
    public required string Name { get; set; }

    public string? Image { get; set; }

    [Display(Name = "Açıklamalar")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
    public string? Descriptions { get; set; }

    [Display(Name = "Fiyat")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
    public decimal Price { get; set; }

    [Display(Name = "Görsel")]
    public IFormFile ImageFile { get; set; }

    [Display(Name = "Foto Galeri")]
    public IEnumerable<IFormFile> ImageFiles { get; set; }

    [Display(Name = "Özellikler")]
    public IEnumerable<Guid> Specs { get; set; }

    public DateTime Date { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    [Display(Name = "İlan Türü")]
    public PostTypesView Type { get; set; }
}
