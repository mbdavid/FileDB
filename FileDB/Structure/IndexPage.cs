using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Numeria.IO
{
    internal class IndexPage : BasePage
    {
        public const long HEADER_SIZE = 46;
        public const int NODES_PER_PAGE = 50;

        public override PageType Type { get { return PageType.Index; } }  //  1 byte
        public byte NodeIndex { get; set; }                               //  1 byte

        public IndexNode[] Nodes { get; set; }

        public bool IsDirty { get; set; }

        public IndexPage(uint pageID)
        {
            PageID = pageID;
            NextPageID = uint.MaxValue;
            NodeIndex = 0;
            Nodes = new IndexNode[IndexPage.NODES_PER_PAGE];
            IsDirty = false;

            for (int i = 0; i < IndexPage.NODES_PER_PAGE; i++)
            {
                var node = Nodes[i] = new IndexNode(this);
            }
        }

    }
}
