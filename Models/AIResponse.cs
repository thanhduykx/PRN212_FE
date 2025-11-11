using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentPRN212.Models
{

    // DTO gửi chat request
    public class ChatRequest
    {
        public string Message { get; set; }
    }

    // DTO nhận AI response
    public class AIResponse
    {
        public string Response { get; set; }
    }
    // DTO cho chat bot
    public class ChatResponse
    {
        public string Reply { get; set; } // JSON: reply
    }

    // DTO gửi tin nhắn chat

}

