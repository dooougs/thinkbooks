using System;
using Dapper.Contrib.Extensions;

namespace ThinkBooksWebsiteTesting.Models
{
    public class Author
    {
        [Key]
        public int AuthorID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

}