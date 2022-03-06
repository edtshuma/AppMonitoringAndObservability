using System.ComponentModel.DataAnnotations;

namespace PlatformService.Dtos
{
    public class  PlatformCreateDto 
    {
       //id FIELD nt rqd handled by DB
          [Required]
        public string Name { get; set; }
        
          [Required]
        public string Publisher { get; set; }
         
        [Required]
        public string Cost { get; set; }
    }
}