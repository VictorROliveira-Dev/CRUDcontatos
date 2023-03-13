using CrudContatos.Data;
using CrudContatos.Models;
using System.Security.Cryptography;

namespace CrudContatos.Repositorio
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        //injeção de dependência:
        private readonly BancoContext _Context;

        public UsuarioRepositorio(BancoContext bancoContext)
        {
            _Context = bancoContext;
        }


        public List<UsuarioModel> BuscarContatos()
        {
            return _Context.Usuarios.ToList();
        }


        public UsuarioModel ListarPorId(int id)
        {
            return _Context.Usuarios.FirstOrDefault(x => x.Id == id);
        }

        public UsuarioModel Adicionar(UsuarioModel usuario)
        {
            usuario.DataCadastro = DateTime.Now;
            usuario.SetSenhaHash();
            _Context.Usuarios.Add(usuario);
            _Context.SaveChanges();
            return usuario;
        }

        public UsuarioModel Atualizar(UsuarioModel usuario)
        {
            UsuarioModel usuarioDB = ListarPorId(usuario.Id);

            if (usuarioDB == null) throw new System.Exception("Erro! Não existe o usuário.");

            usuarioDB.Nome = usuario.Nome;
            usuarioDB.Email = usuario.Email;
            usuarioDB.Login = usuario.Login;
            usuarioDB.Senha = usuario.Senha;
            usuarioDB.Perfil = usuario.Perfil;
            usuario.DataAtualizacao = DateTime.Now;

            _Context.Usuarios.Update(usuarioDB);
            _Context.SaveChanges();    

            return usuarioDB;
        }

        public bool Apagar(int id)
        {
            UsuarioModel usuarioDB = ListarPorId(id);

            if (usuarioDB == null) throw new System.Exception("Erro! Não existe o usuário para ser deletado");

            _Context.Usuarios.Remove(usuarioDB);
            _Context.SaveChanges();

            return true;

        }

        public UsuarioModel BuscarPorLogin(string login)
        {
            return _Context.Usuarios.FirstOrDefault(l => l.Login.ToUpper() == login.ToUpper());
        }

        public UsuarioModel BuscarEmailELogin(string email, string login)
        {
            return _Context.Usuarios.FirstOrDefault(x => x.Email.ToUpper() == email.ToUpper() && x.Login.ToUpper() == login.ToUpper());
        }

        public UsuarioModel AlterarSenha(AlterarSenhaModel alterarSenhaModel)
        {
            UsuarioModel usuarioDB = ListarPorId(alterarSenhaModel.Id);

            if (usuarioDB == null) throw new Exception("Houve um erro na atualização da senha, usuário não encontrado");

            if (!usuarioDB.SenhaValida(alterarSenhaModel.SenhaAtual)) throw new Exception("Senha atual não confere. Tente novamente.");

            if (usuarioDB.SenhaValida(alterarSenhaModel.NovaSenha)) throw new Exception("A nova senha deve ser diferente da atual.");

            usuarioDB.SetNovaSenha(alterarSenhaModel.NovaSenha);
            usuarioDB.DataAtualizacao = DateTime.Now;

            _Context.Usuarios.Update(usuarioDB);
            _Context.SaveChanges();

            return usuarioDB;
        }
    }
}
