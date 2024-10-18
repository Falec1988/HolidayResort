using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HolidayResort.Domain.Entities;

public class Booking
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; }

    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; }

    [Required]
    public int AccommodationId { get; set; }

    [ForeignKey("AccommodationId")]
    public Accommodation Accommodation { get; set; }

    [Required]
    [Display(Name = "Ime i prezime")]
    public string Name { get; set; }

    [Required]
    public string Email { get; set; }

    [Display(Name = "Broj telefona")]
    public string? Phone { get; set; }

    [Required]
    [Display(Name = "Ukupna cijena")]
    public double TotalCost { get; set; }

    [Display(Name = "Broj noćenja")]
    public int Nights { get; set; }

    public string? Status { get; set; }

    [Required]
    [Display(Name = "Datum rezervacije")]
    public DateTime BookingDate { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Datum prijave")]
    public DateOnly CheckInDate { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Datum odjave")]
    public DateOnly CheckOutDate { get; set; }

    public bool IsPaymentSuccessful { get; set; } = false;

    [Display(Name = "Datum plačanja")]
    public DateTime PaymentDate { get; set; }

    [Display(Name = "Stripe Session Id")]
    public string? StripeSessionId { get; set; }

    [Display(Name = "Stripe Payment Intent Id")]
    public string? StripePaymentIntentId { get; set; }

    [Display(Name = "Stvarni datum prijave")]
    public DateTime ActualCheckInDate { get; set; }

    [Display(Name = "Stvarni datum odjave")]
    public DateTime ActualCheckOutDate { get; set; }

    [Display(Name = "Broj smještaja")]
    public int AccommodationNo { get; set; }

    [NotMapped]
    public List<AccommodationNumber> AccommodationNumber { get; set; }
}