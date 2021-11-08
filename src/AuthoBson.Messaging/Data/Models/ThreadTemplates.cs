using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthoBson.Shared.Data.Models;
using AuthoBson.Messaging.Data.Models;

namespace AuthoBson.Messaging.Data.Models.Templates
{
    public interface IThreadTemplate : IModelTemplate
    {
        int[] Content { get; set; }

        int[] Message { get; set; }

        bool Scheme(int[] Head, int[] Message);

        bool IsSchematic(string Head, string Message);

        bool IsSchematic(IThread Thread);
    }

    public class ThreadTemplate : IThreadTemplate, IModelTemplate
    {
        int[] Content { get; set; }

        int[] Message { get; set; }

        public bool Scheme(int[] Username, int[] Password, int[] Email, int[] Verified, int[] Reason)
        {
            return Username.Length == 2 && Password.Length == 2 && Email.Length == 2 && Verified.Length == 2 && Reason.Length == 2
                   && Username[0] < Username[1] && Password[0] < Password[1] && Email[0] < Email[1] && Verified[0] < Verified[1] && Reason[0] < Reason[1];
        }

        public bool IsSchematic(string Head, string Message) =>
        this.Content[0] <= Content.Length && Content.Length < this.Content[1]
        && this.Message[0] <= Message.Length && Message.Length < this.Message[1];

        public bool IsSchematic(IThread Thread) =>
        this.Content[0] <= Thread.Content.Header.Length && Thread.Content.Header.Length < this.Content[1]
        && this.Message[0] <= Thread.Message.;
    }
}
