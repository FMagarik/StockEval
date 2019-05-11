using System.Web.Mvc;
using DDS.Models;
using System.Web;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;

namespace DDS.Controllers {
    [Authorize]
    public class CuentasController : Controller {
        public ActionResult Importar() {
            return View();
        }

        public ActionResult ImportarLotes() {
            ViewBag.NúmeroArchivos = 0;
            FileInfo[] fis = new DirectoryInfo(MvcApplication.TempPath).GetFiles();
            foreach (var fi in fis)
                if (fi.Name.EndsWith(".csv") && fi.Length > 0)
                    ViewBag.NúmeroArchivos++;
            return View();
        }

        public ActionResult Menú() {
            return View();
        }

        private int ProcesarStream(Stream s) {
            int count = 0;
            using (TextFieldParser parser = new TextFieldParser(s)) {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(";");
                while (!parser.EndOfData) {
                    string[] fields = parser.ReadFields();
                    string nombreEmpresa = fields[0];
                    string nombreCuenta = fields[1];
                    int período = Convert.ToInt32(fields[2]);
                    double valor = Convert.ToDouble(fields[3]);
                    Empresa e = Empresa.Get(nombreEmpresa);
                    if (e == null) {
                        e = new Empresa(nombreEmpresa);
                        new Cuenta(e.EmpresaID, período, nombreCuenta, valor);
                        count++;
                    } else {
                        Cuenta c = e.Cuenta(nombreCuenta, período);
                        if (c == null) {
                            new Cuenta(e.EmpresaID, período, nombreCuenta, valor);
                            count++;
                        } else {
                            c.Valor = valor;
                            Cuenta.Save();
                            c.SyncValoresIndicadores();
                        }
                    }
                }
            }
            return count;
        }

        [HttpPost]
        public ActionResult Procesar(HttpPostedFileBase file) {
            if (file != null && file.ContentLength > 0 && file.FileName.EndsWith(".csv")) {
                int count = ProcesarStream(file.InputStream);
                TempData["Info"] = "Se agregaron exitosamente " + count + " cuentas";
            } else TempData["Error"] = "No se pudo procesar el archivo";
            return RedirectToAction("Menú", "Cuentas");
        }

        [HttpPost]
        public ActionResult ProcesarLotes() {
            int count = 0;
            FileInfo[] fis = new DirectoryInfo(MvcApplication.TempPath).GetFiles();
            foreach (var fi in fis) {
                if (fi.Name.EndsWith(".csv") && fi.Length > 0) {
                    count += ProcesarStream(fi.OpenRead());
                    System.IO.File.Move(fi.FullName, fi.DirectoryName + "\\Procesados\\" + fi.Name);
                } else System.IO.File.Move(fi.FullName, fi.DirectoryName + "\\Descartados\\" + fi.Name);
            }
            TempData["Info"] = "Se agregaron exitosamente " + count + " cuentas";
            return RedirectToAction("Menú", "Cuentas");
        }

        private void AgregarCuentasPorPeríodo(Empresa e, int período) {
            if (período != 0) {
                var cuentas = e.CuentasDelPeríodo(período);
                foreach (var c in cuentas)
                    filas.Add(new Tuple<string, string, int, string>(c.Nombre, e.Nombre, período, c.Valor.ToString()));
            }
        }

        private void AgregarCuentasHistóricas(Empresa e) {
            foreach (var c in e.TodasCuentas())
                filas.Add(new Tuple<string, string, int, string>(c.Nombre, e.Nombre, c.Período, c.Valor.ToString()));
        }

        private void AgregarCuentas(Empresa e, int p, bool esHistórico) {
            if (esHistórico) AgregarCuentasHistóricas(e);
            else AgregarCuentasPorPeríodo(e, p);
        }

        private List<Tuple<string, string, int, string>> filas;

        public ActionResult Visualizar(string nombreEmpresa = null, int período = 0, bool esComparativo = false, bool esHistórico = false) {
            ViewBag.NombreEmpresa = nombreEmpresa;
            ViewBag.NombresEmpresas = Empresa.GetNombres();
            ViewBag.Período = período;
            ViewBag.Períodos = Empresa.GetPeríodos();

            filas = new List<Tuple<string, string, int, string>>();
            if (esComparativo) {
                foreach (Empresa e in Empresa.GetEmpresas())
                    AgregarCuentas(e, período, esHistórico);
            } else if (nombreEmpresa != null) {
                Empresa e = Empresa.Get(nombreEmpresa);
                if (e != null && período != 0)
                    AgregarCuentas(e, período, esHistórico);
            }
            if (filas.Count > 0) ViewBag.Filas = filas;

            return View();
        }
    }
}