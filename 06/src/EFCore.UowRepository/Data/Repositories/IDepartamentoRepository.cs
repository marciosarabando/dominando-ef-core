using System.Threading.Tasks;
using EFCore.UowRepository.Data.Repositories.Base;
using EFCore.UowRepository.Domain;

namespace EFCore.UowRepository.Data.Repositories
{
    public interface IDepartamentoRepository : IGenericRepository<Departamento>
    {
        //Task<Departamento> GetByIdAsync(int id);
        //void Add(Departamento departamento);
        //bool Save();
    }
}