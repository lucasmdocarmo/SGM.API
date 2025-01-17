﻿using Flunt.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGM.Cidadao.API.Extensions;
using SGM.Cidadao.Application.Commands.Endereco;
using SGM.Cidadao.Infra.Repositories.Contracts;
using SGM.Shared.Core.Application;
using SGM.Shared.Core.Commands.Handler;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SGM.Cidadao.API.Controllers
{
    [AttributesExtensions]
    [ApiVersion("1.0")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class EnderecoController : Controller
    {
        private readonly ICommandHandler<CadastrarEnderecoCommand> _commandCadastrar;
        private readonly ICommandHandler<EditarEnderecoCommand> _commandEditar;
        private readonly ICommandHandler<DeletarEnderecoCommand> _commandDeletar;
        private readonly ICidadaoRepository _cidadaoRepository;
        private readonly IEnderecoRepository _enderecoRepository;

        public EnderecoController(ICommandHandler<CadastrarEnderecoCommand> commandCadastrar, ICommandHandler<EditarEnderecoCommand> commandEditar,
            ICommandHandler<DeletarEnderecoCommand> commandDeletar, IEnderecoRepository enderecoRepository, ICidadaoRepository cidadaoRepository)
        {
            _commandCadastrar = commandCadastrar;
            _commandEditar = commandEditar;
            _commandDeletar = commandDeletar;
            _enderecoRepository = enderecoRepository;
            _cidadaoRepository = cidadaoRepository;
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Notification), StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(typeof(Notification), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _enderecoRepository.GetAll().ConfigureAwait(true);
            if (result is null) { return NoContent(); }
            return Ok(result);
        }

        [HttpGet("{cpf}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Notification), StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(typeof(Notification), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById([FromRoute][Required] string cpf)
        {
            var cidadao = await _cidadaoRepository.Search(x=>x.CPF == cpf).ConfigureAwait(true);
            if(cidadao == null) { return UnprocessableEntity("Cidadão não encontrado."); }
            var endereco = await _enderecoRepository.Search(x => x.CidadaoId == cidadao.FirstOrDefault().Id);
            if (endereco is null) { return UnprocessableEntity("Endereço não encontrado."); }

            return Ok(endereco);
        }

        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Notification), StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(typeof(Notification), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateItem([FromRoute][Bind] Guid id, [FromBody][Bind] EditarEnderecoCommand command)
        {
            if (!ModelState.IsValid)
            {
                var notifications = new List<Notification>();
                foreach (var erro in ModelState.Where(a => a.Value.Errors.Count > 0).SelectMany(x => x.Value.Errors).ToList())
                {
                    notifications.Add(new Notification("invalidModel", erro.ErrorMessage));
                }
                return StatusCode(412, notifications.ToList());
            }

            var result = await _commandEditar.Handle(command).ConfigureAwait(true) as CommandResult;

            if (!result.Success)
            {
                return UnprocessableEntity();
            }
            return NoContent();
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Notification), StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(typeof(Notification), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateItem([FromBody] CadastrarEnderecoCommand command)
        {
            if (!ModelState.IsValid)
            {
                var notifications = new List<Notification>();
                foreach (var erro in ModelState.Where(a => a.Value.Errors.Count > 0).SelectMany(x => x.Value.Errors).ToList())
                {
                    notifications.Add(new Notification("invalidModel", erro.ErrorMessage));
                }
                return StatusCode(412, notifications.ToList());
            }

            var result = await _commandCadastrar.Handle(command).ConfigureAwait(true) as CommandResult;

            if (!result.Success)
            {
                return UnprocessableEntity(result.Messages);
            }
            return Ok(result.Result);
        }

        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Notification), StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(typeof(Notification), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteItem([FromRoute][Bind][Required] Guid id)
        {
            var command = new DeletarEnderecoCommand() { Id = id };
            var result = await _commandDeletar.Handle(command).ConfigureAwait(true) as CommandResult;

            if (!result.Success)
            {
                return UnprocessableEntity();
            }
            return Ok(result.Result);
        }
    }
}
