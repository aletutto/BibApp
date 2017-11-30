using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BibApp.Models.Warenkorb
{
    public class AdminWarenkorb
    {
        BibContext bibContext;
        public AdminWarenkorb(BibContext context)
        {
            this.bibContext = context;
        }
        public async Task Ausleihen()
        {

        }
    }
}
