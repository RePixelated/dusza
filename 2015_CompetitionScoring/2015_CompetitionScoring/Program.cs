using System;
using System.Linq;
using System.IO;

namespace _2015_CompetitionScoring
{
	class Program
	{
		static void Main(string[] args)
		{
			// NEVEZÉSEK

			// Nevezések beolvasása
			StreamReader nevezes = new StreamReader("nevezes.txt");

			for (int i = 0; i < File.ReadLines("nevezes.txt").Count(); i++)
				Rendezvény.NevezésBe(nevezes.ReadLine());

			// Rendező iskolák kiválasztása
			Rendezvény.RendezőIskolák();

			// EREDMÉNYEK
			
			// Eredmények beolvasása
			Rendezvény.EredményekBe("eredmeny.txt");

			// Döntősök kiválasztása
			Rendezvény.DöntőbeSorsolás();

			// STATISZTIKA

			// Statisztika készítése
			Rendezvény.StatisztikaKészítése();
		}
	}
}
