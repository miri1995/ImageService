﻿
using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Server
{
    class ImageServer
    {
        #region Members
        private IImageController m_controller;
        private ILoggingService m_logging;
        #endregion

        #region Properties
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;          // The event that notifies about a new Command being recieved
        #endregion

        public ImageServer(IImageController controller,ILoggingService logging)
        {
            this.m_controller = controller;
            this.m_logging = logging;
            string[] directories = (ConfigurationManager.AppSettings.Get("Handler").Split(';'));
            foreach (string path in directories)
            {
                try
                {
                    this.CreateHandler(path);
                }
                catch (Exception ex)
                {
                    this.m_logging.Log("Error while creating handler for directory: " + path + " because:" + ex.ToString(), Logging.Modal.MessageTypeEnum.FAIL);
                }

            }
        }




        private void CreateHandler(string path)
        {
            IDirectoryHandler handler = new DirectoyHandler( m_logging, m_controller,path);
            CommandRecieved += handler.OnCommandRecieved;
            handler.DirectoryClose += this.onClose;
            this.m_logging.Log("Created handler for: " + path, Logging.Modal.MessageTypeEnum.INFO);
        }
    }
}
