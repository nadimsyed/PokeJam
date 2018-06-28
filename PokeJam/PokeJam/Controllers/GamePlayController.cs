using System;
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
    [Authorize]
    public class GamePlayController : Controller
    {
        private PokeJamEntities db = new PokeJamEntities();

        public ActionResult HeadsTailsSingle(int? pokemon, string Coin)
        {
            Session["Quarter"] = 0;

            if (Session["TierCount"] == null && (string)Session["PlayType"] == "Tournament")
            {
                Session["TierCount"] = 1;
            }
            else if (Session["TierCount"] != null && (string)Session["PlayType"] == "Tournament")
            {
                int x = (int)Session["TierCount"];
                x++;
                Session["TierCount"] = x;
            }

            if (Session["TierTrack"] == null && (string)Session["PlayType"] == "Tournament")
            {
                Session["TierTrack"] = 0;
            }

            Random random = new Random();
            int flip = random.Next(1, 3);

            Session["Pokemon"] = pokemon;

            int PID;
            try
            {
                PID = (from p in db.PokeTiers
                       where p.PokeID == pokemon
                       select p.PokedexNumber).Single();
            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Home");
            }
            int compThreePoint = 0;
            int compFieldGoal = 0;
            int compPaint = 0;
            int compSteal = 0;
            int compBlock = 0;

            HttpWebRequest WR;
            try
            {
                WR = WebRequest.CreateHttp($"https://pokeapi.co/api/v2/pokemon/{PID}/");
                WR.UserAgent = ".NET Framework Test Client";
            }
            catch (Exception)
            {

                return RedirectToAction("APIError", "Home");
            }


            HttpWebResponse Response;

            try
            {
                Response = (HttpWebResponse)WR.GetResponse();
            }
            catch (WebException e)
            {
                ViewBag.Error = "Exception";
                ViewBag.ErrorDescription = e.Message;

                return RedirectToAction("APIError", "Home");

            }

            if (Response.StatusCode != HttpStatusCode.OK)
            {
                ViewBag.Error = Response.StatusCode;
                ViewBag.ErrorDescription = Response.StatusDescription;

                return RedirectToAction("APIError", "Home");

            }

            StreamReader reader = new StreamReader(Response.GetResponseStream());
            string PokemonData = reader.ReadToEnd();

            try
            {
                JObject JsonData = JObject.Parse(PokemonData);
                ViewBag.Name = JsonData["forms"][0];
                string name = ViewBag.Name.name;
                ViewBag.NameProp = Methods.UppercaseFirst(name);

                Session["PokeName"] = Methods.UppercaseFirst(name);

                string specialAtt = (string)JsonData["stats"][2]["base_stat"];
                string att = (string)JsonData["stats"][4]["base_stat"];
                string speed = (string)JsonData["stats"][0]["base_stat"];
                string specialDef = (string)JsonData["stats"][1]["base_stat"];
                string def = (string)JsonData["stats"][3]["base_stat"];

                compThreePoint = Methods.StatConverter(specialAtt);
                compFieldGoal = Methods.StatConverter(att) + 10;
                compPaint = Methods.StatConverter(speed) + 15;
                compSteal = Methods.StatConverter(specialDef) - 20;
                compBlock = Methods.StatConverter(def) - 15;

            }
            catch (Exception e)
            {
                ViewBag.Error = "JSON Issue";
                ViewBag.ErrorDescription = e.Message;

                return RedirectToAction("APIError", "Home");
            }

            Session["compThreePoint"] = compThreePoint;
            Session["compFieldGoal"] = compFieldGoal;
            Session["compPaint"] = compPaint;
            Session["compSteal"] = compSteal;
            Session["compBlock"] = compBlock;

            if (flip == 1)
            {
                if (Coin == "Heads")
                {
                    Session["Winner"] = "Player";
                    return RedirectToAction("QuarterSelector", "GamePlay");
                }
                else
                {
                    Session["Winner"] = "Computer";

                    return RedirectToAction("QuarterSelector", "GamePlay");
                }
            }
            else if (flip == 2)
            {
                if (Coin == "Tails")
                {
                    Session["Winner"] = "Player";

                    return RedirectToAction("QuarterSelector");
                }
                else
                {
                    Session["Winner"] = "Computer";

                    return RedirectToAction("QuarterSelector");
                }
            }

            return RedirectToAction("QuarterSelector");
        }

        public ActionResult QuarterSelector(string move)
        {
            int quarter;
            string winner;

            try
            {
                quarter = (int)Session["Quarter"];
                winner = (string)Session["Winner"];
            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Home");
            }

            if (quarter != 4)
            {
                quarter++;
                Session["Quarter"] = quarter;

                return View();
            }
            else if (quarter == 4 && winner == "Player")
            {
                return RedirectToAction("GameplayResult");
            }
            else if (quarter == 4 && winner == "Computer")
            {

                return RedirectToAction("GameplayResult2");
            }
            return RedirectToAction("GameplayResult");
        }

        public ActionResult NumberCrunch(int? ThreePoint, int? MidRange, int? Paint, int? Steal, int? Block, string overtime = "no")
        {
            List<string> playerPlays = new List<string>();
            List<string> computerPlays = new List<string>();

            string shot = "";
            bool success = true;
            Random random = new Random();


             int charNum;
            try
            {
                charNum = (int)Session["Char"];
            }
            catch (Exception)
            {

               return  RedirectToAction("Index", "Home");
            }
            Character player = (from c in db.Characters
                                where c.CharID == charNum
                                select c).Single();

            int playerThreePoint;
            int playerFieldGoal;
            int playerPaint;
            int playerSteal;
            int playerBlock;
            int compThreePoint;
            int compFieldGoal;
            int compPaint;
            int compSteal;
            int compBlock;

            try
            {
                playerThreePoint = player.ThreePoint;
                playerFieldGoal = player.FieldGoal;
                playerPaint = player.Paint;
                playerSteal = player.Steal;
                playerBlock = player.Block;

                int compNum = (int)Session["Pokemon"];
                int PID = (from p in db.PokeTiers
                           where p.PokeID == compNum
                           select p.PokedexNumber).Single();
                compThreePoint = (int)Session["compThreePoint"];
                compFieldGoal = (int)Session["compFieldGoal"];
                compPaint = (int)Session["compPaint"];
                compSteal = (int)Session["compSteal"];
                compBlock = (int)Session["compBlock"];


            }
            catch (Exception)
            {

                return RedirectToAction("Index");
            }
            for (int i = 0; i < 5; i++)
            {
                int shotter = Methods.Generator();

                int which = random.Next(0, 101);

                if (which <= Steal)
                {
                    success = Methods.StealBlockConfirm(playerSteal, compSteal);

                    if (success)
                    {
                        string happened = "Ball was stolen!";
                        playerPlays.Add(happened);
                    }
                }
                else if (which > Steal && which <= 100)
                {
                    success = Methods.StealBlockConfirm(playerBlock, compBlock);

                    if (success)
                    {
                        string happened = "Ball was blocked!";
                        playerPlays.Add(happened);
                    }
                }

                if (!success)
                {
                    if (shotter <= ThreePoint)
                    {
                        shot = "ThreePoint";
                    }
                    else if (shotter <= (ThreePoint + MidRange) && shotter > ThreePoint)
                    {
                        shot = "MidRange";
                    }
                    else if (shotter <= (ThreePoint + MidRange + Paint) && shotter > (ThreePoint + MidRange))
                    {
                        shot = "Paint";
                    }

                    if (shot == "ThreePoint")
                    {
                        bool truth = Methods.ShotConfirm(playerThreePoint);
                        string made = truth ? "Player's Three-Point shot went in!" : "Player's Three-Point shot missed!";

                        playerPlays.Add(made);

                        ViewBag.Made = made;
                        if (truth && !success)
                        {
                            int z = (int)Session["User"];
                            z += 3;
                            Session["User"] = z;
                        }
                    }
                    else if (shot == "MidRange")
                    {
                        bool truth = Methods.ShotConfirm(playerFieldGoal);
                        string made = truth ? "Player's Mid-Range shot went in!" : "Player's Mid-Range shot missed!";

                        playerPlays.Add(made);

                        ViewBag.Made = made;
                        if (truth && !success)
                        {
                            int z = (int)Session["User"];
                            z += 2;
                            Session["User"] = z;
                        }
                    }
                    else if (shot == "Paint")
                    {
                        bool truth = Methods.ShotConfirm(playerPaint);
                        string made = truth ? "Player's In-the-Paint shot went in!" : "Player's In-the-Paint shot missed!";

                        playerPlays.Add(made);

                        ViewBag.Made = made;
                        if (truth && !success)
                        {
                            int z = (int)Session["User"];
                            z += 2;
                            Session["User"] = z;
                        }
                    }
                }
            }

            for (int i = 0; i < 5; i++)
            {
                int which2 = random.Next(1, 3);

                if (which2 == 1)
                {
                    success = Methods.StealBlockConfirm(compSteal, playerSteal);

                    if (success)
                    {
                        string happened = "Ball was stolen!";
                        computerPlays.Add(happened);
                    }
                }
                else if (which2 == 2)
                {
                    success = Methods.StealBlockConfirm(compBlock, playerBlock);

                    if (success)
                    {
                        string happened = "Ball was blocked!";
                        computerPlays.Add(happened);
                    }
                }
                int iShot = random.Next(1, 4);

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

                if (!success)
                {
                    if (shot == "ThreePoint")
                    {
                        bool truth = Methods.ShotConfirm(compThreePoint);
                        string made = truth ? "Pokemon's Three-Point shot went in!" : "Pokemon's Three-Point shot missed!";

                        computerPlays.Add(made);

                        ViewBag.Made = made;
                        if (truth && !success)
                        {
                            int z = (int)Session["Comp"];
                            z += 3;
                            Session["Comp"] = z;
                        }
                    }
                    else if (shot == "MidRange")
                    {
                        bool truth = Methods.ShotConfirm(compFieldGoal);
                        string made = truth ? "Pokemon's Mid-Range shot went in!" : "Pokemon's Mid-Range shot missed!";

                        computerPlays.Add(made);

                        ViewBag.Made = made;
                        if (truth && !success)
                        {
                            int z = (int)Session["Comp"];
                            z += 2;
                            Session["Comp"] = z;
                        }
                    }
                    else if (shot == "Paint")
                    {
                        bool truth = Methods.ShotConfirm(compPaint);
                        string made = truth ? "Pokemon's In-the-Paint shot went in!" : "Pokemon's In-the-Paint shot missed!";

                        computerPlays.Add(made);

                        ViewBag.Made = made;
                        if (truth && !success)
                        {
                            int z = (int)Session["Comp"];
                            z += 2;
                            Session["Comp"] = z;
                        }
                    }
                }
            }

            ViewBag.Player = playerPlays;
            ViewBag.Computer = computerPlays;
            ViewBag.Over = overtime;

            return View();
        }

        public ActionResult SinglePlayer(string play)
        {
            if (Session["PlayType"] == null)
            {
                Session.Add("PlayType", play);
            }
            else if (Session["PlayType"] != null)
            {
                Session["PlayType"] = play;
            }

            List<PokeTier> all = db.PokeTiers.ToList();

            ViewBag.All = all;

            return View();
        }

        public ActionResult Tier1(string play)
        {
            if (Session["PlayType"] == null)
            {
                Session.Add("PlayType", play);
            }
            else if (Session["PlayType"] != null)
            {
                Session["PlayType"] = play;
            }

            bool[] Track = new bool[5];
            string[] PokeTrack = new string[5];

            Session["Track"] = Track;
            Session["PokeTrack"] = PokeTrack;
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
        
        public ActionResult Tier5Congratulations()
        {
           int TierCount;
            try
            {
                TierCount = (int)Session["TierCount"];
            }
            catch (Exception)
            {

                return RedirectToAction("Index","Home");
            }

            bool[] track = (bool[])Session["Track"];
            string[] pokeTrack = (string[])Session["PokeTrack"];
            string UserId = (string)Session["UserID"];
            try
            {
                Methods.AddTournament(TierCount, track, pokeTrack, UserId);
            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Home");
            }

            track[0] = false;
            track[1] = false;
            track[2] = false;
            track[3] = false;
            track[4] = false;
            pokeTrack[0] = "N/A";
            pokeTrack[1] = "N/A";
            pokeTrack[2] = "N/A";
            pokeTrack[3] = "N/A";
            pokeTrack[4] = "N/A";

            Session["Track"] = track;
            Session["PokeTrack"] = pokeTrack;
            Session["TierCount"] = 0;

            return View();
        }

        public ActionResult TournamentLoss()
        {

            int TierCount;
            try
            {
                TierCount = (int)Session["TierCount"];
            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Home");
            }
            if (TierCount == 5)
            {
                bool[] track = (bool[])Session["Track"];
                string[] pokeTrack = (string[])Session["PokeTrack"];
                string UserId = (string)Session["UserID"];
                track[4] = false;
                pokeTrack[4] = "N/A";
                Methods.AddTournament(TierCount, track, pokeTrack, UserId);
            }
            if (TierCount == 4)
            {
                bool[] track = (bool[])Session["Track"];
                string[] pokeTrack = (string[])Session["PokeTrack"];
                string UserId = (string)Session["UserID"];
                track[3] = false;
                pokeTrack[3] = "N/A";
                track[4] = false;
                pokeTrack[4] = "N/A";
                Methods.AddTournament(TierCount, track, pokeTrack, UserId);


            }
            if (TierCount == 3)
            {
                bool[] track = (bool[])Session["Track"];
                string[] pokeTrack = (string[])Session["PokeTrack"];
                string UserId = (string)Session["UserID"];
                track[2] = false;
                pokeTrack[2] = "N/A";
                track[3] = false;
                pokeTrack[3] = "N/A";
                track[4] = false;
                pokeTrack[4] = "N/A";
                Methods.AddTournament(TierCount, track, pokeTrack, UserId);

            }
            if (TierCount == 2)
            {
                bool[] track = (bool[])Session["Track"];
                string[] pokeTrack = (string[])Session["PokeTrack"];
                string UserId = (string)Session["UserID"];
                track[1] = false;
                pokeTrack[1] = "N/A";
                track[2] = false;
                pokeTrack[2] = "N/A";
                track[3] = false;
                pokeTrack[3] = "N/A";
                track[4] = false;
                pokeTrack[4] = "N/A";
                Methods.AddTournament(TierCount, track, pokeTrack, UserId);

            }
            if (TierCount == 1)
            {
                bool[] track = (bool[])Session["Track"];
                string[] pokeTrack = (string[])Session["PokeTrack"];
                string UserId = (string)Session["UserID"];
                track[0] = false;
                pokeTrack[0] = "N/A";
                track[1] = false;
                pokeTrack[1] = "N/A";
                track[2] = false;
                pokeTrack[2] = "N/A";
                track[3] = false;
                pokeTrack[3] = "N/A";
                track[4] = false;
                pokeTrack[4] = "N/A";
            }

            return View();
        }

        public ActionResult TournamentNext()
        {
            Session["User"] = 0;
            Session["Comp"] = 0;

            int TierCount = (int)Session["TierCount"];
            if (TierCount == 1)
            {
                bool[] track = (bool[])Session["Track"];
                track[0] = true;
                Session["Track"] = track;
                string[] pokeTrack = (string[])Session["PokeTrack"];
                pokeTrack[0] = (string)Session["PokeName"];

                Session["PokeTrack"] = pokeTrack;

                ViewBag.Redirect = "/GamePlay/Tier2/";

                int store = (int)Session["TierTrack"];
                store++;
                Session["TierTrack"] = store;

            }
            else if (TierCount == 2)
            {
                bool[] track = (bool[])Session["Track"];
                track[1] = true;
                Session["Track"] = track;
                string[] pokeTrack = (string[])Session["PokeTrack"];
                pokeTrack[1] = (string)Session["PokeName"];

                Session["PokeTrack"] = pokeTrack;

                ViewBag.Redirect = "/GamePlay/Tier3/";

                int store = (int)Session["TierTrack"];
                store++;
                Session["TierTrack"] = store;
            }
            else if (TierCount == 3)
            {
                bool[] track = (bool[])Session["Track"];
                track[2] = true;
                Session["Track"] = track;
                string[] pokeTrack = (string[])Session["PokeTrack"];
                pokeTrack[2] = (string)Session["PokeName"];

                Session["PokeTrack"] = pokeTrack;

                ViewBag.Redirect = "/GamePlay/Tier4/";

                int store = (int)Session["TierTrack"];
                store++;
                Session["TierTrack"] = store;
            }
            else if (TierCount == 4)
            {
                bool[] track = (bool[])Session["Track"];
                track[3] = true;
                Session["Track"] = track;
                string[] pokeTrack = (string[])Session["PokeTrack"];
                pokeTrack[3] = (string)Session["PokeName"];

                Session["PokeTrack"] = pokeTrack;

                ViewBag.Redirect = "/GamePlay/Tier5/";

                int store = (int)Session["TierTrack"];
                store++;
                Session["TierTrack"] = store;
            }

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

            int TierCount = 0;
            if ((string)Session["PlayType"] == "Tournament")
            {
                TierCount = (int)Session["TierCount"];
            }

            if (u > c)
            {
                ViewBag.Winner = "Player Won!!!";
                if ((string)Session["PlayType"] == "Tournament")
                {
                    if (TierCount == 5)
                    {
                        bool[] track = (bool[])Session["Track"];
                        track[4] = true;
                        Session["Track"] = track;
                        string[] pokeTrack = (string[])Session["PokeTrack"];
                        pokeTrack[4] = (string)Session["PokeName"];
                        Session["PokeTrack"] = pokeTrack;

                        return RedirectToAction("Tier5Congratulations");
                    }
                    ViewBag.Truth = "True";
                }
            }
            else if (c > u)
            {
                ViewBag.Winner = "Pokemon Won!!! D:";
                if ((string)Session["PlayType"] == "Tournament")
                {
                    ViewBag.Truth = "False";
                }
            }
            else if (u == c)
            {
                ViewBag.Winner = "Game was a Tie!.";

                if ((string)Session["PlayType"] == "Tournament" || (string)Session["PlayType"] == "Single")

                {
                    ViewBag.Truth = "Tie";
                }
            }
            return View();
        }

        public ActionResult GameplayResult2()
        {
            int u = (int)Session["User"];
            int c = (int)Session["Comp"];

            int TierCount = 0;
            if ((string)Session["PlayType"] == "Tournament")
            {
                TierCount = (int)Session["TierCount"];
            }

            if (u > c)
            {
                ViewBag.Winner = "Player Won!!!";
                if ((string)Session["PlayType"] == "Tournament")
                {
                    if (TierCount == 5)
                    {
                        bool[] track = (bool[])Session["Track"];
                        track[4] = true;
                        Session["Track"] = track;
                        string[] pokeTrack = (string[])Session["PokeTrack"];
                        pokeTrack[4] = (string)Session["PokeName"];
                        Session["PokeTrack"] = pokeTrack;

                        return RedirectToAction("Tier5Congratulations");

                    }
                    ViewBag.Truth = "True";
                }
            }
            else if (c > u)
            {
                ViewBag.Winner = "Pokemon Won!!! D:";
                if ((string)Session["PlayType"] == "Tournament")
                {
                    ViewBag.Truth = "False";
                }
            }
            else if (u == c)
            {
                ViewBag.Winner = "Game was a Tie!.";

                if ((string)Session["PlayType"] == "Tournament" || (string)Session["PlayType"] == "Single")

                {
                    ViewBag.Truth = "Tie";
                }
            }
            return View();
        }

        public ActionResult OvertimeStart()
        {
            return View();
        }

        public ActionResult OverTime()
        {
            return View();
        }
    }
}