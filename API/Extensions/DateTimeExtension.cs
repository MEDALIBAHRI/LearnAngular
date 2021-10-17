using System;

namespace API.Extensions
{
    public  static class DateTimeExtension
    {
        public static int GetAge(this DateTime dob){
            DateTime today = DateTime.Now;
            int age = today.Year - dob.Year;
            if(dob.Date> today.AddYears(-age)) age--;
            return age;
        }
    }
}