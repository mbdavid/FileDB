using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace Numeria.IO
{
    internal class Range<TStart, TEnd>
    {
        public TStart Start { get; set; }
        public TEnd End { get; set; }

        public Range()
        {
        }

        public Range(TStart start, TEnd end)
        {
            this.Start = start;
            this.End = end;
        }
    }
}
