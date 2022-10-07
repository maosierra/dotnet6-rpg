using AutoMapper;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services.CharacterService
{
    public class CharacterServices : ICharacterService
    {
        private readonly IMapper mapper;
        private readonly DataContext context;

        public CharacterServices(IMapper mapper, DataContext context)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            var servicesResponse = new ServiceResponse<List<GetCharacterDto>>();
            var character = mapper.Map<Character>(newCharacter);
            context.Characters.Add(character);
            await context.SaveChangesAsync();
            servicesResponse.Data = await context.Characters
                .Select(c => mapper.Map<GetCharacterDto>(c))
                .ToListAsync();
            return servicesResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            var servicesResponse = new ServiceResponse<List<GetCharacterDto>>();
            var character = await context.Characters.FirstOrDefaultAsync(c => c.Id == id);

            if (character is null)
            {
                servicesResponse.Success = false;
                servicesResponse.Message = "Character not found";
                return servicesResponse;
            }

            context.Characters.Remove(character);
            await context.SaveChangesAsync();

            servicesResponse.Data = context.Characters.Select(c => mapper.Map<GetCharacterDto>(c)).ToList();
            return servicesResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacter(int userId)
        {
            var servicesResponse = new ServiceResponse<List<GetCharacterDto>>();
            var dbCharacters = await context.Characters
                .Where(c => c.User.Id == userId)
                .ToListAsync();
            servicesResponse.Data = dbCharacters.Select(c => mapper.Map<GetCharacterDto>(c)).ToList();
            return servicesResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            var servicesResponse = new ServiceResponse<GetCharacterDto>();
            var character = await context.Characters.FirstOrDefaultAsync(c => c.Id == id);
            if (character is null)
            {
                servicesResponse.Success = false;
                servicesResponse.Message = "Character not found";
                return servicesResponse;
            }
            servicesResponse.Data = mapper.Map<GetCharacterDto>(character);
            return servicesResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updateCharacter)
        {
            var servicesResponse = new ServiceResponse<GetCharacterDto>();
            var character = await context.Characters
                .FirstOrDefaultAsync(c => c.Id == updateCharacter.Id);

            if (character is null)
            {
                servicesResponse.Success = false;
                servicesResponse.Message = "Character not found";
                return servicesResponse;
            }

            mapper.Map(updateCharacter, character);

            await context.SaveChangesAsync();

            servicesResponse.Data = mapper.Map<GetCharacterDto>(character);
            return servicesResponse;
        }
    }
}