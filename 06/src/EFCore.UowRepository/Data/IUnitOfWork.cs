using System;
using EFCore.UowRepository.Data.Repositories;

namespace EFCore.UowRepository.Data
{
    public interface IUnitOfWork : IDisposable
    {
        bool Commit();
        IDepartamentoRepository DepartamentoRepository { get; }
    }
}