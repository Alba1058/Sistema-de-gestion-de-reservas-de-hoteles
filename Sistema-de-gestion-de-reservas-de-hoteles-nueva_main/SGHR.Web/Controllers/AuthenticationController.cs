using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Usuarios.Usuario;
using SGHR.Application.Interfaces;
using SGHR.Application.Interfaces.Authentication;
using SGHR.Domain.Entities.Configuration;

namespace SGHR.Web.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IAuthenticationServices _authServices;

        public AuthenticationController(IAuthenticationServices authServices)
        {
            _authServices = authServices;
        }

        public IActionResult Index()
        {
            return View();
        }


        //GET: /Authentication/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //POST: /Authentication/Login
        [HttpPost]
        public async Task<IActionResult> Login(string correo, string contraseña)
        {
            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contraseña))
            {
                ViewBag.Error = "Debe ingresar correo y contraseña.";
                return View();
            }

            var result = await _authServices.LoginSesionAsync(correo, contraseña);

            if (!result.Success)
            {
                ViewBag.Error = result.Message;
                return View();
            }

            var usuario = result.Data as UsuarioDTO;

            HttpContext.Session.SetInt32("UserId", usuario.Id);
            HttpContext.Session.SetString("UserName", usuario.Nombre);
            HttpContext.Session.SetString("UserRole", usuario.RolNombre);

            TempData["Success"] = result.Message;
            switch (usuario.RolNombre)
            {
                case "Recepcionista":
                    return RedirectToAction("Index", "Dashboard", new { area = "Recepcionista" });
                case "Administrador":
                    return RedirectToAction("Index", "Dashboard", new { area = "Administrador" });
                case "Cliente":
                    return RedirectToAction("Index", "Dashboard", new { area = "Cliente" });

                default:
                    return RedirectToAction("Login");
            }

        }


    }
}
