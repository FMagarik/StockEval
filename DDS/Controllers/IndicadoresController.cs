using DDS.Models;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DDS.Controllers {
    [Authorize]
    public class IndicadoresController : Controller {
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
            return View();
        }

        public ActionResult Menú() {
            return View();
        }

        [HttpPost]
        public ActionResult Procesar(string nombre, string fórmula) {
            if (!Indicador.NameTaken(nombre)) {
                RefreshUserID();
                new Indicador(usuarioID, nombre, fórmula);
                TempData["Info"] = "Se creó un indicador exitosamente, de nombre \"" + nombre + '"';
            } else TempData["Error"] = "Ya existe un indicador de nombre \"" + nombre + '"';
            return RedirectToAction("Menú", "Indicadores");
        }

        private void AgregarTuplasPorPeríodo(Indicador i, Empresa e, int período) {
            if (período != 0) {
                var cuentas = e.DiccionarioCuentasDelPeríodo(período);
                string valor = i.CalcularValor(e, período).ToString();
                if (i.parser.EsVálido()) filas.Add(new Tuple<string, int, string>(e.Nombre, período, valor));
            }
        }

        private void AgregarTuplasHistóricas(Indicador i, Empresa e) {
            foreach (int p in e.GetPeríodosDeEmpresa())
                AgregarTuplasPorPeríodo(i, e, p);
        }

        private void AgregarTuplas(Indicador i, Empresa e, int p, bool esHistórico) {
            if (esHistórico) AgregarTuplasHistóricas(i, e);
            else AgregarTuplasPorPeríodo(i, e, p);
        }

        private List<Tuple<string, int, string>> filas;

        public ActionResult Aplicar(string nombreIndicador = null, string nombreEmpresa = null, int período = 0, bool esComparativo = false, bool esHistórico = false) {
            RefreshUserID();
            ViewBag.NombreIndicador = nombreIndicador;
            ViewBag.NombresIndicadores = Indicador.GetNombres(usuarioID);
            ViewBag.NombreEmpresa = nombreEmpresa;
            ViewBag.NombresEmpresas = Empresa.GetNombres();
            ViewBag.Período = período;
            ViewBag.Períodos = Empresa.GetPeríodos();

            filas = new List<Tuple<string, int, string>>();
            if (nombreIndicador != null) {
                Indicador i = Indicador.Get(nombreIndicador);
                if (i != null) {
                    if (esComparativo) {
                        foreach (Empresa e in Empresa.GetEmpresas())
                            AgregarTuplas(i, e, período, esHistórico);
                    } else if (nombreEmpresa != null) {
                        Empresa e = Empresa.Get(nombreEmpresa);
                        if (e != null && período != 0) AgregarTuplas(i, e, período, esHistórico);
                    }
                }
            }
            if (filas.Count > 0) ViewBag.Filas = filas;

            return View();
        }

        public ActionResult Modificar(string nombreIndicador = null, string fórmula = null) {
            RefreshUserID();
            ViewBag.NombresIndicadores = Indicador.GetNombres(usuarioID);
            ViewBag.NombreIndicador = ViewBag.Fórmula = "";
            if (nombreIndicador != null) {
                ViewBag.NombreIndicador = nombreIndicador;
                Indicador i = Indicador.Get(nombreIndicador);
                ViewBag.Fórmula = i.Fórmula;
                if (fórmula != null) {
                    i.Fórmula = fórmula;
                    i.Save();
                    TempData["Info"] = "Se modificó exitosamente al indicador de nombre \"" + nombreIndicador + '"';
                    return RedirectToAction("Menú", "Indicadores");
                }
            }
            return View();
        }

        public ActionResult Borrar(string nombreIndicador = null) {
            RefreshUserID();
            if (nombreIndicador != null) {
                Indicador.Get(nombreIndicador).Borrar();
                TempData["Info"] = "Se borró exitosamente al indicador de nombre \"" + nombreIndicador + '"';
                return RedirectToAction("Menú", "Indicadores");
            }
            ViewBag.NombresIndicadores = Indicador.GetNombres(usuarioID);
            return View();
        }
    }
}