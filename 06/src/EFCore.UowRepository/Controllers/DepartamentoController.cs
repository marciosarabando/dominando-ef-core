using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCore.UowRepository.Data;
using EFCore.UowRepository.Data.Repositories;
using EFCore.UowRepository.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EFCore.UowRepository.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DepartamentoController : ControllerBase
    {
        private readonly ILogger<DepartamentoController> _logger;
        /*private readonly IDepartamentoRepository _departamentoRepository;*/
        private readonly IUnitOfWork _unitOfWork;


        public DepartamentoController(ILogger<DepartamentoController> logger,
            /*IDepartamentoRepository departamentoRepository,*/
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            /*_departamentoRepository = departamentoRepository;*/
            _unitOfWork = unitOfWork;
        }

        //departamento/id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            //var departamento = await _departamentoRepository.GetByIdAsync(id);

            var departamento = await _unitOfWork.DepartamentoRepository.GetByIdAsync(id);

            return Ok(departamento);
        }

        //departamento
        [HttpPost()]
        public IActionResult CreateDepartamentoAsync(Departamento departamento)
        {
            //_departamentoRepository.Add(departamento);
            _unitOfWork.DepartamentoRepository.Add(departamento);

            //var saved = _departamentoRepository.Save();

            _unitOfWork.Commit();

            return Ok(departamento);
        }

        //departamento
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartamentoAsync(int id)
        {
            var departamento = await _unitOfWork.DepartamentoRepository.GetByIdAsync(id);

            _unitOfWork.DepartamentoRepository.Remove(departamento);

            _unitOfWork.Commit();

            return Ok(departamento);
        }

        //departamento/?descricao=teste
        [HttpGet()]
        public async Task<IActionResult> GetDepartamentoByDescricaoAsync([FromQuery] string descricao)
        {
            var departamento = await _unitOfWork.DepartamentoRepository.GetDataAsync(
                p => p.Descricao.Contains(descricao),
                p => p.Include(p => p.Colaboradores),
                orderBy: p => p.Id,
                take: 3
            );

            return Ok(departamento);
        }
    }
}
