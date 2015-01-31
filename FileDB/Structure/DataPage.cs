using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Numeria.IO
{
    internal class DataPage : BasePage
    {
        public const long HEADER_SIZE = 8;
        public const long DATA_PER_PAGE = 4088;

        public override PageType Type { get { return PageType.Data; } }  //  1 byte

        public bool IsEmpty { get; set; }                                //  1 byte
        public short DataBlockLength { get; set; }                       //  2 bytes

        public byte[] DataBlock { get; set; }

        public DataPage(uint pageID)
        {
            PageID = pageID;
            IsEmpty = true;
            DataBlockLength = 0;
            NextPageID = uint.MaxValue;
            DataBlock = new byte[DataPage.DATA_PER_PAGE];
        }
   }
}
