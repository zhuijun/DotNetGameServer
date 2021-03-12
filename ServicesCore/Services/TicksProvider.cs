using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicesCore.Services
{
    public class TicksProvider
    {
        private readonly DateTime _centuryBegin = new DateTime(1970, 1, 1, 8, 0, 0);

        public long TicksCache
        {
            get { return (DateTimeCache - _centuryBegin).Ticks; }
        }
        public long TimestampCache
        {
            get { return TicksCache / TimeSpan.TicksPerSecond; }
        }

        public DateTime DateTimeCache { get; set; }
    }
}
