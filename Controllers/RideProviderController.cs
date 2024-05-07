using CCA_BAL.DTO;
using CCA_BAL.Interface;
using Microsoft.AspNetCore.Mvc;
using CCA_BAL.Result;
using System.Transactions;

namespace CCA1.Controllers
{
    [Route("api")]
    public class RideProviderController : Controller
    {
        private readonly IRideProvideService _service;
        private readonly IBillService _billService;
        private readonly ITripService _tripService;
        
        public RideProviderController(IRideProvideService service,IBillService billService,ITripService tripService)
        {
            _service = service;
            _billService = billService;
            _tripService = tripService;
        }
        [HttpGet("[controller]/Rideprovide/")]
        public async Task<IActionResult> getRider()
        {
            var result = await _service.getRider();
            return Ok(result);
        }
        [HttpGet("[controller]/{rpId}")]
        public async Task<IActionResult> getRideDetails(string rpId)
        {
            var result = await _service.getRiderById(rpId);
            return Ok(result);
        }

        //GET: api/<controller>/new
        [HttpPost("[controller]/new")]
        public async Task<IActionResult> createNewProvide([FromBody]RideDTO rideProviderDTO)
        {
                try
                {

                if (ValidateAge(rideProviderDTO.BirthDate) == false)
                {
                    return BadRequest(new Output()
                    {
                        Result = false,
                        errorMessage = "Age must be minimum of 18."
                    });
                }
                if (!IsValidPhoneNumber(rideProviderDTO.Phone))
                {
                    return BadRequest(new Output()
                    {
                        Result = false,
                        errorMessage = "Phone number must be 10 digits."
                    });
                }
                if (!IsValidEmail(rideProviderDTO.EmailId))
                {
                    return BadRequest(new Output()
                    {
                        Result = false,
                        errorMessage = "Email address should always have @cognizant.com"
                    });
                }
                if (!IsValidName(rideProviderDTO.FirstName, rideProviderDTO.lastName))
                {
                    return BadRequest(new Output()
                    {
                        Result = false,
                        errorMessage = "First name and last name should only have alphabets and last name must be minimum 3 characters long."
                    });
                }
                if (!IsValidDLNo(rideProviderDTO.dlNo))
                {
                    return BadRequest(new Output()
                    {
                        Result = false,
                        errorMessage = "drivingLicenceNumber must be 16 characters long it should including three  hypen(-)"
                    });
                }
                if (!ValidateAadharNumber(rideProviderDTO.Adharcard))
                {
                    return BadRequest(new Output()
                    {
                        Result = false,
                        errorMessage = "Aadhar No number should be 12 digit"
                    });
                }

                //Console.WriteLine(rideProviderDTO.FirstName);
                Output result = await _service.CreateNewRideProvider(rideProviderDTO);
                    if (result.Result)
                    {
                        return Ok();
                    }
                    else
                    {
                        return BadRequest(result.errorMessage);
                    }
                    return BadRequest();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
        }
        [HttpPut("[controller]/{providerId}/update")]
        public async Task<IActionResult> updateRideProvide(string providerId, [FromBody] UpdateRideProvideDTO rideProviderDTO)
        {
            try
            {
                Output result = await _service.UpdateNewRideProvider(providerId,rideProviderDTO);
                if (result.Result == true)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(result.errorMessage);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpDelete("[controller]/{providerId}/delete")]
        public async Task<IActionResult> deleteRideProvide(string providerId)
        {
            var res = await _service.DeleteRideProvider(providerId);
            if (res.Result == true)
            {
                return Ok();
            }
            else
            {
                return StatusCode(500, "Ride provide is registered it cannot be deleted");
            }
        }
        [HttpGet("[controller]/billing/{month}")]
        public async Task<IActionResult> fetchBill(int month)
        {
            var res = await _billService.RetrieveBill(month);
            return Ok(res);
        }
        [HttpPost("[controller]/addbooking")]
        public async Task<IActionResult> CreateTrip([FromBody] TripBookingDTO tripBookingDTO)
        {
            try
            {
                if (!IsSeatNegative(tripBookingDTO.NoOfSeat))
                {
                    return BadRequest(new Output()
                    {
                        Result = false,
                        errorMessage = "No of seats cannot be negative or less than zero"
                    });
                }
                //Console.WriteLine(tripBookingDTO.noOfSeat);
                if (!IsSeatsFilledGreater(tripBookingDTO.NoOfSeat, tripBookingDTO.SeatsFilled))
                {
                    return BadRequest(new Output()
                    {
                        Result = false,
                        errorMessage = "No of seats filled cannot be greater than no of seats and also cannot be less than zero"
                    });
                }

                if (!checkDateTimeInFuture(tripBookingDTO.RideDate, tripBookingDTO.RideTime))
                {
                    return BadRequest(new Output()
                    {
                        Result = false,
                        errorMessage = "Date and time should be in the future."
                    });
                }
                //Console.WriteLine(tripBookingDTO.NoOfSeat);
                Output result = await _tripService.CreateTrip(tripBookingDTO);
                if (result.Result == true)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(result.errorMessage);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("[controller]/bookingStatus/{tripId}")]
        public async Task<IActionResult> getTripDetails(string tripId) {
            var result = await _tripService.getTrip(tripId);
            return Ok(result);
        }
        [HttpPut("[controller]/bookings/{tripId}")]
        public async Task<IActionResult> updateDetails(string tripId, [FromBody] TripBookingDTO tripBookingDTO)
        {
            try
            {
                Output res = await _tripService.updateTrip(tripId, tripBookingDTO);
                if(res.Result == true)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(res.errorMessage);
                }
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
       
        [HttpGet("[controller]/generatebill/{tripId}")]
        public async Task<IActionResult> getBillDetailz(string tripId)
        {
            var result=await _billService.generateBillDetails(tripId);
            return Ok(result);
        }


        private bool IsValidPhoneNumber(long phone) => phone.ToString().Length == 10;

        private bool ValidateAge(DateOnly birthDate)
        {
            // Calculate age based on birth date
            int currentYear = DateOnly.FromDateTime(DateTime.Today).Year;
            int birthYear = birthDate.Year;
            int age = currentYear - birthYear;
            //Console.WriteLine(age);
            if (age >= 18) return true;
            return false;
        }
        private bool IsValidEmail(string email)
        {
            return email.EndsWith("@cognizant.com");
        }
        private bool IsValidName(string firstName, string lastName)
        {
            return firstName.All(char.IsLetter) && lastName.All(char.IsLetter) && lastName.Length >= 3;
        }
        private bool IsValidDLNo(string dlNo)
        {
            return dlNo.Length == 16 && dlNo.Count(c => c == '-') == 3;
        }
        private bool ValidateAadharNumber(long aadharcard)
        {

            return aadharcard.ToString().Length == 12;
        }


        private bool IsSeatNegative(int noOfSeat)
        {
            if (noOfSeat <= 0)
            {
                return false;
            }
            return true;
        }
        private bool IsSeatsFilledGreater(int noOfSeat, int seatsFilled)
        {
            if (noOfSeat < seatsFilled)
            {
                return false;
            }
            return true;
        }

        private bool checkDateTimeInFuture(DateOnly date, TimeOnly time)
        {
            if (date < DateOnly.FromDateTime(DateTime.Today))
            {
                return false;
            }
            else if (date == DateOnly.FromDateTime(DateTime.Today))
            {
                if (time < TimeOnly.FromDateTime(DateTime.Today)) return false;
            }
            return true;
        }
    }
}
