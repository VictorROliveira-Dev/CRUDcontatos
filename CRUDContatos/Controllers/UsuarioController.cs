using CrudContatos.Filters;
using CrudContatos.Models;
using CrudContatos.Repositorio;
using Microsoft.AspNetCore.Mvc;

namespace CrudContatos.Controllers
{
    [PaginaSomenteAdmin]
    public class UsuarioController : Controller
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;

        public UsuarioController(IUsuarioRepositorio usuarioRepositorio)
        {
            _usuarioRepositorio = usuarioRepositorio;
        }

        public IActionResult Index()
        {
            List<UsuarioModel> usuarios = _usuarioRepositorio.BuscarContatos();

            return View(usuarios);
        }

        public IActionResult Adicionar()
        {
            return View();
        }

        public IActionResult Editar(int id)
        {
            UsuarioModel usuario = _usuarioRepositorio.ListarPorId(id);
            return View(usuario);
        }

        [HttpPost]
        public IActionResult Editar(UsuarioSemSenhaModel usuarioSemSenhaModel)
        {
            try
            {
                UsuarioModel usuario = null;

                if (ModelState.IsValid)
                {
                    usuario = new UsuarioModel()
                    {
                        Id = usuarioSemSenhaModel.Id,
                        Nome = usuarioSemSenhaModel.Nome,
                        Login = usuarioSemSenhaModel.Login,
                        Email = usuarioSemSenhaModel.Email,
                        Perfil = usuarioSemSenhaModel.Perfil
                    };

                    usuario = _usuarioRepositorio.Atualizar(usuario);
                    TempData["MensagemSucesso"] = "Usuário alterado com sucesso.";
                    return RedirectToAction("Index");
                }
                return View("Editar", usuario);
            }
            catch (Exception error)
            {
                TempData["MensagemErro"] = $"Não foi possível alterar o usuário. Detalhes do erro: {error.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Adicionar(UsuarioModel usuario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _usuarioRepositorio.Adicionar(usuario);

                    TempData["MensagemSucesso"] = "Usuário cadastrado com sucesso.";
                    return RedirectToAction("Index");
                }
                return View(usuario);
            }
            catch (Exception error)
            {
                TempData["MensagemErro"] = $"Não foi possível cadastrar o usuário. Detalhes do erro: {error.Message}";
                return RedirectToAction("Index");
            }
        }

        public IActionResult ApagarConfirmacao(int id)
        {
            UsuarioModel usuario = _usuarioRepositorio.ListarPorId(id);
            return View(usuario);
        }

        public IActionResult Apagar(int id)
        {
            try
            {
                bool apagado = _usuarioRepositorio.Apagar(id);

                if (apagado)
                {
                    TempData["MensagemSucesso"] = "Usuário deletado com sucesso.";
                }
                else
                {
                    TempData["MensagemErro"] = "Não foi possível deletar o usuário"; 
                }
                return RedirectToAction("Index");
            }
            catch (Exception error)
            {
                TempData["MensagemErro"] = $"Não foi possível deletar o usuário. Detalhes do erro: {error.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}
