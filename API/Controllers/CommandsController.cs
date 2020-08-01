using AutoMapper;
using Commander.Data;
using Commander.Dtos;
using Commander.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Commander.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommanderRepo _repository;
        private readonly IMapper _mapper;

        public CommandsController(ICommanderRepo repository, IMapper mapper) 
        {
            _repository = repository;
            _mapper = mapper;
        }

        //private readonly MockCommanderRepo _repository = new MockCommanderRepo();

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetAllCommands([FromQuery] string howTo)
        {
            var commandItems = _repository.GetAllCommands(howTo);

            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commandItems));
        }

        [HttpGet("{id}", Name = "GetCommandById")]
        public ActionResult<CommandReadDto> GetCommandById(int id)
        {
            var commandItem = _repository.GetCommandById(id);

            if(commandItem != null)
            {
                return Ok(_mapper.Map<CommandReadDto>(commandItem));
            }

            return NotFound();
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommand(CommandCreateDto commandCreateDto)
        {
            var commandModel = _mapper.Map<Command>(commandCreateDto); //Map from dto to model
            _repository.CreateCommand(commandModel); //First you have to map the DTO and then you call the DB create method
            _repository.SaveChanges(); //Commit your changes

            var commandReadDto = _mapper.Map<CommandReadDto>(commandModel); //Map it back to DTO to respond to client

            return CreatedAtRoute(nameof(GetCommandById), new { Id = commandReadDto.Id }, commandReadDto); //It finds the created object in order to manipulate the reponse code
        }

        [HttpPut("{id}")]
        public ActionResult UpdateCommand(int id, CommandUpdateDto commandUpdateDto)
        {
            var commandModelFromRepo = _repository.GetCommandById(id);

            if(commandModelFromRepo == null)
            {
                return NotFound();
            }

            _mapper.Map(commandUpdateDto, commandModelFromRepo);

            _repository.UpdateCommand(commandModelFromRepo);

            _repository.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public ActionResult PartialCommandUpdate(int id, JsonPatchDocument<CommandUpdateDto> patchDoc)
        {
            var commandModelFromRepo = _repository.GetCommandById(id);

            if (commandModelFromRepo == null)
            {
                return NotFound();
            }

            var commandToPatch = _mapper.Map<CommandUpdateDto>(commandModelFromRepo);

            patchDoc.ApplyTo(commandToPatch, ModelState);

            if (!TryValidateModel(commandToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(commandToPatch, commandModelFromRepo);

            _repository.UpdateCommand(commandModelFromRepo);

            _repository.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteCommand(int id)
        {
            var commandModelFromRepo = _repository.GetCommandById(id);

            if (commandModelFromRepo == null)
            {
                return NotFound();
            }

            _repository.DeleteCommand(commandModelFromRepo);

            _repository.SaveChanges();

            return NoContent();
        }
    }
}