using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
    class Eredmeny
    {
        public int programozóSzám;
        public int robotSzam;
        public string[,] csapatnev;
        public int[,] pontszam;
        public Eredmeny()
        {
            StreamReader eredmény = new StreamReader("eredmeny.txt");

            programozóSzám = Convert.ToInt32(eredmény.ReadLine());
            robotSzam = Convert.ToInt32(eredmény.ReadLine());

            csapatnev = new string [programozóSzám + robotSzam, 2]; // [x, 0] -> programozoadatok, [x, 1] -> robotadatok
            pontszam = new int [programozóSzám + robotSzam, 2]; // [x, 0] -> programozoadatok, [x, 1] -> robotadatok

            string[] konténer = new string[programozóSzám+robotSzam]; //ideiglenes változó, ezt még "splitelni" kell majd
            for (int i = 0; i < programozóSzám+robotSzam; i++)
            {
                konténer[i] = eredmény.ReadLine();
            }


            for (int i = 0; i < programozóSzám; i++) // programoró kategóriához ciklus
            {
                string[] ketteskonténer = konténer[i].Split(';');
                csapatnev[i, 0] = ketteskonténer[0];
                pontszam[i, 0] = Convert.ToInt32(ketteskonténer[1]);
            }
            for (int i = 0; i < robotSzam; i++) // robotika ketegóriához ciklus
            {
                string[] ketteskonténer = konténer[i + programozóSzám].Split(';');
                csapatnev[i, 1] = ketteskonténer[0];
                pontszam[i, 1] = Convert.ToInt32(ketteskonténer[1]);
            }
           
        }
    }

}
