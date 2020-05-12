﻿using System;

namespace Blockcore.Platform.Networking.Exceptions
{
    public class MessageProcessingException : Exception
    {
        public MessageProcessingException(string? message) : base(message)
        {

        }

        public MessageProcessingException(string? message, Exception? innerException) : base(message, innerException)
        {

        }
    }
}
