using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Numeria.IO
{
    public class DebugFile
    {
        private Engine _engine;

        internal DebugFile(Engine engine)
        {
            _engine = engine;
        }

        public string DisplayPages()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Constants:");
            sb.AppendLine("=============");
            sb.AppendLine("BasePage.PAGE_SIZE       : " + BasePage.PAGE_SIZE);
            sb.AppendLine("IndexPage.HEADER_SIZE    : " + IndexPage.HEADER_SIZE);
            sb.AppendLine("IndexPage.NODES_PER_PAGE : " + IndexPage.NODES_PER_PAGE);
            sb.AppendLine("DataPage.HEADER_SIZE     : " + DataPage.HEADER_SIZE);
            sb.AppendLine("DataPage.DATA_PER_PAGE   : " + DataPage.DATA_PER_PAGE);

            sb.AppendLine();
            sb.AppendLine("Header:");
            sb.AppendLine("=============");
            sb.AppendLine("IndexRootPageID    : " + _engine.Header.IndexRootPageID.Fmt());
            sb.AppendLine("FreeIndexPageID    : " + _engine.Header.FreeIndexPageID.Fmt());
            sb.AppendLine("FreeDataPageID     : " + _engine.Header.FreeDataPageID.Fmt());
            sb.AppendLine("LastFreeDataPageID : " + _engine.Header.LastFreeDataPageID.Fmt());
            sb.AppendLine("LastPageID         : " + _engine.Header.LastPageID.Fmt());

            sb.AppendLine();
            sb.AppendLine("Pages:");
            sb.AppendLine("=============");

            for (uint i = 0; i <= _engine.Header.LastPageID; i++)
            {
                BasePage page = PageFactory.GetBasePage(i, _engine.Reader);

                sb.AppendFormat("[{0}] >> [{1}] ({2}) ",
                    page.PageID.Fmt(), page.NextPageID.Fmt(), page.Type == PageType.Data ? "D" : "I");

                if (page.Type == PageType.Data)
                {
                    var dataPage = (DataPage)page;

                    if (dataPage.IsEmpty)
                        sb.Append("Empty");
                    else
                        sb.AppendFormat("Bytes: {0}", dataPage.DataBlockLength);
                }
                else
                {
                    var indexPage = (IndexPage)page;

                    sb.AppendFormat("Keys: {0}", indexPage.NodeIndex + 1);
                }


                sb.AppendLine();
            }


            return sb.ToString();
        }
    }

    internal static class Display
    {
        public static string Fmt(this uint val)
        {
            if (val == uint.MaxValue)
                return "----";
            else
                return val.ToString("0000");
        }
    }
}
