using DDS.Models;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DDS.Controllers {
    [Authorize]
    public class MetodologíasController : Controller {
        private int usuarioID;

        private void RefreshUserID() {
            string cookieName = FormsAuthentication.FormsCookieName;
            HttpCookie authCookie = HttpContext.Request.Cookies[cookieName];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
            usuarioID = int.Parse(ticket.Name);
        }

        public ActionResult Agregar() {
            RefreshUserID();
            ViewBag.NombresCuentas = Cuenta.GetNombres();
            ViewBag.NombresIndicadores = Indicador.GetNombres(usuarioID);
            ViewBag.NombresMetodologías = Metodología.GetNombres(usuarioID);
            return View();
        }

        public ActionResult Menú() {
            return View();
        }

        [HttpPost]
        public ActionResult Procesar(string nombre, string fórmula) {
            if (!Metodología.NameTaken(nombre)) {
                RefreshUserID();
                new Metodología(usuarioID, nombre, fórmula);
                TempData["Info"] = "Se creó una metodología exitosamente, de nombre \"" + nombre + '"';
            } else TempData["Error"] = "Ya existe una metodología de nombre \"" + nombre + '"';
            return RedirectToAction("Menú", "Metodologías");
        }

        private void AgregarTuplasPorPeríodo(Metodología m, Empresa e, int período) {
            if (período != 0) {
                bool cumple = m.CumpleCondiciones(e.DiccionarioCuentasDelPeríodo(período));
                if (m.parseosVálidos) {
                    string valor = cumple ? "Cumple" : "No Cumple";
                    filas.Add(new Tuple<string, int, string>(e.Nombre, período, valor));
                }
            }
        }

        private void AgregarTuplasHistóricas(Metodología m, Empresa e) {
            foreach (int p in Empresa.GetPeríodos())
                AgregarTuplasPorPeríodo(m, e, p);
        }

        private void AgregarTuplas(Metodología m, Empresa e, int p, bool esHistórico) {
            if (esHistórico) AgregarTuplasHistóricas(m, e);
            else AgregarTuplasPorPeríodo(m, e, p);
        }

        private List<Tuple<string, int, string>> filas;

        public ActionResult Aplicar(string nombreMetodología = null, string nombreEmpresa = null, int período = 0, bool esComparativo = false, bool esHistórico = false) {
            RefreshUserID();
            ViewBag.NombresMetodologías = Metodología.GetNombres(usuarioID);
            ViewBag.NombresEmpresas = Empresa.GetNombres();
            ViewBag.Períodos = Empresa.GetPeríodos();
            ViewBag.EsComparativo = esComparativo;
            ViewBag.EsHistórico = esHistórico;

            filas = new List<Tuple<string, int, string>>();
            if (nombreMetodología != null) {
                Metodología m = Metodología.Get(nombreMetodología);
                if (m != null) {
                    ViewBag.NombreMetodología = nombreMetodología;
                    ViewBag.NombreEmpresa = nombreEmpresa;
                    ViewBag.Período = período;

                    if (esComparativo) {
                        foreach (Empresa e in Empresa.GetEmpresas())
                            AgregarTuplas(m, e, período, esHistórico);
                    } else if (nombreEmpresa != null) {
                        Empresa e = Empresa.Get(nombreEmpresa);
                        if (e != null) AgregarTuplas(m, e, período, esHistórico);
                    }
                }
            }
            if (filas.Count > 0) ViewBag.Filas = filas;

            return View();
        }

        public ActionResult Modificar(string nombreMetodología = null, string fórmula = null) {
            RefreshUserID();
            ViewBag.NombresIndicadores = Indicador.GetNombres(usuarioID);
            ViewBag.NombresMetodologías = Metodología.GetNombres(usuarioID);
            ViewBag.NombreMetodología = ViewBag.Fórmula = "";
            if (nombreMetodología != null) {
                ViewBag.NombreMetodología = nombreMetodología;
                Metodología m = Metodología.Get(nombreMetodología);
                ViewBag.Fórmula = m.Fórmula();
                if (fórmula != null) {
                    m.BorrarConds();
                    m.NuevaFórmula(fórmula);
                    m.Save();
                    TempData["Info"] = "Se modificó exitosamente a la metodología de nombre \"" + nombreMetodología + '"';
                    return RedirectToAction("Menú", "Metodologías");
                }
            }
            return View();
        }

        public ActionResult Borrar(string nombreMetodología = null) {
            RefreshUserID();
            if (nombreMetodología != null) Metodología.Get(nombreMetodología).Borrar();
            ViewBag.NombresIndicadores = Metodología.GetNombres(usuarioID);
            return View();
        }
    }
}