using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PokeJam.Models;

namespace PokeJam.Controllers
{
    public class PokeTiersController : Controller
    {
        private PokeJamEntities db = new PokeJamEntities();

        // GET: PokeTiers
        public ActionResult Index()
        {
            return View(db.PokeTiers.ToList());
        }

        // GET: PokeTiers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PokeTier pokeTier = db.PokeTiers.Find(id);
            if (pokeTier == null)
            {
                return HttpNotFound();
            }
            return View(pokeTier);
        }

        // GET: PokeTiers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PokeTiers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PokeID,Tiers,PokeName")] PokeTier pokeTier)
        {
            if (ModelState.IsValid)
            {
                db.PokeTiers.Add(pokeTier);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(pokeTier);
        }

        // GET: PokeTiers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PokeTier pokeTier = db.PokeTiers.Find(id);
            if (pokeTier == null)
            {
                return HttpNotFound();
            }
            return View(pokeTier);
        }

        // POST: PokeTiers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PokeID,Tiers,PokeName")] PokeTier pokeTier)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pokeTier).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pokeTier);
        }

        // GET: PokeTiers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PokeTier pokeTier = db.PokeTiers.Find(id);
            if (pokeTier == null)
            {
                return HttpNotFound();
            }
            return View(pokeTier);
        }

        // POST: PokeTiers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PokeTier pokeTier = db.PokeTiers.Find(id);
            db.PokeTiers.Remove(pokeTier);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
