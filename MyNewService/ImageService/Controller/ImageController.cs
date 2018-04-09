using ImageService.Commands;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Controller
{
    public class ImageController : IImageController
    {
        private IImageServiceModal m_modal;                      // The Modal Object
        private Dictionary<int, ICommand> commands;

        public ImageController(IImageServiceModal modal)
        {
            m_modal = modal;                    // Storing the Modal Of The System
            commands = new Dictionary<int, ICommand>()
            {
                {(int)commandsEnum.NewFileCommand,new NewFileCommand(modal) } // For Now will contain NEW_FILE_COMMAND
			};
        }
        public string ExecuteCommand(int commandID, string[] args, out bool resultSuccesful)
        {
            ICommand command;
            // extract command from dictionary if exists
            if (commands.TryGetValue(commandID, out command))
            {
                // create a thread to execute the command
                Task<Tuple<string, bool>> task = new Task<Tuple<string, bool>>(() => {
                    // execute the command
                    bool result;
                    return Tuple.Create(command.Execute(args, out result), result);
                });
                // activate the thread
                task.Start();
                // save result from thread
                Tuple<string, bool> output = task.Result;
                resultSuccessful = output.Item2;
                return output.Item1;
            }
            else
            {
                resultSuccessful = false;
                return "Not a command";
            }
        }
    }
}
