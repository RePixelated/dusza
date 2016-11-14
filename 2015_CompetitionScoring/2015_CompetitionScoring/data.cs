using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace _2015_CompetitionScoring
{
	static class Eszközök
	{
		public enum Érvényesség { érvényes, késett, hibásVerseny };

		public static int Dátum(int hónap, int nap)
		{
			int dátum = 0;

			for (int i = 1; i <= hónap; i++)
				dátum += i % 2 == 0 ? (i == 2 ? 28 : 30) : 31;
			dátum += nap;

			return dátum;
		}
		public static T[] SubArray<T> (this T[] data, int index, int length)
		{
			T[] result = new T[length];
			Array.Copy(data, index, result, 0, length);
			return result;
		}
	}

	public static class Rendezvény
	{
		static List<Iskola> iskolák		= new List<Iskola>();
		static List<Csapat> csapatok	= new List<Csapat>();
		static List<Tanár> tanárok		= new List<Tanár>();
		static List<string> megyék		= new List<string>();

		// Érvényes nevezések száma versenyenként
		static int programozók;
		static int robotikusok;

		// Döntősök azonosítói
		static int[] döntősök;
		// Érvényes nevezések azonosítói
		static int[] nevezésekProg1;
		static int[] nevezésekProg2;
		static int[] nevezésekRobot;


		public static void NevezésBe(string forrás)
		{
			string[] narancs = forrás.Split(';');

			// Előkészület
			if (!Iskolák.Létezik(narancs[3]))
				iskolák.Add(new Iskola(narancs[3], narancs[4]));
			if (!megyék.Contains(narancs[4]))
				megyék.Add(narancs[4]);
			if (!Tanárok.Létezik(narancs[narancs.Length - 1]))
				tanárok.Add(new Tanár(narancs[narancs.Length - 1]));

			// Csapat			
			csapatok.Add(new Csapat(
						Convert.ToInt32(narancs[0]), narancs[1],
						Eszközök.Dátum(Convert.ToInt32(narancs[2].Substring(0, 2)),
							Convert.ToInt32(narancs[2].Substring(2, 2))),
						Iskolák.Sorszám(narancs[3]),
						narancs.Length == 8 ? Eszközök.SubArray(narancs, 5, 2) : Eszközök.SubArray(narancs, 5, 6),
						Tanárok.Sorszám(narancs[narancs.Length - 1])));

			NevezésHiba(Convert.ToInt32(narancs[0]), narancs[1], narancs[3]);

			// Iskola
			iskolák[Iskolák.Sorszám(narancs[3])].ÚjCsapat(
					csapatok.Count() - 1,
					csapatok[csapatok.Count() - 1].NevezésÉrvényesség());

			// Tanár
			tanárok[Tanárok.Sorszám(narancs[narancs.Length - 1])].ÚjCsapat(
					csapatok.Count() - 1);
		}
		static void NevezésHiba(int verseny, string csapat, string iskola)
		{
			StreamWriter ervtelen = new StreamWriter("ervtelen.txt", true);

			if (csapatok[csapatok.Count() - 1].NevezésÉrvényesség() == Eszközök.Érvényesség.késett)
			{
				ervtelen.WriteLine("{0};{1};{2};hatarido", verseny, csapat, iskola);
			}
			else if (csapatok[csapatok.Count() - 1].NevezésÉrvényesség() == Eszközök.Érvényesség.hibásVerseny)
			{
				if (verseny == 1)
					ervtelen.WriteLine("{0};{1};{2};harom versenyzo kell", verseny, csapat, iskola);
				else if (verseny == 4)
					ervtelen.WriteLine("{0};{1};{2};egy versenyzo kell", verseny, csapat, iskola);
			}

			ervtelen.Close();
		}
		public static void RendezőIskolák()
		{
			List<string> rendezők = new List<string>();

			// Rendező iskolák kiválasztása
			for(int i = 0; i < megyék.Count(); i++)
				rendezők.Add(Iskolák.Rendező(megyék[i]));

			// Csapatok értesítése
			StreamWriter regio = new StreamWriter("regio.txt");

			for(int i = 0; i < csapatok.Count(); i++)
				if (csapatok[i].NevezésÉrvényesség() == Eszközök.Érvényesség.érvényes)
					regio.WriteLine("{0};{1};{2};{3}",
							csapatok[i].Név(),
							iskolák[csapatok[i].IskolaID()].Név(),
							rendezők[megyék.IndexOf(iskolák[csapatok[i].IskolaID()].Megye())],
							iskolák[csapatok[i].IskolaID()].Megye());

			regio.Close();
		}

		public static void EredményekBe(string fájl)
		{
			StreamReader eredmeny = new StreamReader(fájl);

			// Érvényes nevezések száma versenyenként
			programozók = Convert.ToInt32(eredmeny.ReadLine());
			robotikusok = Convert.ToInt32(eredmeny.ReadLine());

			for (int i = 0; i < (programozók + robotikusok); i++)
			{
				string[] körte = eredmeny.ReadLine().Split(';');

				// Első fordulóban szerzett pontszám beolvasása
				csapatok[Csapatok.Sorszám(körte[0])].PontszámBe(Convert.ToInt32(körte[1]));
			}
		}
		public static void DöntőbeSorsolás()
		{
			döntősök = Csapatok.DöntősökVálasztása();
			int[] döntősökProg = Csapatok.DöntősökProgramozó();
			int[] döntősökRobot = Csapatok.DöntősökRobotika();

			StreamWriter donto = new StreamWriter("donto.txt");

			// Döntőbe jutott programozó versenyesek
			if (döntősökProg.Length > 0)
				donto.Write("1");
			for (int i = 0; i < döntősökProg.Length; i++)
				donto.Write(";{0} {1}",
						csapatok[döntősökProg[i]].Név(),
						csapatok[döntősökProg[i]].Kategória() == 1 ? "I" : "II");

			donto.WriteLine("");

			// Döntőbe jutott robotika versenyesek
			if (döntősökRobot.Length > 0)
				donto.Write("4");
			for (int i = 0; i < döntősökRobot.Length; i++)
				donto.Write(";{0} {1}",
						csapatok[döntősökRobot[i]].Név(),
						csapatok[döntősökRobot[i]].Kategória() == 1 ? "I" : "II");

			donto.Close();
		}

		public static void StatisztikaKészítése()
		{
			// Az érvényes nevezések száma versenyenként, kategóriánként
			Console.WriteLine("--KATEGÓRIÁNKÉNTI NEVEZÉSEK SZÁMA--");
			KategóriánkéntiNevezésekSzáma();

			Console.WriteLine("");

			// A döntősök adatai pontjaik szerinti csökkenő sorrendben
			Console.WriteLine("--DÖNTŐSÖK ADATAI--");
			DöntősökAdatai();

			Console.WriteLine();

			// Döntősök felkészítő tanárainak versenyzőinek száma
			Console.WriteLine("--FELKÉSZÍTŐ TANÁROK VERSENYZŐI--");
			FelkészítőTanárokVersenyzői();
		}
		static void KategóriánkéntiNevezésekSzáma()
		{
			Console.WriteLine("Programozói verseny, I. kategória: {0}", nevezésekProg1.Length);
			Console.WriteLine("Programozói verseny, II. kategória: {0}", nevezésekProg2.Length);
			Console.WriteLine("Robotika verseny: {0}", nevezésekRobot.Length);
		}
		static void DöntősökAdatai()
		{
			Console.WriteLine("A programozói verseny döntősei:");
			int[] prog = Csapatok.DöntősökProgramozó();
			int i = 0;
			while (i < prog.Length)
			{
				Console.WriteLine("{0,-16} {1}",
						csapatok[prog[i]].Név(),
						iskolák[csapatok[prog[i]].IskolaID()].Név());
				i++;
			}

			Console.WriteLine();

			Console.WriteLine("A robotika verseny döntősei:");
			int[] robot = Csapatok.DöntősökRobotika();
			i = 0;
			while (i < robot.Length)
			{
				Console.WriteLine("{0,-16} {1}",
						csapatok[robot[i]].Név(),
						iskolák[csapatok[robot[i]].IskolaID()].Név());
				i++;
			}
		}
		static void FelkészítőTanárokVersenyzői()
		{
			int[] döntősöketFelkészítők = Tanárok.DöntősöketFelkészítők();

			// Döntősöket felkészítő tanárok versenyzőinek száma
			for (int i = 0; i < döntősöketFelkészítők.Length; i++)
				Console.WriteLine("{0,-16} {1}",
						tanárok[döntősöketFelkészítők[i]].Név(),
						Tanárok.Felkészítések(döntősöketFelkészítők[i]));
		}


		static class Iskolák
		{
			public static bool Létezik(string név)
			{
				int i = 0;
				while (i < iskolák.Count() && iskolák[i].Név() != név)
					i++;

				return i < iskolák.Count();
			}
			public static int Sorszám(string név)
			{
				if (Létezik(név))
				{
					int i = 0;
					while (i < iskolák.Count() && iskolák[i].Név() != név)
						i++;

					return i;
				}

				return -1;
			}

			public static string Rendező(string megye)
			{
				int max = 0;
				int maxIndex = 0;

				for(int i = 0; i < iskolák.Count(); i++)
				{
					if (iskolák[i].Megye() == megye)
					{
						if (iskolák[i].ÉrvényesNevezések() > max)
						{
							max = iskolák[i].ÉrvényesNevezések();
							maxIndex = i;
						}
					}
				}

				return iskolák[maxIndex].Név();
			}
		}
		
		static class Tanárok
		{
			public static bool Létezik(string név)
			{
				int i = 0;
				while (i < tanárok.Count() && tanárok[i].Név() != név)
					i++;

				return i < tanárok.Count;
			}
			public static int Sorszám(string név)
			{
				if (Létezik(név))
				{
					int i = 0;
					while (i < tanárok.Count() && tanárok[i].Név() != név)
						i++;

					return i;
				}

				return -1;
			}

			public static int[] DöntősöketFelkészítők()
			{
				List<int> eredmény = new List<int>();

				// Azon tanárok kiválasztása, akik döntős csapatokat készítettek fel
				for (int i = 0; i < döntősök.Length; i++)
					if (!eredmény.Contains(csapatok[döntősök[i]].TanárID()))
						eredmény.Add(csapatok[döntősök[i]].TanárID());

				return eredmény.ToArray();
			}
			public static int Felkészítések(int id)
			{
				int felkészítések = 0;

				// Az adott tanár összes versenyre felkészített diáka
				for (int i = 0; i < csapatok.Count(); i++)
				{
					if (csapatok[i].NevezésÉrvényesség() == Eszközök.Érvényesség.érvényes)
					{
						if (csapatok[i].TanárID() == id)
						{
							if (csapatok[i].Verseny() == 1)
								felkészítések += 3;
							else
								felkészítések += 1;
						}
					}
				}

				return felkészítések;
			}
		}

		static class Csapatok
		{
			public static int Sorszám(string név)
			{
				int i = 0;
				while (i < csapatok.Count() && csapatok[i].Név() != név)
					i++;

				return i;
			}

			public static int[] DöntősökVálasztása()
			{
				// Az első forduló pontjai alapján felállítótt sorrend
				CsapatSorrend();

				// A döntősök kiválasztása a  sorrend alapján
				int prog1Max = nevezésekProg1.Count() < 2 ? nevezésekProg1.Count() : 2;
				int prog2Max = nevezésekProg2.Count() < 3 ? nevezésekProg2.Count() : 3;
				int robotMax = nevezésekRobot.Count() < 5 ? nevezésekRobot.Count() : 5;

				int[] döntősökProg1 = Eszközök.SubArray(nevezésekProg1.ToArray(), 0, prog1Max);
				int[] döntősökProg2 = Eszközök.SubArray(nevezésekProg2.ToArray(), 0, prog2Max);
				int[] döntősökRobot = Eszközök.SubArray(nevezésekRobot.ToArray(), 0, robotMax);

				// A döntősök visszanyerése verseny típustól függetlenül
				// (Ha szükség van rá, akkor később könnyedén kiválasztható a kivánt versenytípusú csapatok listája)
				return SorrendbenEgyesítés(SorrendbenEgyesítés(döntősökProg1, döntősökProg2), döntősökRobot);
			}
			static void CsapatSorrend()
			{
				// Versenytípusonkénti sorrendek tárolója
				List<int> prog1 = new List<int>();
				List<int> prog2 = new List<int>();
				List<int> robot = new List<int>();

				// A versenytípusonkénti sorrend megállapítása
				for (int i = 0; i < csapatok.Count(); i++)
				{
					if (csapatok[i].NevezésÉrvényesség() == Eszközök.Érvényesség.érvényes)
					{
						if (csapatok[i].Verseny() == 1 && csapatok[i].Kategória() == 1)
							prog1 = SorrendbeIllesztés(i, prog1);
						else if (csapatok[i].Verseny() == 1 && csapatok[i].Kategória() == 2)
							prog2 = SorrendbeIllesztés(i, prog2);
						else if (csapatok[i].Verseny() == 4)
							robot = SorrendbeIllesztés(i, robot);
					}
				}

				nevezésekProg1 = prog1.ToArray();
				nevezésekProg2 = prog2.ToArray();
				nevezésekRobot = robot.ToArray();
			}
			static List<int> SorrendbeIllesztés(int id, List<int> sor)
			{
				// Az adat sorban lévő helyének megállapítása
				int i = 0;
				while (i < sor.Count() && csapatok[id].Pontszám() < csapatok[sor[i]].Pontszám())
					i++;

				// Az adat sorba való beillesztése a korábban megállapított helyére
				sor.Insert(i, id);

				return sor;
			}
			static int[] SorrendbenEgyesítés(int[] egyik, int[] másik)
			{
				int[] sorrend = new int[egyik.Length + másik.Length];

				int i = 0;
				int j = 0; int jMax = egyik.Length;
				int k = 0; int kMax = másik.Length;
				
				while (j < jMax && k < kMax)
				{
					if (csapatok[egyik[j]].Pontszám() > csapatok[másik[k]].Pontszám())
					{
						sorrend[i] = egyik[j];
						j++;
					}
					else
					{
						sorrend[i] = másik[k];
						k++;
					}
					i++;
				}

				while (j < jMax)
				{
					sorrend[i] = egyik[j];
					j++; i++;
				}
				while (k < kMax)
				{
					sorrend[i] = másik[k];
					k++; i++;
				}

				return sorrend;
			}

			public static int[] DöntősökProgramozó()
			{
				List<int> döntősökProg = new List<int>();

				// Az összes olyan döntős kiválasztása, aki a programozói versenyre jelentkezett
				for (int i = 0; i < döntősök.Count(); i++)
					if (csapatok[döntősök[i]].Verseny() == 1)
						döntősökProg.Add(döntősök[i]);

				return döntősökProg.ToArray();
			}
			public static int[] DöntősökProgramozó1()
			{
				List<int> döntősökProg1 = new List<int>();

				// Az összes olyan döntős kiválasztása, aki a programozói verseny I. kategóriájába jelentkezett
				for (int i = 0; i < döntősök.Count(); i++)
					if (csapatok[döntősök[i]].Verseny() == 1 && csapatok[döntősök[i]].Kategória() == 1)
						döntősökProg1.Add(döntősök[i]);

				return döntősökProg1.ToArray();
			}
			public static int[] DöntősökProgramozó2()
			{
				List <int> döntősökProg2 = new List<int>();

				// Az összes olyan döntős kiválasztása, aki a programozói verseny II. fordulójára jelentkezett
				for (int i = 0; i < döntősök.Count(); i++)
					if (csapatok[döntősök[i]].Verseny() == 1 && csapatok[döntősök[i]].Kategória() == 2)
						döntősökProg2.Add(döntősök[i]);

				return döntősökProg2.ToArray();
			}
			public static int[] DöntősökRobotika()
			{
				List <int> döntősökRobot = new List<int>();

				// Az összes olyan döntős kiválasztása, aki a robotika versenyre jelentkezett
				for (int i = 0; i < döntősök.Count(); i++)
					if (csapatok[döntősök[i]].Verseny() == 4)
						döntősökRobot.Add(döntősök[i]);

				return döntősökRobot.ToArray();
			}
		}
	}

	class Csapat
	{
		string név;
		int nevIdő;
		int verseny;
		int kategória = 1;
		int pontszám;

		int versenyzőkSzáma;
		Versenyző[] versenyzők;
		int iskolaID;
		int tanárID;


		public Csapat(int verseny, string név, int nevIdő, int iskolaID, string[] versenyzőAdatok, int tanárID)
		{
			// Általános jellemzók
			this.verseny = verseny;
			this.név = név;
			this.nevIdő = nevIdő;

			this.iskolaID = iskolaID;
			this.tanárID = tanárID;

			// Csapat tagjai
			versenyzőkSzáma = versenyzőAdatok.Length / 2;
			if(versenyzőkSzáma == 1)
				versenyzők = new Versenyző[1];
			else
				versenyzők = new Versenyző[3];
			for (int i = 0; i < versenyzőkSzáma; i++)
			{
				versenyzők[i] = new Versenyző(
						versenyzőAdatok[i * 2],
						Convert.ToInt32(versenyzőAdatok[(i * 2) + 1]));

				if (Convert.ToInt32(versenyzőAdatok[(i * 2) + 1]) > 10)
					kategória = 2;
			}
		}
		public Eszközök.Érvényesség NevezésÉrvényesség()
		{
			if ((verseny == 1 && versenyzőkSzáma != 3) ||
					(verseny == 4 && versenyzőkSzáma != 1))
				return Eszközök.Érvényesség.hibásVerseny;
			else if ((verseny == 1 && nevIdő > Eszközök.Dátum(10, 15)) ||
					(verseny == 4 && nevIdő > Eszközök.Dátum(10, 10)))
				return Eszközök.Érvényesség.késett;
			
			return Eszközök.Érvényesség.érvényes;
		}
		public bool VanIdősebb()
		{
			// Megállapítja, hogy van-e olyan csapat tag, aki 10, évfolyamba, vagy annál nagyobb osztályba jár
			// (Ez a verseny kategóriájának megállapításához kell, ha a csapat a programozói versenyre jelentkezett)
			int i = 0;
			while (i < versenyzők.Length && versenyzők[i].Évfolyam() <= 10)
				i++;

			return i < versenyzők.Length;
		}

		public string Név() { return név; }
		public int IskolaID() { return iskolaID; }
		public int TanárID() { return tanárID; }
		public int Verseny() { return verseny; }
		public int Kategória() { return kategória; }
		public int Pontszám() { return pontszám; }

		public void PontszámBe(int érték) { pontszám = érték; }
	}

	class Iskola
	{
		string név;
		string megye;
		List<int> csapatok = new List<int>();
		int érvényesNevezések = 0;

		public Iskola(string név, string megye)
		{
			this.név = név;
			this.megye = megye;
		}
		public void ÚjCsapat(int csapatID, Eszközök.Érvényesség érvényesség)
		{
			// Egy új csapat feljegyzése az iskolához
			csapatok.Add(csapatID);
			// A csapat érvényes nevezése esetén egy érvényes nevezés regisztrálása
			if (érvényesség == Eszközök.Érvényesség.érvényes)
				érvényesNevezések++;
		}

		public string Név() { return név; }
		public string Megye() { return megye; }

		public int ÉrvényesNevezések() { return érvényesNevezések; }
		public int ŐsszesCsapatSzám() { return csapatok.Count(); }
	}

	class Versenyző
	{
		string név;
		int évfolyam;

		public Versenyző(string név, int évfolyam)
		{
			this.név = név;
			this.évfolyam = évfolyam;
		}

		public int Évfolyam() { return évfolyam; }
	}

	class Tanár
	{
		string név;
		List<int> csapatok = new List<int>();

		public Tanár(string név)
		{
			this.név = név;
		}
		public void ÚjCsapat(int csapatID)
		{
			// Egy új csapat feljegyzése a tanárhoz
			csapatok.Add(csapatID);
		}

		public string Név() { return név; }
	}
}
