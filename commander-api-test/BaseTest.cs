using AutoMapper;
using Commander.Data;
using Commander.Dtos;
using Commander.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace commander_api_test
{
    public class BaseTest
    {
        protected BaseTest(DbContextOptions<CommanderContext> contextOptions)
        {
            ContextOptions = contextOptions;
            Seed();
        }

        protected DbContextOptions<CommanderContext> ContextOptions { get; }

        private void Seed()
        {
            using (var context = new CommanderContext(ContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var cmd = new Command { HowTo = "Test create command", Line = "Test line", Platform = "test" };

                context.Add(cmd);

                context.SaveChanges();
            }
        }

        protected Mapper getMapper()
        {
            var config = new MapperConfiguration(opts =>
            {
                opts.CreateMap<Command, CommandReadDto>();
                opts.CreateMap<CommandCreateDto, Command>();
                opts.CreateMap<CommandUpdateDto, Command>();
                opts.CreateMap<Command, CommandUpdateDto>();
            });

            return (Mapper)config.CreateMapper();
        }

        protected static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            return connection;
        }
    }
}
