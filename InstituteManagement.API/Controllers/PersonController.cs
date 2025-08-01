using AutoMapper;
using InstituteManagement.Core.Entities;
using InstituteManagement.Infrastructure;
using InstituteManagement.Shared.Common;
using InstituteManagement.Shared.DTOs.Persons;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InstituteManagement.API.Controllers
{
    [ApiController]
    [Route("api/people")]
    public class PersonController(AppDbContext context, IMapper mapper) : Controller
    {

        private readonly AppDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<PersonDto>>> Create(CreatePersonDto dto)
        {
            var person = _mapper.Map<Person>(dto); // Or manual mapping
            _context.People.Add(person);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<PersonDto>(person);
            return ApiResponse<PersonDto>.Ok(result);
        }

    }
}
