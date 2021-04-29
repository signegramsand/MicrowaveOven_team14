using System;
using System.Collections.Generic;
using System.Text;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave_InteractionTest.Test.Unit
{
    [TestFixture]
    public class IT6_DisplayPowerTubeAndUICookcontrol
    {
        private Door doorSut;
        private UserInterface UI;

        private Button powerButtonSut;
        private Button timeButtonSut;
        private Button startCancelButtonSut;

        private Light light;
        private ITimer timer;




        private IOutput output;

        private CookController cooker;

        private Display display;

        private PowerTube powerTube;

        [SetUp]
        public void Setup()
        {
            //Arrange




            output = Substitute.For<IOutput>();


            timer = Substitute.For<ITimer>();


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

        [Test]
        public void StateSetTime_StartCancelPressed_TimerExpiredPowerTubeTurnedOff()
        {
            //Arrange
            powerButtonSut.Press(); // State set to SetPower
            timeButtonSut.Press(); //State set to SetTime with a 1 minute timer.



            //Act
            startCancelButtonSut.Press(); //State set to cooking
            output.ClearReceivedCalls();


            timer.Expired += Raise.Event();

            //Assert
            Assert.Multiple(() =>
            {
                output.Received(1).OutputLine(Arg.Is<string>(str =>
                    str.ToLower().Contains("display") && str.ToLower().Contains("clear")));

                output.Received(1).OutputLine(Arg.Is<string>(str =>
                    str.ToLower().Contains("powertube") && str.ToLower().Contains("off")));

            });
            
        }


        [Test]
        public void StateSetTime_StartCancelPressed_TimerTicksDisplayShowTime()
        {
            //Arrange
            powerButtonSut.Press(); // State set to SetPower
            timeButtonSut.Press(); //State set to SetTime with a 1 minute timer.



            //Act
            startCancelButtonSut.Press(); //State set to cooking
            output.ClearReceivedCalls();


            timer.TimerTick += Raise.Event();


                output.Received(1).OutputLine(Arg.Is<string>(str =>
                    str.ToLower().Contains("display") && str.ToLower().Contains("shows") 
                                                      && !str.ToLower().Contains(" w")));


        }


        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        [TestCase(8)]
        [TestCase(9)]
        [TestCase(10)]
        [TestCase(11)]
        [TestCase(12)]
        [TestCase(13)]
        [TestCase(14)]
        [TestCase(15)]
        public void StateSetTime_SetPowerStartCancelPressed_PowerTubeTurnsOn(int NumberOfPowerpresses)
        {
            //Arrange
            for (int i = 0; i < NumberOfPowerpresses; i++)
            {
                powerButtonSut.Press(); // State set to SetPower
            }
            
            timeButtonSut.Press(); //State set to SetTime with a 1 minute timer.



            //Act
            output.ClearReceivedCalls();

            startCancelButtonSut.Press(); //State set to cooking

            //Assert
            output.Received(1).OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("powertube") && str.ToLower().Contains("works")));

        }

        [Test]
        public void StateReady_SetPower_DisplayShowPower()
        {
            //Arrange


            //Act
            powerButtonSut.Press(); // State set to SetPower

            //Assert
            output.Received(1).OutputLine(Arg.Is<string>(str =>
                    str.ToLower().Contains("display") && str.ToLower().Contains("shows")
                                                      && str.ToLower().Contains(" w")));
        }



    }
}
