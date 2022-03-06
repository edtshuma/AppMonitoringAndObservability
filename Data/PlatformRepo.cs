using PlatformService.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlatformService.Data
{
    public class PlatformRepo : IPlatformRepo
    {
        private readonly AppDbContext _context;

        public PlatformRepo(AppDbContext context)
        {
            //inject an insatnce of AppDbContext when PltformRepo is constructed
            //and aasign that instance to _context         
            _context = context;

        }
        public bool SaveChanges()
        {
            //if dbcommit was succesfull
            return (_context.SaveChanges() >= 0);
        }

        IEnumerable<Platform> IPlatformRepo.GetAllPlatforms()
        {
            return _context.Platforms.ToList();
        }

        public void CreatePlatform(Platform plat)
        {
            if (plat == null)
            {
                throw new ArgumentNullException(nameof(plat));
            }
            _context.Platforms.Add(plat);
        }

        public Platform GetPlatformById(int id)
        {
            return _context.Platforms.FirstOrDefault(p => p.Id == id);
        }
    }
}