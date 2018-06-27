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
    public class GamePlayController : Controller
    {
        private PokeJamEntities db = new PokeJamEntities();
        //int TierCount = 1;

        // GET: GamePlay
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult HeadsTailsSingle(int pokemon, string Coin)
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

            //List<string> pokemons = new List<string>();
            //string name = (from p in db.PokeTiers
            //               where p.PokeID == pokemon
            //               select p.PokeName).Single();
            //pokemons.Add(name);

            //if (Session["TierPoke"] == null)
            //{
            //    Session["TierPoke"] = pokemons;
            //}
            //else if (Session["TierPoke"] != null)
            //{
            //    List<string> pokes = (List<string>)Session["TierPoke"];
            //    pokes.Add(pokemon);
            //    Session["TierPoke"] = pokes;
            //}


            Random random = new Random();
            int flip = random.Next(1, 3);

            Session["Pokemon"] = pokemon;


            int PID = (from p in db.PokeTiers
                       where p.PokeID == pokemon
                       select p.PokedexNumber).Single();
            int compThreePoint = 0;
            int compFieldGoal = 0;
            int compPaint = 0;
            int compSteal = 0;
            int compBlock = 0;
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
                return View();
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

        public ActionResult QuarterSelector()
        {
            int quarter = (int)Session["Quarter"];
            string winner = (string)Session["Winner"];
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

        //TODO: send a bool here from the overttime. use that to calculate and if overtime is true, pass back to a action/view for overttime
        public ActionResult NumberCrunch(int ThreePoint, int MidRange, int Paint, int Steal, int Block, bool overtime = false)
        {
            List<string> playerPlays = new List<string>();
            List<string> computerPlays = new List<string>();

            string shot = "";
            bool success = true;
            Random random = new Random();


            int charNum = (int)Session["Char"];
            Character player = (from c in db.Characters
                                where c.CharID == charNum
                                select c).Single();
            int playerThreePoint = player.ThreePoint;
            int playerFieldGoal = player.FieldGoal;
            int playerPaint = player.Paint;
            int playerSteal = player.Steal;
            int playerBlock = player.Block;

            int compNum = (int)Session["Pokemon"];
            int PID = (from p in db.PokeTiers
                       where p.PokeID == compNum
                       select p.PokedexNumber).Single();
            int compThreePoint = (int)Session["compThreePoint"];
            int compFieldGoal = (int)Session["compFieldGoal"];
            int compPaint = (int)Session["compPaint"];
            int compSteal = (int)Session["compSteal"];
            int compBlock = (int)Session["compBlock"];


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

                    ViewBag.Which = 1;
                    ViewBag.Success = success;
                }
                else if (which > Steal && which <= 100)
                {
                    success = Methods.StealBlockConfirm(playerBlock, compBlock);

                    if (success)
                    {
                        string happened = "Ball was blocked!";
                        playerPlays.Add(happened);
                    }

                    ViewBag.Which = 2;
                    ViewBag.Success = success;
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

                    ViewBag.Which = 1;
                    ViewBag.Success = success;
                }
                else if (which2 == 2)
                {
                    success = Methods.StealBlockConfirm(compBlock, playerBlock);

                    if (success)
                    {
                        string happened = "Ball was blocked!";
                        computerPlays.Add(happened);
                    }

                    ViewBag.Which = 2;
                    ViewBag.Success = success;
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

        public ActionResult GamePlay(/*int pokemon, */string Coin)
        {
            ViewBag.Coin = Coin;
            ViewBag.Choice = Session["Pokemon"];

            return View();
        }

        public ActionResult GamePlay2(/*int pokemon, */string Coin)
        {
            //ViewBag.Coin = Coin;
            //ViewBag.Choice = Session["Pokemon"];
            bool success = true;

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
                    success = Methods.StealBlockConfirm(SD, StealC);

                    ViewBag.Which = 1;
                    ViewBag.Success = success;
                }
                else if (which == 2)
                {
                    success = Methods.StealBlockConfirm(SD, BlockC);

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
                bool truth = Methods.ShotConfirm(ThreePoint);
                string made = truth ? "Pokemon's shot went in!" : "Pokemon's shot missed!";
                ViewBag.Made = made;
                if (truth && !success)
                {
                    Session["Comp"] = 3;
                }
            }
            else if (shot == "MidRange")
            {
                bool truth = Methods.ShotConfirm(FieldGoal);
                string made = truth ? "Pokemon's shot went in!" : "Pokemon's shot missed!";
                ViewBag.Made = made;
                if (truth && !success)
                {
                    Session["Comp"] = 2;
                }
            }
            else if (shot == "Paint")
            {
                bool truth = Methods.ShotConfirm(Paint);
                string made = truth ? "Pokemon's shot went in!" : "Pokemon's shot missed!";
                ViewBag.Made = made;
                if (truth && !success)
                {
                    Session["Comp"] = 2;
                }
            }

            return View();
        }

        public ActionResult PlayConclusion(string shot)
        {
            bool success = true;

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
                int A = (Methods.StatConverter(att)) / 3 + 10;
                int S = (Methods.StatConverter(speed)) / 3 + 15;
                int SD = (Methods.StatConverter(specialDef)) / 3 - 20;
                int D = (Methods.StatConverter(def)) / 3 - 15;


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
                    success = Methods.StealBlockConfirm(StealC, SD);

                    ViewBag.Which = 1;
                    ViewBag.Success = success;
                }
                else if (which == 2)
                {
                    success = Methods.StealBlockConfirm(BlockC, SD);

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
                string made = truth ? "Shot went in!" : "Shot missed!";
                ViewBag.Made = made;
                if (truth && !success)
                {
                    Session["User"] = 3;
                }
            }
            else if (shot == "MidRange")
            {
                bool truth = Methods.ShotConfirm(character.FieldGoal);
                string made = truth ? "Shot went in!" : "Shot missed!";
                ViewBag.Made = made;
                if (truth && !success)
                {
                    Session["User"] = 2;
                }
            }
            else if (shot == "Paint")
            {
                bool truth = Methods.ShotConfirm(character.Paint);
                string made = truth ? "Shot went in!" : "Shot missed!";
                ViewBag.Made = made;
                if (truth && !success)
                {
                    Session["User"] = 2;
                }
            }

            return View();
        }

        public ActionResult PlayConclusionX(string shot)
        {
            bool success = true;

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
                int A = (Methods.StatConverter(att)) / 3 + 10;
                int S = (Methods.StatConverter(speed)) / 3 + 15;
                int SD = (Methods.StatConverter(specialDef)) / 3 - 20;
                int D = (Methods.StatConverter(def)) / 3 - 15;


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
                    success = Methods.StealBlockConfirm(StealC, SD);

                    ViewBag.Which = 1;
                    ViewBag.Success = success;
                }
                else if (which == 2)
                {
                    success = Methods.StealBlockConfirm(BlockC, SD);

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
                string made = truth ? "Shot went in!" : "Shot missed!";
                ViewBag.Made = made;
                if (truth && !success)
                {
                    Session["User"] = 3;
                }
            }
            else if (shot == "MidRange")
            {
                bool truth = Methods.ShotConfirm(character.FieldGoal);
                string made = truth ? "Shot went in!" : "Shot missed!";
                ViewBag.Made = made;
                if (truth && !success)
                {
                    Session["User"] = 2;
                }
            }
            else if (shot == "Paint")
            {
                bool truth = Methods.ShotConfirm(character.Paint);
                string made = truth ? "Shot went in!" : "Shot missed!";
                ViewBag.Made = made;
                if (truth && !success)
                {
                    Session["User"] = 2;
                }
            }

            return View();
        }

        public ActionResult PlayConclusion2()
        {
            bool success = true;

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
                    success = Methods.StealBlockConfirm(SD, StealC);

                    ViewBag.Which = 1;
                    ViewBag.Success = success;
                }
                else if (which == 2)
                {
                    success = Methods.StealBlockConfirm(SD, BlockC);

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
                bool truth = Methods.ShotConfirm(ThreePoint);
                string made = truth ? "Pokemon's shot went in!" : "Pokemon's shot missed!";
                ViewBag.Made = made;
                if (truth && !success)
                {
                    Session["Comp"] = 3;
                }
            }
            else if (shot == "MidRange")
            {
                bool truth = Methods.ShotConfirm(FieldGoal);
                string made = truth ? "Pokemon's shot went in!" : "Pokemon's shot missed!";
                ViewBag.Made = made;
                if (truth && !success)
                {
                    Session["Comp"] = 2;
                }
            }
            else if (shot == "Paint")
            {
                bool truth = Methods.ShotConfirm(Paint);
                string made = truth ? "Pokemon's shot went in!" : "Pokemon's shot missed!";
                ViewBag.Made = made;
                if (truth && !success)
                {
                    Session["Comp"] = 2;
                }
            }

            return View();
        }

        public ActionResult PlayConclusionX2()
        {
            bool success = true;

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
                    success = Methods.StealBlockConfirm(SD, StealC);

                    ViewBag.Which = 1;
                    ViewBag.Success = success;
                }
                else if (which == 2)
                {
                    success = Methods.StealBlockConfirm(SD, BlockC);

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
                bool truth = Methods.ShotConfirm(ThreePoint);
                string made = truth ? "Pokemon's shot went in!" : "Pokemon's shot missed!";
                ViewBag.Made = made;
                if (truth && !success)
                {
                    Session["Comp"] = 3;
                }
            }
            else if (shot == "MidRange")
            {
                bool truth = Methods.ShotConfirm(FieldGoal);
                string made = truth ? "Pokemon's shot went in!" : "Pokemon's shot missed!";
                ViewBag.Made = made;
                if (truth && !success)
                {
                    Session["Comp"] = 2;
                }
            }
            else if (shot == "Paint")
            {
                bool truth = Methods.ShotConfirm(Paint);
                string made = truth ? "Pokemon's shot went in!" : "Pokemon's shot missed!";
                ViewBag.Made = made;
                if (truth && !success)
                {
                    Session["Comp"] = 2;
                }
            }

            return View();
        }

        public ActionResult PlayConclusion3(string shot)
        {
            bool success = true;

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
                    success = Methods.StealBlockConfirm(SD, StealC);

                    ViewBag.Which = 1;
                    ViewBag.Success = success;
                }
                else if (which == 2)
                {
                    success = Methods.StealBlockConfirm(SD, BlockC);

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
                string made = truth ? "Shot went in!" : "Shot missed!";
                ViewBag.Made = made;
                if (truth && !success)
                {
                    string z = (string)Session["User"].ToString();
                    int x = int.Parse(z);
                    x += 3;
                    Session["User"] = x;
                }
            }
            else if (shot == "MidRange")
            {
                bool truth = Methods.ShotConfirm(character.FieldGoal);
                string made = truth ? "Shot went in!" : "Shot missed!";
                ViewBag.Made = made;
                if (truth && !success)
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
                if (truth && !success)
                {
                    int x = (int)Session["User"];
                    x += 2;
                    Session["User"] = x;
                }
            }

            return View();
        }

        public ActionResult PlayConclusionX3(string shot)
        {
            bool success = true;

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
                    success = Methods.StealBlockConfirm(SD, StealC);

                    ViewBag.Which = 1;
                    ViewBag.Success = success;
                }
                else if (which == 2)
                {
                    success = Methods.StealBlockConfirm(SD, BlockC);

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
                string made = truth ? "Shot went in!" : "Shot missed!";
                ViewBag.Made = made;
                if (truth && !success)
                {
                    string z = (string)Session["User"].ToString();
                    int x = int.Parse(z);
                    x += 3;
                    Session["User"] = x;
                }
            }
            else if (shot == "MidRange")
            {
                bool truth = Methods.ShotConfirm(character.FieldGoal);
                string made = truth ? "Shot went in!" : "Shot missed!";
                ViewBag.Made = made;
                if (truth && !success)
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
                if (truth && !success)
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
            bool success = true;

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
                    success = Methods.StealBlockConfirm(SD, StealC);

                    ViewBag.Which = 1;
                    ViewBag.Success = success;
                }
                else if (which == 2)
                {
                    success = Methods.StealBlockConfirm(SD, BlockC);

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
                bool truth = Methods.ShotConfirm(ThreePoint);
                string made = truth ? "Pokemon's shot went in!" : "Pokemon's shot missed!";
                ViewBag.Made = made;
                if (truth && !success)
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
                if (truth && !success)
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
                if (truth && !success)
                {
                    int x = (int)Session["Comp"];
                    x += 3;
                    Session["Comp"] = x;
                }
            }

            return View();
        }

        public ActionResult PlayConclusionX4()
        {
            bool success = true;

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
                    success = Methods.StealBlockConfirm(SD, StealC);

                    ViewBag.Which = 1;
                    ViewBag.Success = success;
                }
                else if (which == 2)
                {
                    success = Methods.StealBlockConfirm(SD, BlockC);

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
                bool truth = Methods.ShotConfirm(ThreePoint);
                string made = truth ? "Pokemon's shot went in!" : "Pokemon's shot missed!";
                ViewBag.Made = made;
                if (truth && !success)
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
                if (truth && !success)
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
                if (truth && !success)
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

            //int store = (int)Session["TierTrack"];
            //store++;
            //Session["TierTrack"] = store;

            int TierCount = (int)Session["TierCount"];

            bool[] track = (bool[])Session["Track"];
            string[] pokeTrack = (string[])Session["PokeTrack"];
            //Tournament tournament = new Tournament();
            string UserId = (string)Session["UserID"];
            Methods.AddTournament(TierCount, track, pokeTrack, UserId);

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
            //tournament.Id = (string)Session["UserID"];
            //tournament.T1 = track[0];
            //tournament.T2 = track[1];
            //tournament.T3 = track[2];
            //tournament.T4 = track[3];
            //tournament.T5 = track[4];
            //tournament.P1 = pokeTrack[0];
            //tournament.P2 = pokeTrack[1];
            //tournament.P3 = pokeTrack[2];
            //tournament.P4 = pokeTrack[3];
            //tournament.P5 = pokeTrack[4];

            //db.Tournaments.Add(tournament);
            //db.SaveChangesAsync();

            return View();
        }

        public ActionResult TournamentLoss()
        {
            int TierCount = (int)Session["TierCount"];
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
                if ((string)Session["PlayType"] == "Tournament")
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
                if ((string)Session["PlayType"] == "Tournament")
                {
                    ViewBag.Truth = "Tie";
                }
            }
            return View();
        }

        //TODO: Create Overtime here. Redirect to HeadsTails and play the game for one more round. or just play another round and display the overtime result. And keeps going to overtime till game is not a tie

    }
}