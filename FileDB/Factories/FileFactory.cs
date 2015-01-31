using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Numeria.IO
{
    internal class FileFactory
    {
        public static void CreateEmptyFile(BinaryWriter writer)
        {
            // Create new header instance
            var header = new Header();

            header.IndexRootPageID = 0;
            header.FreeIndexPageID = 0;
            header.FreeDataPageID = uint.MaxValue;
            header.LastFreeDataPageID = uint.MaxValue;
            header.LastPageID = 0;

            HeaderFactory.WriteToFile(header, writer);

            // Create a first fixed index page
            var pageIndex = new IndexPage(0);
            pageIndex.NodeIndex = 0;
            pageIndex.NextPageID = uint.MaxValue;

            // Create first fixed index node, with fixed middle guid
            var indexNode = pageIndex.Nodes[0];
            indexNode.ID = new Guid(new byte[] { 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127, 127 });
            indexNode.IsDeleted = true;
            indexNode.Right = new IndexLink();
            indexNode.Left = new IndexLink();
            indexNode.DataPageID = uint.MaxValue;
            indexNode.FileName = string.Empty;
            indexNode.FileExtension = string.Empty;

            PageFactory.WriteToFile(pageIndex, writer);

        }
    }
}
