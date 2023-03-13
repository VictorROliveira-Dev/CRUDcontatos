using CrudContatos.Filters;
using CrudContatos.Helper;
using CrudContatos.Models;
using CrudContatos.Repositorio;
using Microsoft.AspNetCore.Mvc;

namespace CrudContatos.Controllers
{
    [PaginaParaUsuarioLogado]
    public class ContatoController : Controller
    {
        private readonly IContatoRepositorio _contatoRepositorio;
        private readonly ISessao _sessao;

        public ContatoController(IContatoRepositorio contatoRepositorio, ISessao sessao)
        {
            _contatoRepositorio = contatoRepositorio;
            _sessao = sessao;
        }

        public IActionResult Index()
        {
            UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
            List<ContatoModel> contatos = _contatoRepositorio.BuscarContatos(usuarioLogado.Id);
            return View(contatos);
        }

        public IActionResult Adicionar()
        {
            return View();
        }

        public IActionResult Editar(int id)
        {
            ContatoModel contato = _contatoRepositorio.ListarPorId(id);
            return View(contato);
        }

        public IActionResult ApagarConfirmacao(int id)
        {
            ContatoModel contato = _contatoRepositorio.ListarPorId(id);
            return View(contato);
        }

        public IActionResult Apagar(int id)
        {
            try
            {
                bool apagado = _contatoRepositorio.Apagar(id);

                if (apagado)
                {
                    TempData["MensagemSucesso"] = "Contato deletado com sucesso.";
                }
                else
                {
                    TempData["MensagemErro"] = "Não foi possível deletar o contato";
                }
                return RedirectToAction("Index");
            }
            catch (Exception error)
            {
                TempData["MensagemErro"] = $"Não foi possível apagar o contato. Detalhes do erro: {error.Message}";
                return RedirectToAction("Index");
            }            
        }

        [HttpPost]
        public IActionResult Adicionar(ContatoModel contato)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
                    contato.UsuarioId = usuarioLogado.Id;

                    contato = _contatoRepositorio.Adicionar(contato);

                    TempData["MensagemSucesso"] = "Contato cadastrado com sucesso.";
                    return RedirectToAction("Index");
                }
                return View(contato);
            }
            catch (Exception error)
            {
                TempData["MensagemErro"] = $"Não foi possível cadastrar o contato. Detalhes do erro: {error.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Alterar(ContatoModel contato)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
                    contato.UsuarioId = usuarioLogado.Id;

                    contato = _contatoRepositorio.Atualizar(contato);

                    TempData["MensagemSucesso"] = "Contato alterado com sucesso.";
                    return RedirectToAction("Index");
                }
                return View("Editar", contato);
            }
            catch (Exception error)
            {
                TempData["MensagemErro"] = $"Não foi possível alterar o contato. Detalhes do erro: {error.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}
