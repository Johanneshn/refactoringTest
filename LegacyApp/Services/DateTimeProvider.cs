using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegacyApp.Services
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime DateTime => DateTime.Now;   
    }
}
