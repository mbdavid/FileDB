using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Numeria.IO
{
    internal class IndexFactory
    {
        public static IndexNode GetRootIndexNode(Engine engine)
        {
            IndexPage rootIndexPage = engine.CacheIndexPage.GetPage(engine.Header.IndexRootPageID);
            return rootIndexPage.Nodes[0];
        }

        public static IndexNode BinaryInsert(EntryInfo target, IndexNode baseNode, Engine engine)
        {
            var dif = baseNode.ID.CompareTo(target.ID);

            if (dif == 1) // > Maior (Right)
            {
                if (baseNode.Right.IsEmpty)
                    return BinaryInsertNode(baseNode.Right, baseNode, target, engine);
                else
                    return BinaryInsert(target, GetChildIndexNode(baseNode.Right, engine), engine);
            }
            else if (dif == -1) // < Menor (Left)
            {
                if (baseNode.Left.IsEmpty)
                    return BinaryInsertNode(baseNode.Left, baseNode, target, engine);
                else
                    return BinaryInsert(target, GetChildIndexNode(baseNode.Left, engine), engine);
            }
            else
            {
                throw new FileDBException("Same GUID?!?");
            }
        }

        private static IndexNode GetChildIndexNode(IndexLink link, Engine engine)
        {
            var pageIndex = engine.CacheIndexPage.GetPage(link.PageID);
            return pageIndex.Nodes[link.Index];
        }

        private static IndexNode BinaryInsertNode(IndexLink baseLink, IndexNode baseNode, EntryInfo entry, Engine engine)
        {
            // Must insert my new nodo
            var pageIndex = engine.GetFreeIndexPage();
            var newNode = pageIndex.Nodes[pageIndex.NodeIndex];

            baseLink.PageID = pageIndex.PageID;
            baseLink.Index = pageIndex.NodeIndex;

            newNode.UpdateFromEntry(entry);
            newNode.DataPageID = DataFactory.GetStartDataPageID(engine);

            if (pageIndex.PageID != baseNode.IndexPage.PageID)
                engine.CacheIndexPage.AddPage(baseNode.IndexPage, true);

            engine.CacheIndexPage.AddPage(pageIndex, true);

            return newNode;
        }

        public static IndexNode BinarySearch(Guid target, IndexNode baseNode, Engine engine)
        {
            var dif = baseNode.ID.CompareTo(target);

            if (dif == 1) // > Maior (Right)
            {
                if (baseNode.Right.IsEmpty) // If there no ones on right, GUID not found
                    return null;
                else
                    return BinarySearch(target, GetChildIndexNode(baseNode.Right, engine), engine); // Recursive call on right node
            }
            else if (dif == -1) // < Menor (Left)
            {
                if (baseNode.Left.IsEmpty) // If there no ones on left, GUID not found
                    return null;
                else
                    return BinarySearch(target, GetChildIndexNode(baseNode.Left, engine), engine); // Recursive call on left node
            }
            else
            {
                // Found it
                return baseNode;
            }
        }


    }
}
