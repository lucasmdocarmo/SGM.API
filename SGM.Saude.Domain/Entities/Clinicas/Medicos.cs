﻿using SGM.Saude.Domain.Entities.Clinicas;
using SGM.Shared.Core.Contracts;
using SGM.Shared.Core.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGM.Saude.Domain.Entities
{
    public sealed class Medicos :BaseEntity, IAggregateRoot
    {
        public string Nome { get; set; }
        public string Profissao { get; set; }
        public string Especialidade { get; set; }
        public string DiasDisponiveis { get; set; }
        public bool Ativo { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public decimal ValorHora { get; set; }

        public Clinica Clinica { get; set; }
        public Guid ClinicaId { get; set; }

        public Endereco Endereco { get; set; }
        public Guid EnderecoId { get; set; }
    }
}