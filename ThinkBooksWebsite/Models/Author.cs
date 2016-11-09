using System;

namespace ThinkBooksWebsite.Models
{
    public class Author
    {
        public int AuthorID { get; set; }
        public int AuthorStatusID { get; set; }
        // Name of AuthorStatus
        public string AuthorStatusName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}