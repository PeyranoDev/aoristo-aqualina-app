using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Responses
{

    public class AuthResponseDto
    {
        public string AccessToken { get; set; }
        public UserForResponse User { get; set; }
    }
}
