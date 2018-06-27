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
    [Authorize]
    public class HomeController : Controller
    {
        private PokeJamEntities db = new PokeJamEntities();

        
        public ActionResult Index()
        {
            Session["UserID"] = User.Identity.GetUserId();

            Session["TierCount"] = 0;
            Session["User"] = 0;
            Session["Comp"] = 0;

            string user = (string)Session["UserID"];

            List<Character> characters = (from c in db.Characters
                                          where c.Id == user
                                          select c).ToList(); 
            ViewBag.Characters = characters;

            if (characters.Count == 0)
            {
                return View("CreatePlayerStart");
            }

            return View();
        }


        [Authorize]
        public ActionResult CreatePlayerStart()
        {

            return View();
        }

        
 public ActionResult YourTournamentHistory()
        {
            string user = (string)Session["UserID"];

            List<Tournament> tournaments = (from t in db.Tournaments
                                          where t.Id == user
                                          select t).ToList();
            ViewBag.Tournaments = tournaments;

            return View();
        }

        public ActionResult AllTournamentHistory()
        {
            List<Tournament> tournaments = (from t in db.Tournaments
                                            select t).ToList();
            ViewBag.Tournaments = tournaments;


                return RedirectToAction("CreatePlayerStart");
            }
        
        public ActionResult SuccessfullyCreatedChar(string CharName, int? CharHeight,  int? CharWeight, int? ThreePoint, int? FieldGoat, int? Paint, int? Steal, int? Block)
        {
           
                if (CharName == null)
                {
                    return RedirectToAction("CreatePlayerStart");
                }
                else if(CharName != null)
            {
                Character character = new Character();
                character.Id = (string)Session["UserID"];
                character.CharName = CharName;
                character.Height = (int)CharHeight;
                character.Weight = (int)CharWeight;
                character.ThreePoint = (int)ThreePoint;
                character.FieldGoal = (int)FieldGoat;
                character.Paint = (int)Paint;
                character.Steal = (int)Steal;
                character.Block = (int)Block;

                db.Characters.Add(character);
                db.SaveChanges();
                List<Character> characters = new List<Character>();
                characters.Add(character);
                ViewBag.Char = characters;

                Session["Char"] = character.CharID;
            }

                return View();      
        }

        public ActionResult PokeJam(int CharID = 0)
        {
            if (CharID != 0)
            {
                Session["Char"] = CharID;
            }
            
            int TierCount;
            try
            {
                 TierCount = (int)Session["TierCount"];
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
            }
            catch (Exception)
            {

                return RedirectToAction("Index");
            }
            

            TierCount = (int)Session["TierCount"];
            if (TierCount == 5)
            {
                bool[] track = (bool[])Session["Track"];
                string[] pokeTrack = (string[])Session["PokeTrack"];
                string UserId = (string)Session["UserID"];
                track[4] = false;
                pokeTrack[4] = "N/A";
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

            }

            Session["TierCount"] = 0;
            Session["User"] = 0;
            Session["Comp"] = 0;

            return View();
        }

        public ActionResult Rules()
        {
            return View();
        }

        public ActionResult ViewStats()
        {
            int charID = (int)Session["Char"];
            List<Character> characters = (from c in db.Characters
                                          where c.CharID == charID
                                          select c).ToList();

            ViewBag.Characters = characters;
            return View();
        }
        public ActionResult DatabaseError()
        {
            return View();
        }
        public ActionResult APIError()
        {
            return View();
        }
    }
}