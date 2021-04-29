using System.Threading;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;
using Timer = Microwave.Classes.Boundary.Timer;

namespace Microwave_InteractionTest
{
    [TestFixture]
    public class IT5_CookControlAndTimer
    {
        private Door doorSut;
        private UserInterface UI;

        private Button powerButtonSut;
        private Button timeButtonSut;
        private Button startCancelButtonSut;

        private Light light;
        private Timer timer;


        private CookController cooker;

        private IDisplay display;

        private IOutput output;


        private IPowerTube powerTube;

        [SetUp]
        public void Setup()
        {
            //Arrange


            
            powerTube = Substitute.For<IPowerTube>();
            display = Substitute.For<IDisplay>();
            output = Substitute.For<IOutput>();


            timer = new Timer();
            light = new Light(output);
            cooker = new CookController(timer, display, powerTube);



            doorSut = new Door();
            powerButtonSut = new Button();
            timeButtonSut = new Button();
            startCancelButtonSut = new Button();

            UI = new UserInterface(powerButtonSut, timeButtonSut, startCancelButtonSut, doorSut, display, light, cooker);


            cooker.UI = UI;

            //State is ready by default

        }

        [Test]
        public void StateSetTime_StartCancelPressed_TimerTicked()
        {
            //Arrange
            powerButtonSut.Press(); // State set to SetPower
            timeButtonSut.Press(); //State set to SetTime with a 1 minute timer.


            //Act
            startCancelButtonSut.Press();
            display.ClearReceivedCalls();

            Thread.Sleep(1005); // Wait one tick

            //Assert
            display.Received(1).ShowTime(Arg.Any<int>(), Arg.Any<int>());

        }

        [Test]
        public void StateSetTime_StartCancelPressed_TimerTickedTwice()
        {
            //Arrange
            powerButtonSut.Press(); // State set to SetPower
            timeButtonSut.Press(); //State set to SetTime with a 1 minute timer.


            //Act
            startCancelButtonSut.Press();
            display.ClearReceivedCalls();
            powerTube.ClearReceivedCalls();

            Thread.Sleep(2005); // Wait two ticks

            //Assert
            Assert.Multiple(() =>
            {
                display.Received(2).ShowTime(Arg.Any<int>(), Arg.Any<int>());
                powerTube.Received(0).TurnOff();
            });
        }

        [Test]
        public void StateSetTime_StartCancelPressed_TimerExpired()
        {
            //Arrange
            powerButtonSut.Press(); // State set to SetPower
            timeButtonSut.Press(); //State set to SetTime with a 1 minute timer.


            //Act
            startCancelButtonSut.Press();
            display.ClearReceivedCalls();
            powerTube.ClearReceivedCalls();

            Thread.Sleep(60005); // Wait a minute so timer expires

            //Assert
            Assert.Multiple(() =>
            {
                display.Received(60-1).ShowTime(Arg.Any<int>(), Arg.Any<int>());
                powerTube.Received(1).TurnOff();
            });
        }


        [Test]
        public void StateSetTime_StartCancelPressed_TimerNotExpired()
        {
            //Arrange
            powerButtonSut.Press(); // State set to SetPower
            timeButtonSut.Press(); //State set to SetTime with a 1 minute timer.


            //Act
            startCancelButtonSut.Press();
            display.ClearReceivedCalls();
            powerTube.ClearReceivedCalls();

            Thread.Sleep(59005); // Wait a litte less than a minute so timer doesn't expire

            //Assert
            Assert.Multiple(() =>
            {
                display.Received(59).ShowTime(Arg.Any<int>(), Arg.Any<int>());
                powerTube.Received(0).TurnOff();
            });
        }
    }
}