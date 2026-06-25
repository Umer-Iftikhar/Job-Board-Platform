using Microsoft.AspNetCore.Identity;
using Moq;


namespace JobBoard.Api.Tests.Helpers
{
    public static class MockUserManager
    {
        public static Mock<UserManager<TUser>> Create<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(
                store.Object,
                null!, null!, null!, null!, null!, null!, null!, null!);
            return mgr;
        }
    }
}
