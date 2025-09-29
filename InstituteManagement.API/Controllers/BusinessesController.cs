using AutoMapper;
using InstituteManagement.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InstituteManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessesController : AppControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly AppDbContext _appDbContext;

        public BusinessesController(
            UserManager<AppUser> userManager,
            IMapper mapper,
            AppDbContext appDbContext)
        {
            _userManager = userManager;
            _mapper = mapper;
            _appDbContext = appDbContext;
        }


        [HttpGet("institutes")]
        public async Task<IActionResult> GetInstitutes()
        {
            var institutes = await _appDbContext.InstituteProfiles
                .Select(i => new
                {
                    i.Id,
                    i.DisplayName,
                    i.LogoUrl
                })
                .ToListAsync();

            return Ok(institutes);
        }

        [HttpGet("gyms")]
        public async Task<IActionResult> GetGyms()
        {
            var gyms = await _appDbContext.GymProfiles
                .Select(g => new
                {
                    g.Id,
                    g.DisplayName,
                    g.LogoUrl
                })
                .ToListAsync();

            return Ok(gyms);
        }

    }
}
