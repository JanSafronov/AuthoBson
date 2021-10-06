using AuthoBson.Messaging.Services.Shared;

namespace AuthoBson.Messaging.Services
{
    public class HomeService
    {
        private static int _numTimesCalled = 0;
        
        public ServiceResult<string> GetHome()
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