﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kOS
{
    public class TimeSpan : Structure
    {
        System.TimeSpan span;

        public TimeSpan(double unixStyleTime)
        {
            span = System.TimeSpan.FromSeconds(unixStyleTime);

            AddSuffix("YEAR", null, () => { return Year(); });
            AddSuffix("DAY", null, () => { return Day(); });
            AddSuffix("HOUR", null, () => { return span.Hours; });
            AddSuffix("MINUTE", null, () => { return span.Minutes; });
            AddSuffix("SECOND", null, () => { return span.Seconds; });
            AddSuffix("SECONDS", null, () => { return span.TotalSeconds; });

            AddSuffix("CLOCK", null, () => { return span.Hours + ":" + String.Format(span.Minutes.ToString("00") + ":" + String.Format(span.Seconds.ToString("00"))); });
            AddSuffix("CALENDAR", null, () => { return "Year " + Year() + ", day " + Day(); });
        }

        public int Year()
        {
            return (int)Math.Floor(span.Days / 365.0) + 1;
        }

        public int Day()
        {
            return (span.Days % 365) + 1;
        }

        public double ToUnixStyleTime()
        {
            return span.TotalSeconds;
        }
        
        public static TimeSpan operator +(TimeSpan a, TimeSpan b) { return new TimeSpan(a.ToUnixStyleTime() + b.ToUnixStyleTime()); }
        public static TimeSpan operator -(TimeSpan a, TimeSpan b) { return new TimeSpan(a.ToUnixStyleTime() - b.ToUnixStyleTime()); }
        public static TimeSpan operator +(TimeSpan a, double b) { return new TimeSpan(a.ToUnixStyleTime() + b); }
        public static TimeSpan operator -(TimeSpan a, double b) { return new TimeSpan(a.ToUnixStyleTime() - b); }
        public static TimeSpan operator *(TimeSpan a, double b) { return new TimeSpan(a.ToUnixStyleTime() * b); }
        public static TimeSpan operator /(TimeSpan a, double b) { return new TimeSpan(a.ToUnixStyleTime() / b); }
        public static TimeSpan operator -(double b, TimeSpan a) { return new TimeSpan(b - a.ToUnixStyleTime()); }
        public static TimeSpan operator *(double b, TimeSpan a) { return new TimeSpan(b * a.ToUnixStyleTime()); }
        public static TimeSpan operator /(double b, TimeSpan a) { return new TimeSpan(b / a.ToUnixStyleTime()); }
        public static TimeSpan operator /(TimeSpan b, TimeSpan a) { return new TimeSpan(b.ToUnixStyleTime() / a.ToUnixStyleTime()); }
        public static bool operator >(TimeSpan a, TimeSpan b) { return a.ToUnixStyleTime() > b.ToUnixStyleTime(); }
        public static bool operator <(TimeSpan a, TimeSpan b) { return a.ToUnixStyleTime() < b.ToUnixStyleTime(); }
        public static bool operator >=(TimeSpan a, TimeSpan b) { return a.ToUnixStyleTime() >= b.ToUnixStyleTime(); }
        public static bool operator <=(TimeSpan a, TimeSpan b) { return a.ToUnixStyleTime() <= b.ToUnixStyleTime(); }
        public static bool operator >(TimeSpan a, double b) { return a.ToUnixStyleTime() > b; }
        public static bool operator <(TimeSpan a, double b) { return a.ToUnixStyleTime() < b; }
        public static bool operator >=(TimeSpan a, double b) { return a.ToUnixStyleTime() >= b; }
        public static bool operator <=(TimeSpan a, double b) { return a.ToUnixStyleTime() <= b; }
        public static bool operator >(double a, TimeSpan b) { return a > b.ToUnixStyleTime(); }
        public static bool operator <(double a, TimeSpan b) { return a < b.ToUnixStyleTime(); }
        public static bool operator >=(double a, TimeSpan b) { return a >= b.ToUnixStyleTime(); }
        public static bool operator <=(double a, TimeSpan b) { return a <= b.ToUnixStyleTime(); }

        public override object TryOperation(string op, object other, bool reverseOrder)
        {
            // Order shouldn't matter here
            if (other is TimeSpan && op == "+") return this + (TimeSpan)other;
            if (other is double && op == "+") return this + (double)other;
            if (other is double && op == "*") return this * (double)other; // Order would matter here if this were matrices

            if (!reverseOrder)
            {
                if (other is TimeSpan && op == "-") return this - (TimeSpan)other;
                if (other is TimeSpan && op == "/") return this / (TimeSpan)other;
                if (other is double && op == "-") return this - (double)other;
                if (other is double && op == "/") return this / (double)other;
                if (other is TimeSpan && op == ">") return this > (TimeSpan)other;
                if (other is TimeSpan && op == "<") return this < (TimeSpan)other;
                if (other is TimeSpan && op == ">=") return this >= (TimeSpan)other;
                if (other is TimeSpan && op == "<=") return this <= (TimeSpan)other;
                if (other is double && op == ">") return this > (double)other;
                if (other is double && op == "<") return this < (double)other;
                if (other is double && op == ">=") return this >= (double)other;
                if (other is double && op == "<=") return this <= (double)other;
            }
            else
            {
                if (other is TimeSpan && op == "-") return (TimeSpan)other - this;
                if (other is TimeSpan && op == "/") return (TimeSpan)other / this;
                if (other is double && op == "-") return (double)other - this;
               if (other is double && op == "/") return (double)other / this; // Can't imagine why the heck you'd want to do this but here it is
               if (other is TimeSpan && op == ">") return this < (TimeSpan)other;
               if (other is TimeSpan && op == "<") return this > (TimeSpan)other;
               if (other is TimeSpan && op == ">=") return (TimeSpan)other >= this;
               if (other is TimeSpan && op == "<=") return (TimeSpan)other <= this;
               if (other is double && op == ">") return (double)other > this;
               if (other is double && op == "<") return (double)other < this;
               if (other is double && op == ">=") return (double)other >= this;
               if (other is double && op == "<=") return (double)other <= this;

            }

            return base.TryOperation(op, other, reverseOrder);
        }

        public override string ToString()
        {
            return Math.Floor(span.TotalSeconds).ToString("0");
        }
    }
}
