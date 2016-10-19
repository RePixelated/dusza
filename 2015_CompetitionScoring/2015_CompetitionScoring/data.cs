using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace _2015_CompetitionScoring
{
	static class Adatok
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

		public Iskola[] Rendezők()
		{
			Iskola[] iskolák = Iskolák();
			string[] megyék = Megyék(iskolák);
			List<Iskola> rendezők = new List<Iskola>();

			for (int i = 0; i < megyék.Length; i++)
			{
				int max = 0;
				int index = 0;

				for (int j = 0; j < iskolák.Length; j++)
				{
					if (iskolák[j].Megye() == megyék[i])
					{
						if (max < iskolák[j].Jelentkezők())
							max = iskolák[j].Jelentkezők();
						index = j;
					}
				}

				rendezők.Add(iskolák[index]);
			}

			return rendezők.ToArray();
		}

		string[] Megyék(Iskola[] iskolák)
		{
			List<string> megyék = new List<string>();

			for (int i = 0; i < iskolák.Length; i++)
			{
				int j = 0;
				while (j < megyék.Count && megyék[j] != iskolák[i].Megye())
					j++;

				if (j >= megyék.Count)
					megyék.Add(iskolák[i].Megye());
			}

			return megyék.ToArray();
		}

		Iskola[] Iskolák()
		{
			List<Iskola> iskolák = new List<Iskola>();

			/// Iskolák beolvasása
			for (int i = 0; i < nevezések.Count; i++)
			{
				int j = 0;
				while (j < iskolák.Count && iskolák[j].Név() != nevezések[i].Iskola())
					j++;

				if (j >= iskolák.Count)
					iskolák.Add(new Iskola(nevezések[i].Iskola(), nevezések[i].Megye()));
			}

			/// Iskolák nevezéseinek beolvasása
			for (int i = 0; i < nevezések.Count; i++)
			{
				if (nevezések[i].Érvényesség() == _2015_CompetitionScoring.Nevezés.Státusz.Érvényes)
				{
					for (int j = 0; j  < iskolák.Count; j++)
					{
						if (nevezések[i].Megye() != iskolák[j].Megye())
							iskolák[j].ÚjCsapat();
					}
				}
			}

			return iskolák.ToArray();
		}

		public Nevezés Nevezés(int i) { return nevezések[i]; }
	}

	class Nevezés
	{
		int verseny;
		string csapat;
		int nevIdő;
		string iskola;
		string megye;
		string diák;
		List<string> diákok = new List<string>();
		int évfolyam;
		string tanár;

		public enum Státusz { Érvényes, Késett, RosszVerseny };
		Státusz érvényesség = new Státusz();

		public Nevezés(string forrás)
		{
			string[] narancs = forrás.Split(';');

			/// Verseny kódja
			verseny = Convert.ToInt32(narancs[0]);

			/// Csapat neve
			csapat = narancs[1];

			/// Nevezés ideje
			int hónap = Convert.ToInt32(narancs[2].Substring(0, 2));
			int nap = Convert.ToInt32(narancs[2].Substring(2, 2));

			nevIdő = Dátum(hónap, nap);

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


			/// Hiba ellenőrzés
			if ((verseny == 1 && narancs.Count() != 12) ||
					(verseny == 4 && narancs.Count() != 8))
			{
				érvényesség = Státusz.RosszVerseny;
			}
			else if ((verseny == 1 && nevIdő > Dátum(10, 15)) ||
					(verseny == 4 && nevIdő > Dátum(10, 10)))
			{
				érvényesség = Státusz.Késett;
			}
			else
			{
				érvényesség = Státusz.Érvényes;
			}

			Hiba();
		}

		void Hiba()
		{
			StreamWriter cél = new StreamWriter("ervtelen.txt", true);

			if (érvényesség == Státusz.Késett)
			{
				cél.WriteLine("{0};{1};{2};hatarido", verseny, csapat, iskola);
			}
			else if (érvényesség == Státusz.RosszVerseny)
			{
				if (verseny == 1)
					cél.WriteLine("{0};{1};{2};harom versenyzo kell", verseny, csapat, iskola);
				if (verseny == 4)
					cél.WriteLine("{0};{1};{2};egy versenyzo kell", verseny, csapat, iskola);
			}

			cél.Close();
		}

		int Dátum(int hónap, int nap)
		{
			int dátum = 0;

			for (int i = 1; i <= hónap; i++)
				dátum += i % 2 == 0 ? (i == 2 ? 28 : 30) : 31;
			dátum += nap;

			return dátum;
		}

		public string Iskola() {return iskola; }
		public string Megye() { return megye; }
		public int Évfolyam() { return évfolyam; }
		public Státusz Érvényesség() { return érvényesség; }
	}

	class Iskola
	{
		string név;
		string megye;
		int jelentkezők;

		public Iskola (string név, string megye)
		{
			this.név = név;
			this.megye = megye;
		}

		public void ÚjCsapat() { jelentkezők++; }

		public string Név() { return név; }
		public string Megye() { return megye; }
		public int Jelentkezők() { return jelentkezők; }
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
