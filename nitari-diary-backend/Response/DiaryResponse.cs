using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nitari_diary_backend.Response
{
  public class DiaryResponse
  {
    public string UserId { get; set; }
    public string Date { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string CreatedAt { get; set; }
    public string UpdatedAt { get; set; }
  }
}
