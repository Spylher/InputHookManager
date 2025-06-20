using System.Diagnostics;

namespace InputHookManager.Tests
{
    public class InputManagerTests
    {
        [Fact]
        public void Attach_to_non_existent_process_byId()
        {
            var inputController = new InputManager();

            try
            {
                inputController.Attach(1122344566);
            }
            catch (Exception)
            {
                return;
            }

            Assert.Fail("Fail expected");
        }

        [Fact]
        public void Attach_to_non_existent_process()
        {
            var inputController = new InputManager();

            try
            {
                inputController.Attach(new Process());
            }
            catch (Exception)
            {
                return;
            }

            Assert.Fail("Fail expected");
        }
    }
}
