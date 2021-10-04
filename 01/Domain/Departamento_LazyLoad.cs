using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DominandoEntityFramework.Domain
{
    public class DepartamentoLazyLoad
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public bool Ativo { get; set; }

        /* exemplo de uso do LazyLoad com injeção de dependencia do Lazy Load na Entidade
        public Departamento()
        {
        }

        private ILazyLoader _lazyLoader { get; set; }
        private Departamento(ILazyLoader lazyLoader)
        {
            _lazyLoader = lazyLoader;
        }

        private List<Funcionario> _funcionarios;
        public List<Funcionario> Funcionarios
        {
            get => _lazyLoader.Load(this, ref _funcionarios);
            set => _funcionarios = value;
        }
        */

        public DepartamentoLazyLoad()
        {
        }

        private Action<object, string> _lazyLoader { get; set; }
        private DepartamentoLazyLoad(Action<object, string> lazyLoader)
        {
            _lazyLoader = lazyLoader;
        }

        private List<Funcionario> _funcionarios;
        public List<Funcionario> Funcionarios
        {
            get
            {
                _lazyLoader?.Invoke(this, nameof(Funcionarios));

                return _funcionarios;
            }
            set => _funcionarios = value;
        }

    }
}