using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputHookManager.Tests
{
    public class InputControllerTests
    {
        [Fact]
        public void Attach_to_non_existent_process_byId()
        {
            var inputController = new InputController();

            try
            {
                inputController.Attach(12312412);
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
            var inputController = new InputController();

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
