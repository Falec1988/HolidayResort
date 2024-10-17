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
    public DateTime BookingDate { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Datum dolaska")]
    public DateOnly CheckInDate { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Datum odlaska")]
    public DateOnly CheckOutDate { get; set; }

    public bool IsPaymentSuccessful { get; set; } = false;

    public DateTime PaymentDate { get; set; }

    public string? StripeSessionId { get; set; }

    public string? StripePaymentIntentId { get; set; }

    public DateTime ActualCheckInDate { get; set; }

    public DateTime ActualCheckOutDate { get; set; }

    public int AccommodationNo { get; set; }
}