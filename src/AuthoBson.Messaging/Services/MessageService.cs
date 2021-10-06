using AuthoBson.Messaging.Services.Shared;
using AuthoBson.Shared.Data;

namespace AuthoBson.Messaging.Services
{
    public class MessageService : ISharedService<Message>
    {
        

        private static int _numTimesCalled = 0;
        
        public ServiceResult<string> GetMessage()
        {
            ++_numTimesCalled;

            if (_numTimesCalled > 3)
            {
                return ServiceResult.BadRequest<string>("Dude. Really.");
            }
            if (_numTimesCalled > 1)
            {
                return ServiceResult.Conflict<string>("I already told you we're up!");
            }
            
            return ServiceResult.Ok("Up and running!");
        }
    }
}