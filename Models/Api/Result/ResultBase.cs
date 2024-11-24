﻿namespace Server.Models.Api.Result
{
    public class ResultBase
    {
        public string Message { get; init; }
        public int StatusCode { get; init; }

        public ResultBase()
        { }

        public ResultBase(string message, int statusCode) =>
            (Message, StatusCode) = (message, statusCode);
    }
}
