using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave_InteractionTest
{
    [TestFixture]
    public class IT3_UIAndCookControl
    {
        private Door doorSut;
        private UserInterface UI;

        private Button powerButtonSut;
        private Button timeButtonSut;
        private Button startCancelButtonSut;

        private CookController cooker;

        private IDisplay display;
        private ILight light;


        private ITimer timer;
        private IPowerTube powerTube;

        [SetUp]
        public void Setup()
        {
            //Arrange


            timer = Substitute.For<ITimer>();
            powerTube = Substitute.For<IPowerTube>();
            light = Substitute.For<ILight>();
            display = Substitute.For<IDisplay>();


            cooker = new CookController(timer,display,powerTube);



            doorSut = new Door();
            powerButtonSut = new Button();
            timeButtonSut = new Button();
            startCancelButtonSut = new Button();

            UI = new UserInterface(powerButtonSut, timeButtonSut, startCancelButtonSut, doorSut, display, light, cooker);
            
            
            cooker.UI = UI;

            //State is ready by default

        }
    }
}