using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Numeria.IO
{
    internal class IndexLink
    {
        public byte Index { get; set; }
        public uint PageID { get; set; }

        public IndexLink()
        {
            Index = 0;
            PageID = uint.MaxValue;
        }

        public bool IsEmpty
        {
            get
            {
                return PageID == uint.MaxValue;
            }
        }
    }
}
