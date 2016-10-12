using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace _2015_CompetitionScoring
{
    class Adatok
    {
        public static Nevezések nevezések = new Nevezések();
    }

    class Nevezések
    {
        List<Nevezés> nevezések = new List<Nevezés>();

        public Nevezések()
        {
            StreamReader forrás = new StreamReader("nevezes.txt");

            int i = 0;
            while (i < File.ReadLines("nevezes.txt").Count())
            {
                nevezések.Add(new Nevezés(forrás.ReadLine()));
                i++;
            }
        }

        public Nevezés Nevezés(int i) { return nevezések[i]; }
    }

    class Nevezés
    {
        int verseny;
        string csapat;
        int nevIdő;
        int hónap;
        int nap;
        string iskola;
        string megye;
        string diák;
        List<string> diákok = new List<string>();
        int évfolyam;
        string tanár;

        public Nevezés(string forrás)
        {
            string[] narancs = forrás.Split(';');

            /// Verseny kódja
            verseny = Convert.ToInt32(narancs[0]);

            /// Csapat neve
            csapat = narancs[1];

            /// Nevezés ideje
            hónap = Convert.ToInt32(narancs[2].Substring(0, 2));
            nap = Convert.ToInt32(narancs[2].Substring(2, 2));

            nevIdő = 0;
            for (int i = 1; i <= hónap; i++)
                nevIdő += i % 2 == 0 ? (i == 2 ? 28 : 30) : 31;
            nevIdő += nap;

            /// Iskola neve
            iskola = narancs[3];

            /// Megye neve
            megye = narancs[4];

            /// Diákok neve
            if (verseny == 1)
                for (int i = 0; i < 3; i++)
                    diákok.Add(narancs[5 + i]);

            else
                diák = narancs[5];

            /// Évfolyam
            évfolyam = Convert.ToInt32(verseny == 1 ? narancs[8] : narancs[6]);

            /// Felkészítő tanár
            tanár = verseny == 1 ? narancs[9] : narancs[7];
        }

        public int Évfolyam() { return évfolyam; }
    }

    class Eredmény
    {
        int progSzám;
        int robotSzám;

        List<Csapat> csapatok = new List<Csapat>();

        public Eredmény()
        {
            StreamReader forrás = new StreamReader("eredmeny.txt");

            progSzám = Convert.ToInt32(forrás.ReadLine());
            robotSzám = Convert.ToInt32(forrás.ReadLine());

            csapatok.Add(new Csapat(forrás.ReadLine()));
        }
    }

    class Csapat
    {
        string név;
        int pontszám;

        public Csapat(string forrás)
        {
            string[] körte = forrás.Split(';');

            /// Név
            név = körte[0];

            /// Pontszám
            pontszám = Convert.ToInt32(körte[1]);
        }
    }
}
