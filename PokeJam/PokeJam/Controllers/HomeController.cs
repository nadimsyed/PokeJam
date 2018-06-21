using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PokeJam.Models;
using Microsoft.AspNet.Identity;

namespace PokeJam.Controllers
{
    public class HomeController : Controller
    {
        private PokeJamEntities db = new PokeJamEntities();

        [Authorize]
        public ActionResult Index()
        {
            Session["UserID"] = User.Identity.GetUserId();

            Session["User"] = 0;
            Session["Comp"] = 0;

            List<AspNetUser> aspNetUsers = (from a in db.AspNetUsers
                                            select a).ToList();
            
            for (int i = 0; i < aspNetUsers.Count; i++)
            {
                Session["I"] = aspNetUsers[i].Id;
            }

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
                ViewBag.NameProp = Methods.UppercaseFirst(name);

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

                int SA = Methods.StatConverter(specialAtt);
                int A = (int.Parse(att)) / 3 + 10;
                int S = (int.Parse(speed)) / 3 + 15;
                int SD = (int.Parse(specialDef)) / 3 - 20;
                int D = (int.Parse(def)) / 3 - 15;


                ViewBag.ThreePoint = SA;
                ViewBag.FieldGoal = A;
                ViewBag.Paint = S;
                ViewBag.Steal = SD;
                ViewBag.Block = D;

                int x = Methods.Generator();

                ViewBag.Number = x;

                int StealTester = SD + 5;

                bool success = Methods.StealBlockConfirm(SD, StealTester);

                ViewBag.Success = success;

                //130-300 for weight
                Random random = new Random();
                int w = random.Next(130, 301);
                int z = Methods.WeightReturn(w);

                ViewBag.Weight = z;

                int SA2 = Methods.WeightToSA(z);
                int A2 = Methods.WeightToA(z);
                int S2 = Methods.WeightToS(z);
                int SD2 = (Methods.WeightToSA(z)) / 2;
                int D2 = (Methods.WeightToA(z)) / 2;

                int SDx = random.Next(SD2 - 2, SD2 + 6);
                int Dx = random.Next(D2 - 2, D2 + 6);


                ViewBag.ThreePointW = SA2;
                ViewBag.FieldGoalW = A2;
                ViewBag.PaintW = S2;
                ViewBag.StealW = SDx;
                ViewBag.BlockW = Dx;

                //height 60-96 inches
                int h = random.Next(60, 97);
                int a = Methods.HeightReturn(h);

                ViewBag.Height = a;

                int SA3 = Methods.HeightToSA(a);
                int A3 = Methods.HeightToA(a);
                int S3 = Methods.HeightToS(a);
                int SD3 = (Methods.HeightToSA(a)) / 2;
                int D3 = (Methods.HeightToA(a)) / 2;

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
            ViewBag.CharName = CharName;
            ViewBag.CharHeight = CharHeight;
            ViewBag.CharWeight = CharWeight;

            Random random = new Random();


            int z = Methods.WeightReturn((int)CharWeight);

            ViewBag.Weight = z;

            int SA2 = Methods.WeightToSA(z);
            int A2 = Methods.WeightToA(z);
            int S2 = Methods.WeightToS(z);
            int SD2 = (Methods.WeightToSA(z)) / 2;
            int D2 = (Methods.WeightToA(z)) / 2;

            int SDx = random.Next(SD2 - 2, SD2 + 6);
            int Dx = random.Next(D2 - 2, D2 + 6);


            ViewBag.ThreePointW = SA2;
            ViewBag.FieldGoalW = A2;
            ViewBag.PaintW = S2;
            ViewBag.StealW = SDx;
            ViewBag.BlockW = Dx;

            int a = Methods.HeightReturn((int)CharHeight);

            ViewBag.Height = a;

            int SA3 = Methods.HeightToSA(a);
            int A3 = Methods.HeightToA(a);
            int S3 = Methods.HeightToS(a);
            int SD3 = (Methods.HeightToSA(a)) / 2;
            int D3 = (Methods.HeightToA(a)) / 2;

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
        public ActionResult SuccessfullyCreatedChar(string CharName, int CharHeight,  int CharWeight, int ThreePoint, int FieldGoat, int Paint, int Steal, int Block)
        {
            if (CharName != null)
            {
                Character character = new Character();
                //The ID will be the ID from the identity if we make that switch
                //character.UserID = 2;
                character.Id = (string)Session["I"];
                character.CharName = CharName;
                character.Height = CharHeight;
                character.Weight = CharWeight;
                character.ThreePoint = ThreePoint;
                character.FieldGoal = FieldGoat;
                character.Paint = Paint;
                character.Steal = Steal;
                character.Block = Block;

                db.Characters.Add(character);
                db.SaveChanges();
                List<Character> characters = new List<Character>();
                characters.Add(character);
                ViewBag.Char = characters;

                Session["Char"] = character.CharID;
            }


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
    }
}