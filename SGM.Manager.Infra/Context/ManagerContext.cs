﻿using Flunt.Notifications;
using Microsoft.EntityFrameworkCore;
using SGM.Manager.Domain.Entities;
using SGM.Manager.Domain.Entities.Integration;
using SGM.Shared.Core.Contracts.UnitOfWork;
using SGM.Shared.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGM.Manager.Infra.Context
{
    public class ManagerContext : DbContext, IUnitOfWork
    {
        public ManagerContext(DbContextOptions<ManagerContext> options) : base(options) { }

        public DbSet<Departamento> Departamento { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Funcionario> Funcionario { get; set; }
        public DbSet<AppIntegration> AppIntegration { get; set; }
        public DbSet<CidadaoUser> UserCidadao { get; set; }

        public void Rollback() => Database.RollbackTransaction();
        public void Begin() => Database.BeginTransaction();
        public async Task<bool> Commit() => await base.SaveChangesAsync() > 0;
        public bool CheckIfDatabaseExists() => Database.CanConnect();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys())) relationship.DeleteBehavior = DeleteBehavior.Cascade;

            modelBuilder.Ignore<Notification>();
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DbContext).Assembly);
            base.OnModelCreating(modelBuilder);
            modelBuilder.Seed();
        }
    }
    internal static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            var departamento = new Departamento("Clinica", "CLI001");
            var departamento1 = new Departamento("Saude", "SAU001");
            var departamento2 = new Departamento("Gerencia", "GER001");
            var departamento3 = new Departamento("Presidencia", "GER002");

            modelBuilder.Entity<Departamento>().HasData(departamento, departamento1, departamento2, departamento3);

            var user = new Usuario("Lucas Mariano", "123456", "lucasmdc@gmail.com", Shared.Core.ValueObjects.ETipoUsuario.Administrador, departamento.Id,"11640810633");
            var user2 = new Usuario("Sebastiao", "123456", "sebastic@gmail.com", Shared.Core.ValueObjects.ETipoUsuario.Administrador, departamento2.Id, "12345678991");
            var user3 = new Usuario("Steve Rogers", "123456", "america@gmail.com", Shared.Core.ValueObjects.ETipoUsuario.Administrador, departamento3.Id, "14474729192");

            modelBuilder.Entity<Usuario>().HasData(user, user2, user3);

            var func = new Funcionario("lucas","lucasm", "123456", Shared.Core.ValueObjects.ETipoFuncionario.Clinica, departamento.Id, "11640810633");
            var func2 = new Funcionario("Sebastiao", "sebasx", "123456", Shared.Core.ValueObjects.ETipoFuncionario.Gestao, departamento2.Id, "12345678991");
            var func3 = new Funcionario("Steve Rogers", "america", "123456", Shared.Core.ValueObjects.ETipoFuncionario.Gestao, departamento3.Id, "14474729192");

            modelBuilder.Entity<Funcionario>().HasData(func, func2, func3);

            var integration1 = new AppIntegration("chave-cidadao", ESistema.Cidadao);
            var integration2 = new AppIntegration("chave-manager", ESistema.Manager);
            var integration3 = new AppIntegration("chave-saude", ESistema.Saude);

            modelBuilder.Entity<AppIntegration>().HasData(integration1, integration2, integration3);
        }
    }
}
