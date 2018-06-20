using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PokeJam.Controllers
{
    public class GamePlayController : Controller
    {
        // GET: GamePlay
        public ActionResult Index()
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

        public ActionResult GamePlay(string PlayType = "")
        {
            ViewBag.PlayType = PlayType;
            if (Session["PlayType"] == null)
            {
                Session.Add("PlayType", ViewBag.PlayType);
            }
            PlayType = (string)Session["PlayType"];
            Session["PlayType"] = PlayType;
            return View();
        }
       

        public ActionResult PlayConclusion(string shot)
        {
            ViewBag.Shot = shot;
            return View();
        }

        public ActionResult PlayConclusion2()
        {

            return View();
        }

        public ActionResult PlayConclusion3()
        {

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
    }
}