
using System.Collections;
using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Models;
using PlatformsService.SyncDataService.Http;
using System.Threading.Tasks;
using PlatformService.Dtos;

namespace PlatformService
{
    //ControllerBase is fo APIs
    [Route("api/[controller]")]
    [ApiController]
   public class PlatformsController : ControllerBase {
        private readonly IMapper _mapper;
        private IPlatformRepo _repository;
        private readonly ICommandDataClient _commandDataClient;

        public PlatformsController(IPlatformRepo repository, 
        IMapper mapper,
        ICommandDataClient commandDataClient)
       {
           _mapper = mapper;
           _repository =repository;
           _commandDataClient = commandDataClient;
       }

        [HttpGet]
       public ActionResult<IEnumerable<PlatformReadDto>> GetAllPlatforms() {
       
       Console.WriteLine("Getting all platforms ...");

        var platformItems= _repository.GetAllPlatforms();

        return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
       }



         [HttpGet("{id}", Name="GetPlatformById")]
       public ActionResult<PlatformReadDto> GetPlatformById(int id) {
       
       Console.WriteLine("Getting all platforms ...");

        var platformItem = _repository.GetPlatformById(id);

        if (platformItem != null) {

           return Ok(_mapper.Map<PlatformReadDto>(platformItem));
        }

        return NotFound();
       }



       [HttpPost]
       public async Task<ActionResult<PlatformCreateDto>> CreatePlatform(PlatformCreateDto platformCreateDto) {
       
        var model= _mapper.Map<Platform>(platformCreateDto);
        _repository.CreatePlatform(model);
         _repository.SaveChanges();
         
         var platformReadDto= _mapper.Map<PlatformReadDto>(model);
        //where can u access the newly created resiurce >> CreatedAtRoute
        //name of the route <<GetPlatformById

        try {
           await _commandDataClient.SendPlatformToCommand(platformReadDto);
        }

        catch (Exception ex) {
          Console.WriteLine($"Could not send asynchronoulsy: { ex.Message}");
        }      
        return CreatedAtRoute(nameof(GetPlatformById), new {id=platformReadDto.Id}, platformReadDto);
       }

   }
}