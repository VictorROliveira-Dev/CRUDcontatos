using CrudContatos.Data;
using CrudContatos.Models;
using System.Security.Cryptography;

namespace CrudContatos.Repositorio
{
    public class ContatoRepositorio : IContatoRepositorio
    {
        //injeção de dependência:
        private readonly BancoContext _bancoContext;

        public ContatoRepositorio(BancoContext bancoContext)
        {
            _bancoContext = bancoContext;
        }


        public List<ContatoModel> BuscarContatos(int usuarioId)
        {
            return _bancoContext.Contatos.Where(x => x.Id == usuarioId).ToList();
        }


        public ContatoModel ListarPorId(int id)
        {
            return _bancoContext.Contatos.FirstOrDefault(ct => ct.Id == id);
        }

        public ContatoModel Adicionar(ContatoModel contato)
        {
            _bancoContext.Contatos.Add(contato);
            _bancoContext.SaveChanges();
            return contato;
        }

        public ContatoModel Atualizar(ContatoModel contato)
        {
            ContatoModel contatoDB = ListarPorId(contato.Id);

            if (contatoDB == null) throw new System.Exception("Erro! Não existe o contato.");

            contatoDB.Nome = contato.Nome;
            contatoDB.Email = contato.Email;
            contatoDB.Celular = contato.Celular;

            _bancoContext.Contatos.Update(contatoDB);
            _bancoContext.SaveChanges();    

            return contatoDB;
        }

        public bool Apagar(int id)
        {
            ContatoModel contatoDB = ListarPorId(id);

            if (contatoDB == null) throw new System.Exception("Erro! Não existe o contato para ser deletado");

            _bancoContext.Contatos.Remove(contatoDB);
            _bancoContext.SaveChanges();

            return true;

        }
    }
}
