using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Logging.Modal
{
    public class MessageRecievedEventArgs : EventArgs
    {
        private MessageTypeEnum status;
        private string message;

        public MessageTypeEnum Status { get; set; }
        public string Message { get; set; }

        public MessageRecievedEventArgs(MessageTypeEnum status, string message)
        {
            this.status = status;
            this.message = message;
        }
    }
}
