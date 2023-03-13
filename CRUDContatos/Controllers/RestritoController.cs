using CrudContatos.Filters;
using Microsoft.AspNetCore.Mvc;

namespace CrudContatos.Controllers
{
    [PaginaParaUsuarioLogado]
    public class RestritoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
