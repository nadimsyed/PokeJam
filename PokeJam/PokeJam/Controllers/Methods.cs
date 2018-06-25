using PokeJam.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PokeJam.Controllers
{
    public class Methods
    {
        private static PokeJamEntities db = new PokeJamEntities();

        public static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public static int StatConverter(string x)
        {
            return (int.Parse(x)) / 3;
        }

        public static int Generator()
        {
            Random random = new Random();

            return random.Next(0, 101);
        }

        public static bool ShotConfirm(int shot)
        {

            int x = Generator();

            if (x > shot)
            {
                return false;
            }
            return true;
        }

        public static bool StealBlockConfirm(int offense, int defense)
        {
            int x = Generator();

            if (offense > defense)
            {
                if (x + 5 >= offense)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else if (defense >= offense)
            {
                if (x - 5 > defense)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return true;
        }

        public static int WeightReturn(int weight)
        {
            if (weight >= 130 && weight < 140)
            {
                return 1;
            }
            else if (weight >= 140 && weight < 150)
            {
                return 2;

            }
            else if (weight >= 150 && weight < 160)
            {
                return 3;

            }
            else if (weight >= 160 && weight < 170)
            {
                return 4;

            }
            else if (weight >= 170 && weight < 180)
            {
                return 5;

            }
            else if (weight >= 180 && weight < 190)
            {
                return 6;

            }
            else if (weight >= 200 && weight < 210)
            {
                return 7;

            }
            else if (weight >= 210 && weight < 220)
            {
                return 8;

            }
            else if (weight >= 220 && weight < 230)
            {
                return 9;

            }
            else if (weight >= 230 && weight < 240)
            {
                return 10;

            }
            else if (weight >= 240 && weight < 250)
            {
                return 11;

            }
            else if (weight >= 250 && weight < 260)
            {
                return 12;

            }
            else if (weight >= 260 && weight < 270)
            {
                return 13;

            }
            else if (weight >= 270 && weight < 280)
            {
                return 14;

            }
            else if (weight >= 280 && weight < 290)
            {
                return 15;

            }
            else if (weight >= 290 && weight <= 300)
            {
                return 16;

            }
            return 0;
        }

        public static int HeightReturn(int height)
        {
            int x = 0;
            if (height >= 60 && height < 63)
            {
                x = 1;
            }
            else if (height >= 63 && height < 66)
            {
                x = 2;
            }
            else if (height >= 66 && height < 69)
            {
                x = 3;
            }
            else if (height >= 69 && height < 72)
            {
                x = 4;
            }
            else if (height >= 72 && height < 75)
            {
                x = 5;
            }
            else if (height >= 75 && height < 78)
            {
                x = 6;
            }
            else if (height >= 78 && height < 81)
            {
                x = 7;
            }
            else if (height >= 81 && height < 84)
            {
                x = 8;
            }
            else if (height >= 84 && height < 87)
            {
                x = 9;
            }
            else if (height >= 87 && height <= 90)
            {
                x = 10;
            }
            return x;
        }

        public static int WeightToSA(int weight)
        {
            int x = 0;
            if (weight == 1)
            {
                x = 16;
            }
            else if (weight == 2)
            {
                x = 15;
            }
            else if (weight == 3)
            {
                x = 14;
            }
            else if (weight == 4)
            {
                x = 13;
            }
            else if (weight == 5)
            {
                x = 12;
            }
            else if (weight == 6)
            {
                x = 11;
            }
            else if (weight == 7)
            {
                x = 10;
            }
            else if (weight == 8)
            {
                x = 9;
            }
            else if (weight == 9)
            {
                x = 8;
            }
            else if (weight == 10)
            {
                x = 7;
            }
            else if (weight == 11)
            {
                x = 6;
            }
            else if (weight == 12)
            {
                x = 5;
            }
            else if (weight == 13 || weight == 14 || weight == 15 || weight == 16)
            {
                x = 4;
            }
            return x;
        }

        public static int HeightToSA(int height)
        {
            int x = 0;
            if (height == 1)
            {
                x = 16;
            }
            else if (height == 2)
            {
                x = 15;
            }
            else if (height == 3)
            {
                x = 14;
            }
            else if (height == 4)
            {
                x = 13;
            }
            else if (height == 5)
            {
                x = 12;
            }
            else if (height == 6)
            {
                x = 11;
            }
            else if (height == 7)
            {
                x = 10;
            }
            else if (height == 8)
            {
                x = 9;
            }
            else if (height == 9)
            {
                x = 8;
            }
            else if (height == 10)
            {
                x = 7;
            }
            return x;
        }

        public static int WeightToA(int weight)
        {
            int x = 0;
            if (weight == 1 || weight == 16)
            {
                x = 8;
            }
            else if (weight == 2 || weight == 15)
            {
                x = 9;
            }
            else if (weight == 3 || weight == 14)
            {
                x = 10;
            }
            else if (weight == 4 || weight == 13)
            {
                x = 11;
            }
            else if (weight == 5 || weight == 12)
            {
                x = 12;
            }
            else if (weight == 6 || weight == 11)
            {
                x = 13;
            }
            else if (weight == 7 || weight == 10)
            {
                x = 14;
            }
            else if (weight == 8)
            {
                x = 15;
            }
            else if (weight == 9)
            {
                x = 16;
            }
            return x;
        }

        public static int HeightToA(int height)
        {
            int x = 0;
            if (height == 1)
            {
                x = 7;
            }
            else if (height == 2)
            {
                x = 8;
            }
            else if (height == 3)
            {
                x = 9;
            }
            else if (height == 4)
            {
                x = 10;
            }
            else if (height == 5)
            {
                x = 11;
            }
            else if (height == 6)
            {
                x = 12;
            }
            else if (height == 7)
            {
                x = 13;
            }
            else if (height == 8)
            {
                x = 14;
            }
            else if (height == 9)
            {
                x = 15;
            }
            else if (height == 10)
            {
                x = 16;
            }
            return x;
        }

        public static int WeightToS(int weight)
        {
            int x = 0;
            if (weight == 1 || weight == 10)
            {
                x = 12;
            }
            else if (weight == 2 || weight == 9)
            {
                x = 13;
            }
            else if (weight == 3 || weight == 8)
            {
                x = 14;
            }
            else if (weight == 4 || weight == 7)
            {
                x = 15;
            }
            else if (weight == 5 || weight == 6)
            {
                x = 16;
            }
            else if (weight == 11 || weight == 12)
            {
                x = 7;
            }
            else if (weight == 13)
            {
                x = 6;
            }
            else if (weight == 13 || weight == 14 || weight == 15 || weight == 16)
            {
                x = 15;
            }
            return x;
        }

        public static int HeightToS(int height)
        {
            int x = 0;
            if (height == 1 || height == 2 || height == 3)
            {
                x = 7;
            }
            else if (height == 4)
            {
                x = 8;
            }
            else if (height == 5)
            {
                x = 9;
            }
            else if (height == 6)
            {
                x = 10;
            }
            else if (height == 7)
            {
                x = 11;
            }
            else if (height == 8)
            {
                x = 12;
            }
            else if (height == 9)
            {
                x = 13;
            }
            else if (height == 10)
            {
                x = 14;
            }
            return x;
        }

        public static void AddTournament(int TierCount, bool[] track, string[] pokeTrack, string UserId)
        {
            Tournament tournament = new Tournament();
            tournament.Id = UserId;
            tournament.T1 = track[0];
            tournament.T2 = track[1];
            tournament.T3 = track[2];
            tournament.T4 = track[3];
            tournament.T5 = track[4];
            tournament.P1 = pokeTrack[0];
            tournament.P2 = pokeTrack[1];
            tournament.P3 = pokeTrack[2];
            tournament.P4 = pokeTrack[3];
            tournament.P5 = pokeTrack[4];

            db.Tournaments.Add(tournament);
            db.SaveChanges();
        }
    }
}