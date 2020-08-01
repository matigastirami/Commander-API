using Commander.Controllers;
using Commander.Data;
using Commander.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Xunit;
using Xunit.Abstractions;

namespace commander_api_test
{
    public class CommanderIntegrationTests : BaseTest
    {
        private readonly ITestOutputHelper _output;

        //CommandsController _commandController;


        public CommanderIntegrationTests(ITestOutputHelper output) : base(
                new DbContextOptionsBuilder<CommanderContext>().UseSqlite("Filename=memory").Options
            )
        {
            _output = output;
        }
        
        [Fact]
        public void CreateContextTest()
        {
            using(var context = new CommanderContext(ContextOptions))
            {
                var sqlRepo = new SqlCommanderRepo(context);                

                var controller = new CommandsController(sqlRepo, getMapper());

                var request = new CommandCreateDto { HowTo = "Unit Test 1", Line = "Unit Test 1", Platform = "Unit Test 1" };

                var resp = controller.CreateCommand(request);

                

                _output.WriteLine($"{resp.ToString()}");

                //Assert.IsType(, resp.Result);


            }
        }
    }
}
