using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace PokeJam.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            HttpWebRequest WR = WebRequest.CreateHttp("https://pokeapi.co/api/v2/pokemon/9/");
            WR.UserAgent = ".NET Framework Test Client";

            HttpWebResponse Response;

            try
            {
                Response = (HttpWebResponse)WR.GetResponse();
            }
            catch (WebException e)
            {
                ViewBag.Error = "Exception";
                ViewBag.ErrorDescription = e.Message;
                return View();
            }

            if (Response.StatusCode != HttpStatusCode.OK)
            {
                ViewBag.Error = Response.StatusCode;
                ViewBag.ErrorDescription = Response.StatusDescription;
                return View();
            }

            StreamReader reader = new StreamReader(Response.GetResponseStream());
            string PokemonData = reader.ReadToEnd();

            try
            {
                JObject JsonData = JObject.Parse(PokemonData);
                ViewBag.Name = JsonData["forms"][0];
                string name = ViewBag.Name.name;
                ViewBag.NameProp = UppercaseFirst(name);

                ViewBag.Speed = JsonData["stats"][0];
                ViewBag.SpecialDef = JsonData["stats"][1];
                ViewBag.SpecialAttack = JsonData["stats"][2];
                ViewBag.Def = JsonData["stats"][3];
                ViewBag.Attack = JsonData["stats"][4];
                ViewBag.HP = JsonData["stats"][5];
                ViewBag.Image = JsonData["sprites"];

                string specialAtt = (string)JsonData["stats"][2]["base_stat"];
                string att = (string)JsonData["stats"][4]["base_stat"];
                string speed = (string)JsonData["stats"][0]["base_stat"];
                string specialDef = (string)JsonData["stats"][1]["base_stat"];
                string def = (string)JsonData["stats"][3]["base_stat"];

                int SA = StatConverter(specialAtt);
                int A = (int.Parse(att)) / 3 + 10;
                int S = (int.Parse(speed)) / 3 + 15;
                int SD = (int.Parse(specialDef)) / 3 - 20;
                int D = (int.Parse(def)) / 3 - 15;


                ViewBag.ThreePoint = SA;
                ViewBag.FieldGoal = A;
                ViewBag.Paint = S;
                ViewBag.Steal = SD;
                ViewBag.Block = D;

                int x = Generator();

                ViewBag.Number = x;

                int StealTester = SD + 5;

                bool success = StealBlockConfirm(SD, StealTester);

                ViewBag.Success = success;

                //130-300 for weight
                Random random = new Random();
                int w = random.Next(130, 301);
                int z = WeightReturn(w);

                ViewBag.Weight = z;

                int SA2 = WeightToSA(z);
                int A2 = WeightToA(z);
                int S2 = WeightToS(z);
                int SD2 = (WeightToSA(z)) / 2;
                int D2 = (WeightToA(z)) / 2;

                int SDx = random.Next(SD2 - 2, SD2 + 6);
                int Dx = random.Next(D2 - 2, D2 + 6);


                ViewBag.ThreePointW = SA2;
                ViewBag.FieldGoalW = A2;
                ViewBag.PaintW = S2;
                ViewBag.StealW = SDx;
                ViewBag.BlockW = Dx;

                //height 60-96 inches
                int h = random.Next(60, 97);
                int a = HeightReturn(h);

                ViewBag.Height = a;

                int SA3 = HeightToSA(a);
                int A3 = HeightToA(a);
                int S3 = HeightToS(a);
                int SD3 = (HeightToSA(a)) / 2;
                int D3 = (HeightToA(a)) / 2;

                int SDy = random.Next(SD3 - 2, SD3 + 4);
                int Dy = random.Next(D3 - 2, D3 + 4);


                ViewBag.ThreePointH = SA3;
                ViewBag.FieldGoalH = A3;
                ViewBag.PaintH = S3;
                ViewBag.StealH = SDy;
                ViewBag.BlockH = Dy;

                ViewBag.ThreePointWH = SA2 + SA3;
                ViewBag.FieldGoalWH = A2 + A3;
                ViewBag.PaintWH = S2 + S3;
                ViewBag.StealWH = SDx + SDy;
                ViewBag.BlockWH = Dx + Dy;

                //Session["SkillPoints"] = 100;


            }
            catch (Exception e)
            {
                ViewBag.Error = "JSON Issue";
                ViewBag.ErrorDescription = e.Message;
                return View();
            }

            return View();
        }

        public ActionResult SkillPoint(int ThreePoint, int FieldGoat, int Paint, int Steal, int Block)
        {

            return View();
        }

        public ActionResult CreatePlayerStart()
        {

            return View();
        }

        public ActionResult CreatePlayer(string CharName, int? CharHeight, int? CharWeight)
        {
            Random random = new Random();


            int z = WeightReturn((int)CharWeight);

            ViewBag.Weight = z;

            int SA2 = WeightToSA(z);
            int A2 = WeightToA(z);
            int S2 = WeightToS(z);
            int SD2 = (WeightToSA(z)) / 2;
            int D2 = (WeightToA(z)) / 2;

            int SDx = random.Next(SD2 - 2, SD2 + 6);
            int Dx = random.Next(D2 - 2, D2 + 6);


            ViewBag.ThreePointW = SA2;
            ViewBag.FieldGoalW = A2;
            ViewBag.PaintW = S2;
            ViewBag.StealW = SDx;
            ViewBag.BlockW = Dx;

            int a = HeightReturn((int)CharHeight);

            ViewBag.Height = a;

            int SA3 = HeightToSA(a);
            int A3 = HeightToA(a);
            int S3 = HeightToS(a);
            int SD3 = (HeightToSA(a)) / 2;
            int D3 = (HeightToA(a)) / 2;

            int SDy = random.Next(SD3 - 2, SD3 + 4);
            int Dy = random.Next(D3 - 2, D3 + 4);


            ViewBag.ThreePointH = SA3;
            ViewBag.FieldGoalH = A3;
            ViewBag.PaintH = S3;
            ViewBag.StealH = SDy;
            ViewBag.BlockH = Dy;

            ViewBag.ThreePointWH = SA2 + SA3;
            ViewBag.FieldGoalWH = A2 + A3;
            ViewBag.PaintWH = S2 + S3;
            ViewBag.StealWH = SDx + SDy;
            ViewBag.BlockWH = Dx + Dy;

            return View();
        }

        //Might have to pass in CharName, CharHeight, CharWeight from previous action to this action to make the character in database
        public ActionResult SuccessfullyCreatedChar(int ThreePoint, int FieldGoat, int Paint, int Steal, int Block)
        {

            return View();
        }

        public ActionResult PokeJam()
        {
            return View();
        }

        public ActionResult Rules()
        {
            return View();
        }

        public ActionResult ViewStats()
        {
            return View();
        }

        public ActionResult SinglePlayer()
        {
            return View();
        }

        public ActionResult Tournament()
        {
            return View();
        }

        public ActionResult Tier1()
        {
            return View();
        }

        public ActionResult Tier2()
        {
            return View();
        }

        public ActionResult Tier3()
        {
            return View();
        }

        public ActionResult Tier4()
        {
            return View();
        }

        public ActionResult Tier5()
        {
            return View();
        }

        public ActionResult GamePlay()
        {
            return View();
        }

        public ActionResult PlayConclusion(string shot)
        {
            ViewBag.Shot = shot;
            return View();
        }
        public ActionResult Tier5Congratulations()
        {

            return View();
        }

        public ActionResult TournamentLoss()
        {

            return View();
        }
        public ActionResult TournamentNext()
        {

            return View();
        }
        public ActionResult SingleNext()
        {

            return View();
        }
        public ActionResult GameplayResult()
        {

            return View();
        }
        public ActionResult Conclusion3()
        {

            return View();
        }
        public ActionResult Conclusion2()
        {

            return View();
        }



        static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        static int StatConverter(string x)
        {
            return (int.Parse(x)) / 3;
        }

        static int Generator()
        {
            Random random = new Random();

            return random.Next(0, 101);
        }

        static bool ShotConfirm(int shot)
        {

            int x = Generator();

            if (x > shot)
            {
                return false;
            }
            return true;
        }

        static bool StealBlockConfirm(int offense, int defense)
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

        static int WeightReturn(int weight)
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

        static int HeightReturn(int height)
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

        static int WeightToSA(int weight)
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

        static int HeightToSA(int height)
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

        static int WeightToA(int weight)
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

        static int HeightToA(int height)
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

        static int WeightToS(int weight)
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

        static int HeightToS(int height)
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
    }
}