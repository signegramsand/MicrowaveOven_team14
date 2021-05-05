using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;
using Timer = Microwave.Classes.Boundary.Timer;

namespace Microwave_InteractionTest.Test.Unit
{
    [TestFixture]
    public class IT8_CompleteSystemAndTimer
    {
        private Door doorSut;
        private UserInterface UI;

        private Button powerButtonSut;
        private Button timeButtonSut;
        private Button startCancelButtonSut;

        private Light light;
        private Timer timer;

        private Output output;

        private CookController cooker;

        private Display display;

        private PowerTube powerTube;

        private StringWriter ConsoleReader;

        [SetUp]
        public void Setup()
        {
            //Arrange
            timer = new Timer();
            

            output = new Output();

            ConsoleReader = new StringWriter();
            Console.SetOut(ConsoleReader);


            powerTube = new PowerTube(output);
            display = new Display(output);
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
        [TearDown]
        public void TearDown()
        {
            timer.Stop(); //Stopping timer, to avoid "ghost-timers" in subsequent tests
        }


        [Test]
        public void StateReady_DoorOpen_LightTurnsOnAndOutputIsCalled()
        {
            //Arrange
            ConsoleReader.GetStringBuilder().Clear();

            //Act
            doorSut.Open();

            //Assert

            string[] OutputLines = ConsoleReader.ToString().Split("\r\n");


            List<string> NumberOfCorrectCallsRecieved = OutputLines.Where(x =>
                (x.ToLower().Contains("light") && x.ToLower().Contains("on"))
            ).ToList();

            Assert.That(NumberOfCorrectCallsRecieved.Count == 1, Is.True);

        }





        [Test]
        public void StateDoorOpen_DoorClose_LightTurnsOffAndOutputIsCalled()
        {
            //Arrange
            doorSut.Open();


            //Act
            ConsoleReader.GetStringBuilder().Clear();
            doorSut.Close();


            //Assert

            string[] OutputLines = ConsoleReader.ToString().Split("\r\n");


            List<string> NumberOfCorrectCallsRecieved = OutputLines.Where(x =>
                (x.ToLower().Contains("light") && x.ToLower().Contains("off"))
            ).ToList();

            Assert.That(NumberOfCorrectCallsRecieved.Count == 1, Is.True);
        }

        //  TODO Vi mangler at kopiere og omskrive tests fra Display og powertube

        [Test]
        public void StateReady_PowerButtonPressed_DisplayShowPower()
        {
            //Arrange


            //Act
            ConsoleReader.GetStringBuilder().Clear();
            powerButtonSut.Press(); // State set to SetPower





            //Assert
            string[] OutputLines = ConsoleReader.ToString().Split("\r\n");


            List<string> NumberOfCorrectCallsRecieved = OutputLines.Where(x =>
                (x.ToLower().Contains("display") && x.ToLower().Contains("shows") && x.ToLower().Contains(" w"))
            ).ToList();


            Assert.That(NumberOfCorrectCallsRecieved.Count == 1, Is.True);

        }

        [Test]
        public void StateSetTime_StartCancelPressed_TimerTicksDisplayShowTime()
        {
            //Arrange
            powerButtonSut.Press(); // State set to SetPower
            timeButtonSut.Press(); //State set to SetTime with a 1 minute timer.



            //Act
            startCancelButtonSut.Press(); //State set to cooking
            ConsoleReader.GetStringBuilder().Clear();

            Thread.Sleep(1100);



            //Assert
            string[] OutputLines = ConsoleReader.ToString().Split("\r\n");


            List<string> NumberOfCorrectCallsRecieved = OutputLines.Where(x =>
                (x.ToLower().Contains("display") && x.ToLower().Contains("shows") && !x.ToLower().Contains(" w"))
            ).ToList();


            Assert.That(NumberOfCorrectCallsRecieved.Count == 1, Is.True);
           

        }



        [Test]
        public void StateSetTime_StartCancelPressed_TimerExpiresOutputIsCorrect()
        {
            //Arrange
            powerButtonSut.Press(); // State set to SetPower
            timeButtonSut.Press(); //State set to SetTime with a 1 minute timer.



            //Act
            ConsoleReader.GetStringBuilder().Clear();
            startCancelButtonSut.Press(); //State set to cooking
            Thread.Sleep(60200);



            //Assert
            string[] OutputLines = ConsoleReader.ToString().Split("\r\n");


            List<string> NumberOfCorrectTickCallsRecieved = OutputLines.Where(x =>
                (x.ToLower().Contains("display") && x.ToLower().Contains("shows") && !x.ToLower().Contains(" w"))
            ).ToList();

            List<string> NumberOfCorrectClearCallsRecieved = OutputLines.Where(x =>
                (x.ToLower().Contains("display") && x.ToLower().Contains("clear"))).ToList();

            List<string> NumberOfCorrectLightOffCallsRecieved = OutputLines.Where(x =>
                (x.ToLower().Contains("light") && x.ToLower().Contains("off") && !x.ToLower().Contains(" w"))
            ).ToList();

            List<string> NumberOfCorrectPowertubeOffCallsRecieved = OutputLines.Where(x =>
                (x.ToLower().Contains("powertube") && x.ToLower().Contains("off") && !x.ToLower().Contains(" w"))
            ).ToList();


            Assert.Multiple(() =>
            {
                Assert.That(NumberOfCorrectTickCallsRecieved.Count == 60, Is.True);
                Assert.That(NumberOfCorrectClearCallsRecieved.Count == 1, Is.True);
                Assert.That(NumberOfCorrectLightOffCallsRecieved.Count == 1, Is.True);
                Assert.That(NumberOfCorrectPowertubeOffCallsRecieved.Count == 1, Is.True);
            });
        }

        [Test]
        public void StateCooking_DoorOpens_DisplayClears()
        {
            //Arrange
            powerButtonSut.Press(); // State set to SetPower
            timeButtonSut.Press(); //State set to SetTime with a 1 minute timer.
            startCancelButtonSut.Press(); //State set to cooking


            //Act

            ConsoleReader.GetStringBuilder().Clear();
            doorSut.Open();



            //Assert
            string[] OutputLines = ConsoleReader.ToString().Split("\r\n");


            List<string> NumberOfCorrectCallsRecieved = OutputLines.Where(x =>
                (x.ToLower().Contains("display") && x.ToLower().Contains("clear"))).ToList();


            Assert.That(NumberOfCorrectCallsRecieved.Count == 1, Is.True);

        }





        [Test]
        public void StateSetTime_StartCancelPressed_PowerTubeTurnsOn()
        {
            //Arrange
            powerButtonSut.Press(); // State set to SetPower
            timeButtonSut.Press(); //State set to SetTime with a 1 minute timer.



            //Act

            ConsoleReader.GetStringBuilder().Clear();
            startCancelButtonSut.Press(); //State set to cooking



            //Assert
            string[] OutputLines = ConsoleReader.ToString().Split("\r\n");


            List<string> NumberOfCorrectCallsRecieved = OutputLines.Where(x =>
                (x.ToLower().Contains("powertube") && x.ToLower().Contains("works"))).ToList();

            
            Assert.That(NumberOfCorrectCallsRecieved.Count == 1, Is.True);

        }



        [Test]
        public void StateCooking_DoorOpens_PowerTubeTurnsOff()
        {
            //Arrange
            powerButtonSut.Press(); // State set to SetPower
            timeButtonSut.Press(); //State set to SetTime with a 1 minute timer.
            startCancelButtonSut.Press(); //State set to cooking


            //Act

            ConsoleReader.GetStringBuilder().Clear();
            doorSut.Open();



            //Assert
            string[] OutputLines = ConsoleReader.ToString().Split("\r\n");



            List<string> NumberOfCorrectCallsRecieved = OutputLines.Where(x =>
                (x.ToLower().Contains("powertube") && x.ToLower().Contains("off"))).ToList();


            Assert.That(NumberOfCorrectCallsRecieved.Count == 1, Is.True);

        }


    }
}