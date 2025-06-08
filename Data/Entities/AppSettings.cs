using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class AppSettings
    {
        public int Id { get; set; }
        public int TowerId { get; set; }
        public Tower Tower { get; set; }
        public string? LogoUrl { get; set; }
        public string? BannerUrl { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public ColorPalleteList Colors { get; set; } = new ColorPalleteList();
    }

    public class ColorPalleteList
    {
        public string Primary { get; set; } = "#FFFFFF";
        public string Secondary { get; set; } = "#000000"; 
        public string Accent { get; set; } = "#FF5733"; 
        public string Background { get; set; } = "#F0F0F0"; 
    }
}
