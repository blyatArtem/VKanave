﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKanave.Extensions
{
    public static class UnixTimeExtensions
    {
        /// <param name="dt">in seconds</param>
        internal static int ToUnixTime(this DateTime dt)
        {
            DateTimeOffset dto = new DateTimeOffset(dt);
            return (int)dto.ToUnixTimeSeconds();
        }

        /// <param name="unixtime">in seconds</param>
        internal static DateTime ToDateTime(this int unixtime)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixtime);
            return dtDateTime;
        }
    }
}
