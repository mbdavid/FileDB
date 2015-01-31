using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Numeria.IO
{
    internal class Header
    {
        public const long LOCKER_POS = 98;
        public const long HEADER_SIZE = 100;

        public const string FileID = "FileDB";        // 6 bytes
        public const short FileVersion = 1;           // 2 bytes

        /// <summary>
        /// Armazena a primeira página que contem o inicio do indice. Valor sempre fixo = 0. Utilizado o inicio da busca binária
        /// Storage the fist index page (root page). It's fixed on 0 (zero)
        /// </summary>
        public uint IndexRootPageID { get; set; }      // 4 bytes

        /// <summary>
        /// Contem a página que possui espaço disponível para novas inclusões de indices
        /// This last has free nodes to be used
        /// </summary>
        public uint FreeIndexPageID { get; set; }      // 4 bytes
        
        /// <summary>
        /// Quando há exclusão de dados, a primeira pagina a ficar vazia infora a esse ponteiro que depois vai aproveitar numa proxima inclusão
        /// When a deleted data, this variable point to first page emtpy. I will use to insert the next data page
        /// </summary>
        public uint FreeDataPageID { get; set; }       // 4 bytes

        /// <summary>
        /// Define, numa exclusão de dados, a ultima pagina excluida. Será utilizado para fazer segmentos continuos de exclusão, ou seja, assim que um segundo arquivo for apagado, o ponteiro inicial dele deve apontar para o ponteiro final do outro
        /// Define, in a deleted data, the last deleted page. It's used to make continuos statments of empty page data
        /// </summary>
        public uint LastFreeDataPageID { get; set; }   // 4 bytes
        
        /// <summary>
        /// Ultima página utilizada pelo FileDB (seja para Indice/Data). É utilizado para quando o arquivo precisa crescer (criar nova pagina)
        /// Last used page on FileDB disk (even index or data page). It's used to grow the file db (create new pages)
        /// </summary>
        public uint LastPageID { get; set; }           // 4 bytes

        public Header()
        {
            IndexRootPageID = uint.MaxValue;
            FreeIndexPageID = uint.MaxValue;
            FreeDataPageID = uint.MaxValue;
            LastFreeDataPageID = uint.MaxValue;
            LastPageID = uint.MaxValue;
            IsDirty = false;
        }

        public bool IsDirty { get; set; }
    }
}
