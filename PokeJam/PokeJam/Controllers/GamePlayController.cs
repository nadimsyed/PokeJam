﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using PokeJam.Models;

namespace PokeJam.Controllers
{
    public class GamePlayController : Controller
    {
        private PokeJamEntities db = new PokeJamEntities();

        // GET: GamePlay
        public ActionResult Index()
        {
            Session["User"] = 0;
            Session["Comp"] = 0;

            return View();
        }

        public ActionResult SinglePlayer()
        {
            List<PokeTier> all = db.PokeTiers.ToList();

            ViewBag.All = all;
                
            return View();
            
        }

        public ActionResult Tier1()
        {
            List<PokeTier> tiers = db.PokeTiers.Where(
                p => p.Tiers == 1).ToList();

            ViewBag.Poke = tiers;

            return View();
        }

        public ActionResult Tier2()
        {
            List<PokeTier> tiers = db.PokeTiers.Where(
                p => p.Tiers == 2).ToList();

            ViewBag.Poke = tiers;

            return View();
        }

        public ActionResult Tier3()
        {
            List<PokeTier> tiers = db.PokeTiers.Where(
                p => p.Tiers == 3).ToList();

            ViewBag.Poke = tiers;

            return View();
        }

        public ActionResult Tier4()
        {
            List<PokeTier> tiers = db.PokeTiers.Where(
                p => p.Tiers == 4).ToList();

            ViewBag.Poke = tiers;

            return View();
        }

        public ActionResult Tier5()
        {
            List<PokeTier> tiers = db.PokeTiers.Where(
                p => p.Tiers == 5).ToList();

            ViewBag.Poke = tiers;

            return View();
        }

        public ActionResult GamePlay(int pokemon, string Coin, string PlayType = "")
        {
            ViewBag.PlayType = PlayType;
            if (Session["PlayType"] == null)
            {
                Session.Add("PlayType", ViewBag.PlayType);
            }
            PlayType = (string)Session["PlayType"];
            Session["PlayType"] = PlayType;

            Session["Pokemon"] = pokemon;

            ViewBag.Coin = Coin;
            ViewBag.Choice = pokemon;

            return View();
        }
       

        public ActionResult PlayConclusion(string shot)
        {
            ViewBag.Shot = shot;

            int ID = (int)Session["Char"];


            Character character = db.Characters.Where(
                c => c.CharID == ID).Single();

            int StealC = character.Steal;
            int BlockC = character.Block;

            int PID = (int)Session["Pokemon"];

            HttpWebRequest WR = WebRequest.CreateHttp($"https://pokeapi.co/api/v2/pokemon/{PID}/");
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

                string specialAtt = (string)JsonData["stats"][2]["base_stat"];
                string att = (string)JsonData["stats"][4]["base_stat"];
                string speed = (string)JsonData["stats"][0]["base_stat"];
                string specialDef = (string)JsonData["stats"][1]["base_stat"];
                string def = (string)JsonData["stats"][3]["base_stat"];

                //TODO: Replace all the int.parse with the method
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

                //int StealTester = SD + 5;

                //bool success = Methods.StealBlockConfirm(SD, StealTester);

                Random random = new Random();
                int which = random.Next(1, 3);

                if (which == 1)
                {
                    bool success = Methods.StealBlockConfirm(StealC, SD);

                    ViewBag.Which = 1;
                    ViewBag.Success = success;
                }
                else if (which == 2)
                {
                    bool success = Methods.StealBlockConfirm(BlockC, SD);

                    ViewBag.Which = 2;
                    ViewBag.Success = success;
                }

            }
            catch (Exception e)
            {
                ViewBag.Error = "JSON Issue";
                ViewBag.ErrorDescription = e.Message;
                return View();
            }


            if (shot == "ThreePoint")
            {
                bool truth = Methods.ShotConfirm(character.ThreePoint);
                string made =  truth ? "Shot went in!": "Shot missed!";
                ViewBag.Made = made;
                if (truth)
                {
                    Session["User"] = 3;
                }
            }
            else if (shot == "MidRange")
            {
                bool truth = Methods.ShotConfirm(character.FieldGoal);
                string made =  ? "Shot went in!" : "Shot missed!";
                ViewBag.Made = made;
                if (truth)
                {
                    Session["User"] = 2;
                }
            }
            else if (shot == "Paint")
            {
                bool truth = Methods.ShotConfirm(character.Paint);
                string made =  ? "Shot went in!" : "Shot missed!";
                ViewBag.Made = made;
                if (truth)
                {
                    Session["User"] = 2;
                }
            }

            return View();
        }

        public ActionResult PlayConclusion2()
        {
            int ThreePoint = 0;
            int FieldGoal = 0;
            int Paint = 0;

            Random random = new Random();
            int iShot = random.Next(1, 4);
            string shot = "";

            if (iShot == 1)
            {
                shot = "ThreePoint";
            }
            else if (iShot == 2)
            {
                shot = "MidRange";
            }
            else if (iShot == 3)
            {
                shot = "Paint";
            }
            int ID = (int)Session["Char"];

            Character character = db.Characters.Where(
                c => c.CharID == ID).Single();

            int StealC = character.Steal;
            int BlockC = character.Block;

            int PID = (int)Session["Pokemon"];

            //TODO: Ask whats worse practice, making a bunch of sessions and storing the Pokemons Basketball Stats, or a second request to the API
            HttpWebRequest WR = WebRequest.CreateHttp($"https://pokeapi.co/api/v2/pokemon/{PID}/");
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

                ThreePoint = SA;
                FieldGoal = A;
                Paint = S;

                ViewBag.ThreePoint = SA;
                ViewBag.FieldGoal = A;
                ViewBag.Paint = S;
                ViewBag.Steal = SD;
                ViewBag.Block = D;

                int x = Methods.Generator();

                ViewBag.Number = x;

                //int StealTester = SD + 5;

                //bool success = Methods.StealBlockConfirm(SD, StealTester);

                int which = random.Next(1, 3);

                if (which == 1)
                {
                    bool success = Methods.StealBlockConfirm(SD, StealC);

                    ViewBag.Which = 1;
                    ViewBag.Success = success;
                }
                else if (which == 2)
                {
                    bool success = Methods.StealBlockConfirm(SD, BlockC);

                    ViewBag.Which = 2;
                    ViewBag.Success = success;
                }

            }
            catch (Exception e)
            {
                ViewBag.Error = "JSON Issue";
                ViewBag.ErrorDescription = e.Message;
                return View();
            }


            if (shot == "ThreePoint")
            {
                //TODO: For past MVP, insert Pokemon name, can store that name and just use as Variable for replacement
                bool truth = Methods.ShotConfirm(ThreePoint);
                string made = truth ? "Pokemon's shot went in!" : "Pokemon's shot missed!";
                ViewBag.Made = made;
                if (truth)
                {
                    Session["Comp"] = 3;
                }
            }
            else if (shot == "MidRange")
            {
                bool truth = Methods.ShotConfirm(FieldGoal);
                string made =  truth ? "Pokemon's shot went in!" : "Pokemon's shot missed!";
                ViewBag.Made = made;
                if (truth)
                {
                    Session["Comp"] = 2;
                }
            }
            else if (shot == "Paint")
            {
                bool truth = Methods.ShotConfirm(Paint);
                string made = truth ? "Pokemon's shot went in!" : "Pokemon's shot missed!";
                ViewBag.Made = made;
                if (truth)
                {
                    Session["Comp"] = 2;
                }
            }

            return View();
        }

        public ActionResult PlayConclusion3(string shot)
        {
            ViewBag.Shot = shot;

            int ID = (int)Session["Char"];


            Character character = db.Characters.Where(
                c => c.CharID == ID).Single();

            int StealC = character.Steal;
            int BlockC = character.Block;

            int PID = (int)Session["Pokemon"];

            HttpWebRequest WR = WebRequest.CreateHttp($"https://pokeapi.co/api/v2/pokemon/{PID}/");
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

                Random random = new Random();
                int which = random.Next(1, 3);

                if (which == 1)
                {
                    bool success = Methods.StealBlockConfirm(SD, StealC);

                    ViewBag.Which = 1;
                    ViewBag.Success = success;
                }
                else if (which == 2)
                {
                    bool success = Methods.StealBlockConfirm(SD, BlockC);

                    ViewBag.Which = 2;
                    ViewBag.Success = success;
                }

            }
            catch (Exception e)
            {
                ViewBag.Error = "JSON Issue";
                ViewBag.ErrorDescription = e.Message;
                return View();
            }


            if (shot == "ThreePoint")
            {
                bool truth = Methods.ShotConfirm(character.ThreePoint);
                string made =  truth ? "Shot went in!" : "Shot missed!";
                ViewBag.Made = made;
                if (truth)
                {
                    int x = (int)Session["User"];
                    x += 3;
                    Session["User"] = x;
                }
            }
            else if (shot == "MidRange")
            {
                bool truth = Methods.ShotConfirm(character.FieldGoal);
                string made = truth ? "Shot went in!" : "Shot missed!";
                ViewBag.Made = made;
                if (truth)
                {
                    int x = (int)Session["User"];
                    x += 2;
                    Session["User"] = x;
                }
            }
            else if (shot == "Paint")
            {
                bool truth = Methods.ShotConfirm(character.Paint);
                string made = truth ? "Shot went in!" : "Shot missed!";
                ViewBag.Made = made;
                if (truth)
                {
                    int x = (int)Session["User"];
                    x += 2;
                    Session["User"] = x;
                }
            }

            return View();
        }

        public ActionResult PlayConclusion4()
        {
            int ThreePoint = 0;
            int FieldGoal = 0;
            int Paint = 0;

            Random random = new Random();
            int iShot = random.Next(1, 4);
            string shot = "";

            if (iShot == 1)
            {
                shot = "ThreePoint";
            }
            else if (iShot == 2)
            {
                shot = "MidRange";
            }
            else if (iShot == 3)
            {
                shot = "Paint";
            }
            int ID = (int)Session["Char"];

            Character character = db.Characters.Where(
                c => c.CharID == ID).Single();

            int StealC = character.Steal;
            int BlockC = character.Block;

            int PID = (int)Session["Pokemon"];

            //TODO: Ask whats worse practice, making a bunch of sessions and storing the Pokemons Basketball Stats, or a second request to the API
            HttpWebRequest WR = WebRequest.CreateHttp($"https://pokeapi.co/api/v2/pokemon/{PID}/");
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

                ThreePoint = SA;
                FieldGoal = A;
                Paint = S;

                ViewBag.ThreePoint = SA;
                ViewBag.FieldGoal = A;
                ViewBag.Paint = S;
                ViewBag.Steal = SD;
                ViewBag.Block = D;

                int x = Methods.Generator();

                ViewBag.Number = x;

                //int StealTester = SD + 5;

                //bool success = Methods.StealBlockConfirm(SD, StealTester);

                int which = random.Next(1, 3);

                if (which == 1)
                {
                    bool success = Methods.StealBlockConfirm(SD, StealC);

                    ViewBag.Which = 1;
                    ViewBag.Success = success;
                }
                else if (which == 2)
                {
                    bool success = Methods.StealBlockConfirm(SD, BlockC);

                    ViewBag.Which = 2;
                    ViewBag.Success = success;
                }

            }
            catch (Exception e)
            {
                ViewBag.Error = "JSON Issue";
                ViewBag.ErrorDescription = e.Message;
                return View();
            }


            if (shot == "ThreePoint")
            {
                //TODO: For past MVP, insert Pokemon name, can store that name and just use as Variable for replacement
                bool truth = Methods.ShotConfirm(ThreePoint);
                string made = truth ? "Pokemon's shot went in!" : "Pokemon's shot missed!";
                ViewBag.Made = made;
                if (truth)
                {
                    int x = (int)Session["Comp"];
                    x += 3;
                    Session["Comp"] = x;
                }
            }
            else if (shot == "MidRange")
            {
                bool truth = Methods.ShotConfirm(FieldGoal);
                string made = truth ? "Pokemon's shot went in!" : "Pokemon's shot missed!";
                ViewBag.Made = made;
                if (truth)
                {
                    int x = (int)Session["Comp"];
                    x += 2;
                    Session["Comp"] = x;
                }
            }
            else if (shot == "Paint")
            {
                bool truth = Methods.ShotConfirm(Paint);
                string made = truth ? "Pokemon's shot went in!" : "Pokemon's shot missed!";
                ViewBag.Made = made;
                if (truth)
                {
                    int x = (int)Session["Comp"];
                    x += 3;
                    Session["Comp"] = x;
                }
            }

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
            int u = (int)Session["User"];
            int c = (int)Session["Comp"];

            if (u > c)
            {
                ViewBag.Winner = "Player Won!!!";
            }
            else if (c > u)
            {
                ViewBag.Winner = "Pokemon Won!!! D:";
            }
            else if (u == c)
            {
                ViewBag.Winner = "Game was a Tie! No one won.";
            }
            return View();
        }
    }
}