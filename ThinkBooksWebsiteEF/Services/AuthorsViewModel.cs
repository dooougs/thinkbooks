using System.Collections.Generic;
using ThinkBooksWebsiteEF.Models;

namespace ThinkBooksWebsiteEF.Services
{
    public class AuthorsViewModel
    {
        public List<Author> Authors { get; set; }
        public int CountOfAuthors { get; set; }
    }
}