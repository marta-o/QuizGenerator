using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizGenerator.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int QuizId { get; set; }
        public List<Answer> Answers { get; set; }
    }
}
