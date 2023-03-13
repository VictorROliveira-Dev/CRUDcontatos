using CrudContatos.Helper;
using CrudContatos.Models;
using CrudContatos.Repositorio;
using Microsoft.AspNetCore.Mvc;

namespace CrudContatos.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly ISessao _sessao;
        private readonly IEmail _email;

        public LoginController(IUsuarioRepositorio usuarioRepositorio, ISessao sessao, IEmail email)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _sessao = sessao;
            _email = email;
        }

        public IActionResult Index()
        {
            //Quando o usuário já estiver logado, o redireciona para a home:
            if (_sessao.BuscarSessaoDoUsuario() != null) return RedirectToAction("Index", "Home");

            return View();
        }

        public IActionResult RedefinirSenha()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Logar(LoginModel loginModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UsuarioModel usuario = _usuarioRepositorio.BuscarPorLogin(loginModel.Login);

                    if (usuario != null)
                    {
                        if (usuario.SenhaValida(loginModel.Senha))
                        {
                            _sessao.CriarSessaoDoUsuario(usuario);
                            return RedirectToAction("Index", "Home");
                        }

                        TempData["MensagemErro"] = $"Senha do usuário é inválida, tente novamente.";
                    }

                    TempData["MensagemErro"] = $"Usuário e/ou senha inválida, tente novamente.";
                }

                return View("Index");
            }
            catch (Exception error)
            {

                TempData["MensagemErro"] = $"Não foi possível realizar o seu login, tente novamente. Detalhes do erro: {error.Message}";
                return RedirectToAction("Index"); 
            }
        }

        [HttpPost]
        public IActionResult EnviarLinkRedefinirSenha(RedefinirSenhaModel redefinirSenha)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UsuarioModel usuario = _usuarioRepositorio.BuscarEmailELogin(redefinirSenha.Email, redefinirSenha.Login);

                    if (usuario != null)
                    {
                        string novaSenha = usuario.GerarNovaSenha();                        
                        string mensagem = $"Sua nova senha é: {novaSenha}";

                        bool emailEnviado = _email.Enviar(usuario.Email, "Sistema de contatos - Nova senha", mensagem);

                        if (emailEnviado)
                        {
                            _usuarioRepositorio.Atualizar(usuario);
                            TempData["MensagemSucesso"] = $"Enviamos para o seu email uma nova senha.";
                        }
                        else
                        {
                            TempData["MensagemErro"] = $"Não conseguimos enviar o e-mail, tente novamente.";
                        }
                        
                        return RedirectToAction("Index", "Login");
                    }

                    TempData["MensagemErro"] = $"Login ou email inválidos, tente novamente.";
                }

                return View("Index");
            }
            catch (Exception error)
            {

                TempData["MensagemErro"] = $"Não foi possível fazer a redefinição de senha, tente novamente. Detalhes do erro: {error.Message}";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Deslogar()
        {
            _sessao.RemoverSessaoUsuario();

            return RedirectToAction("Index", "Login");
        }
    }
}
