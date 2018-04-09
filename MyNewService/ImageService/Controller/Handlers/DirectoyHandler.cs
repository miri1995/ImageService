using ImageService.Modal;
using System;
using System.IO;
using System.Linq;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Logging.Modal;
using ImageService.Server;

namespace ImageService.Controller.Handlers
{
    public class DirectoyHandler : IDirectoryHandler
    {
        #region Members
        private IImageController m_controller;              // The Image Processing Controller
        private ILoggingService m_logging;
        private FileSystemWatcher m_dirWatcher;             // The Watcher of the Dir
        private string m_path;                              // The Path of directory
        private readonly string[] validExtensions =
          { ".jpg", ".png", ".gif", ".bmp" };             //The only file types are relevant.
        #endregion

        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose;              // The Event That Notifies that the Directory is being closed


        public DirectoyHandler(ILoggingService logging, IImageController controller, string path)
        {
            this.m_logging = logging;
            this.m_controller = controller;
            this.m_path = path;
            this.m_dirWatcher = new FileSystemWatcher(this.m_path);
        }


       public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
       {
            // The Event that will be activated upon new Command
            bool result;
            string message = this.m_controller.ExecuteCommand(e.CommandID, e.Args, out result);
            if (result)
            {
                this.m_logging.Log(message, MessageTypeEnum.INFO);
            }
            else
            {
                this.m_logging.Log(message, MessageTypeEnum.FAIL);
            }
        }




        // The Function Recieves the directory to Handle
        public void StartHandleDirectory(string dirPath)
        {
            string message1 = "Enter Handle Directory " + dirPath;
            m_logging.Log(message1, MessageTypeEnum.INFO);
            // add all images in the directory to the output directory.
            string[] filesInDirectory = Directory.GetFiles(m_path);
            foreach (string file in filesInDirectory)
            {
                string message2 = "Handeling Directory " + file;
                m_logging.Log(message2, MessageTypeEnum.INFO);
                string extension = Path.GetExtension(file);
                if (this.validExtensions.Contains(extension))
                {
                    string[] args = { file };
                    OnCommandRecieved(this, new CommandRecievedEventArgs((int)CommandEnum.NewFileCommand,args, file));
                }
            }
            this.m_dirWatcher.Created += new FileSystemEventHandler(Watcher);
            this.m_dirWatcher.Changed += new FileSystemEventHandler(Watcher);
            //listen to directory
            this.m_dirWatcher.EnableRaisingEvents = true;
            string start_message = "Start handle to directory: " + dirPath;
            this.m_logging.Log(start_message, MessageTypeEnum.INFO);

        }

        private void Watcher(object sender, FileSystemEventArgs events_arg)
        {
           
            string extension = Path.GetExtension(events_arg.FullPath);
            // check that the file is an image.
            if (this.validExtensions.Contains(extension))
            {
                string[] args = { events_arg.FullPath };
                CommandRecievedEventArgs commandRecievedEventArgs = new CommandRecievedEventArgs((int)CommandEnum.NewFileCommand, args, "");
                this.OnCommandRecieved(this, commandRecievedEventArgs);
            }

        }

        public void StopHandler(object sender, DirectoryCloseEventArgs close_event)
        {
            try
            {
                // stop listen to directory
                this.m_dirWatcher.EnableRaisingEvents = false;
                // remove OnCommandRecieved from the CommandRecived Event.
                ((ImageServer)sender).CommandRecieved -= this.OnCommandRecieved;
                this.m_logging.Log("Closing the handler of the directory " + this.m_path, MessageTypeEnum.INFO);
            }
            catch (Exception ex)
            {
                this.m_logging.Log("Error in closing handler of the directory " + this.m_path + " "
                    + ex.ToString(), MessageTypeEnum.FAIL);
            }
        }

    }
}