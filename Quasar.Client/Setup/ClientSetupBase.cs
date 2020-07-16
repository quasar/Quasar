using Quasar.Client.User;

namespace Quasar.Client.Setup
{
    public abstract class ClientSetupBase
    {
        protected UserAccount UserAccount;

        protected ClientSetupBase()
        {
            UserAccount = new UserAccount();
        }
    }
}
