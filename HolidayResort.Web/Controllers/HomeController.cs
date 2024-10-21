using HolidayResort.Application.Interfaces;
using HolidayResort.Application.Utility;
using HolidayResort.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.Presentation;
using System.Globalization;

namespace HolidayResort.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new()
            {
                AccommodationList = _unitOfWork.Accommodation.GetAll(includeProperties: "AccommodationEquipment"),
                Nights = 1,
                CheckInDate = DateOnly.FromDateTime(DateTime.Now)
            };

            return View(homeVM);
        }

        [HttpPost]
        public IActionResult GetAccommodationsByDate(int nights, DateOnly checkInDate)
        {
            var accommodationList = _unitOfWork.Accommodation.GetAll(includeProperties: "AccommodationEquipment").ToList();

            var accommodationNoList = _unitOfWork.AccommodationNumber.GetAll().ToList();
            
            var bookedAccommodation = _unitOfWork.Booking.GetAll(x => x.Status == SD.StatusApproved ||
            x.Status == SD.StatusCheckedIn).ToList();


            foreach (var accommodation in accommodationList)
            {
                int roomAvailable = SD.AccommodationRoomsAvailableCount
                    (accommodation.Id, accommodationNoList, checkInDate, nights, bookedAccommodation);

                accommodation.IsAvailable = roomAvailable > 0 ? true : false;
            }
            HomeVM homeVM = new()
            {
                CheckInDate = checkInDate,
                AccommodationList = accommodationList,
                Nights = nights
            };

            return PartialView("_AccommodationList", homeVM);
        }

        [HttpPost]
        public IActionResult GeneratePPTExport(int id)
        {
            var accommodation = _unitOfWork.Accommodation.GetAll(includeProperties: "AccommodationEquipment").FirstOrDefault(x => x.Id == id);
            
            if (accommodation is null)
            {
                return RedirectToAction(nameof(Error));
            }

            string basePath = _webHostEnvironment.WebRootPath;
            string filePath = basePath + @"/Exports/DetaljiSmještaja.pptx";

            using IPresentation presentation = Presentation.Open(filePath);

            ISlide slide = presentation.Slides[0];

            IShape? shape = slide.Shapes.FirstOrDefault(x => x.ShapeName == "txtVillaName") as IShape;
            if (shape is not null)
            {
                shape.TextBody.Text = accommodation.Name;
            }

            shape = slide.Shapes.FirstOrDefault(x => x.ShapeName == "txtVillaDescription") as IShape;
            if (shape is not null)
            {
                shape.TextBody.Text = accommodation.Description;
            }

            shape = slide.Shapes.FirstOrDefault(x => x.ShapeName == "txtOccupancy") as IShape;
            if (shape is not null)
            {
                shape.TextBody.Text = string.Format("Kapacitet : " + accommodation.Capacity);
            }

            shape = slide.Shapes.FirstOrDefault(x => x.ShapeName == "txtVillaSize") as IShape;
            if (shape is not null)
            {
                shape.TextBody.Text = string.Format("Kvadratura : " + accommodation.SquareMeter + " m2");
            }

            shape = slide.Shapes.FirstOrDefault(x => x.ShapeName == "txtPricePerNight") as IShape;
            if (shape is not null)
            {
                shape.TextBody.Text = string.Format(accommodation.Price.ToString("C", CultureInfo.CreateSpecificCulture("fr-FR")) + " / po noæenju");
            }

            shape = slide.Shapes.FirstOrDefault(x => x.ShapeName == "txtVillaAmenitiesHeading") as IShape;

            if (shape is not null)
            {
                List<string> listItems = accommodation.AccommodationEquipment.Select(x => x.Name).ToList();

                shape.TextBody.Text = "";

                foreach (var item in listItems)
                {
                    IParagraph paragraph = shape.TextBody.AddParagraph();
                    ITextPart textPart = paragraph.AddTextPart(item);

                    paragraph.ListFormat.Type = ListType.Bulleted;
                    paragraph.ListFormat.BulletCharacter = '\u2022';
                    textPart.Font.FontName = "system-ui";
                    textPart.Font.FontSize = 18;
                    textPart.Font.Color = ColorObject.FromArgb(144, 148, 152);
                }
            }

            shape = slide.Shapes.FirstOrDefault(x => x.ShapeName == "imgVilla") as IShape;

            if (shape is not null)
            {
                byte[] imageData;
                string imageUrl;
                try 
                {
                    imageUrl = string.Format("{0}{1}", basePath, accommodation.ImageUrl);
                    imageData = System.IO.File.ReadAllBytes(imageUrl);
                }
                catch (Exception)
                {
                    imageUrl = string.Format("{0}{1}", basePath, "/images/placeholder.png");
                    imageData = System.IO.File.ReadAllBytes(imageUrl);
                }
                slide.Shapes.Remove(shape);
                using MemoryStream imageStream = new(imageData);
                IPicture newPicture = slide.Pictures.AddPicture(imageStream, 60, 120, 300, 200);
            }

            MemoryStream memoryStream = new();
            presentation.Save(memoryStream);
            memoryStream.Position = 0;

            return File(memoryStream,"application/pptx","DetaljiSmještaja.pptx");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
