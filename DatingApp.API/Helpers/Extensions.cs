using Microsoft.AspNetCore.Http;
using System;

namespace DatingApp.API.Helpers
{
    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse response, string message)
        {  
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers","Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin","*");
        }

        public static int CalculateAge(this DateTime theDateTime)
        {
            var age = DateTime.Today.Year - theDateTime.Year;
            /*
            Add the age (in years) to the original datetime passed in to see if it is later than today's date.
            If it is then the birth is yet to come and the age should be one less.
            */
            if (theDateTime.AddYears(age) >= DateTime.Today)
            {
                age--;
            }
            return age;
        }
    }
}