using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Logging.Modal
{
    /// <summary>
    /// MessageRecievedEventArgs class.
    /// manages the args for log writting
    /// </summary>
    public class MessageRecievedEventArgs : EventArgs
    {
        #region members
        //members
        private MessageTypeEnum status;
        private string message;
        #endregion
        #region Properties
        public MessageTypeEnum Status
        {
            get { return status; }
            set { status = value; }
        }
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        #endregion
        /// <summary>
        /// MessageRecievedEventArgs function.
        /// </summary>
        /// <param name="status">type of log writting</param>
        /// <param name="message">message text</param>
        public MessageRecievedEventArgs(MessageTypeEnum status, string message)
        {
            this.status = status;
            this.message = message;
        }
    }
}