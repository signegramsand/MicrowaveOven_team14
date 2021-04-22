using System;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave_InteractionTest
{
    [TestFixture]
    public class IT2_UIAndButtons
    {
        private Door door;
        private UserInterface UI;

        private Button powerButtonSut;
        private Button timeButtonSut;
        private Button startCancelButtonSut;

        private IDisplay display;
        private ILight light;

        private ICookController cooker;

        [SetUp]
        public void Setup()
        {
            //Arrange
            door = new Door();
            light = Substitute.For<ILight>();
            display = Substitute.For<IDisplay>();
            cooker = Substitute.For<ICookController>();

            powerButtonSut = new Button();
            timeButtonSut = new Button();
            startCancelButtonSut = new Button();



            UI = new UserInterface(powerButtonSut, timeButtonSut, startCancelButtonSut, door, display, light, cooker);
            //State is ready by default

        }


        [Test]
        public void StateReady_PowerButtonPress_DisplayShowsPower()
        {

            //Act
            powerButtonSut.Press();

            //Assert
            display.Received(1).ShowPower(Arg.Any<int>());

        }

        [Test]
        public void StateSetPower_StartCancelButtonPress_DisplayShowsPower()
        {
            //Arrange
            powerButtonSut.Press();
            display.ClearReceivedCalls();

            //Act
            startCancelButtonSut.Press();

            //Assert
            display.Received(1).Clear();

        }



        [Test]
        public void StateSetPower_TimeButtonPress_DisplayShowsTime()
        {
            //Arrange
            powerButtonSut.Press();
            display.ClearReceivedCalls();

            //Act
            timeButtonSut.Press();

            //Assert
            display.Received(1).ShowTime(Arg.Any<int>(),Arg.Any<int>());

        }
    }
}