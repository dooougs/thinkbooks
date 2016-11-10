using System;
using Dapper.Contrib.Extensions;

namespace ThinkBooksWebsiteTesting.Models
{
    [Table("BookStatus")]
    public class BookStatus
    {
        [Key]
        public int BookStatusID { get; set; }
        public string Name { get; set; }
    }

}