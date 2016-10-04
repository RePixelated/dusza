using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2015_CompetitionScoring
{
    class Nevezés
    {
        int verseny;
        string név;
        int nevIdő;
        string iskola;
        string megye;
        Diák[] diákok;
        string csapat;

        public Nevezés()
        {

        }
    }

    class Diák
    {
        string név;
        int évfolyam;
        string tanár;
    }
}
