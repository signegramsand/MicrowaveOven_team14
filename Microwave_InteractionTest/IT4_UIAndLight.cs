using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave_InteractionTest
{
    [TestFixture]
    public class IT4_UIAndLight
    {
        private Door doorSut;
        private UserInterface UI;

        private Button powerButtonSut;
        private Button timeButtonSut;
        private Button startCancelButtonSut;

        private Light light;

        private CookController cooker;

        private IDisplay display;
        
        private IOutput output;

        private ITimer timer;
        private IPowerTube powerTube;

        [SetUp]
        public void Setup()
        {
            //Arrange


            timer = Substitute.For<ITimer>();
            powerTube = Substitute.For<IPowerTube>();
            display = Substitute.For<IDisplay>();
            output = Substitute.For<IOutput>();



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
        public void StateReady_DoorOpen_LightTurnsOnAndOutputIsCalled()
        {
            //Act
            doorSut.Open();

            //Assert
            output.Received(1).OutputLine(Arg.Is<string>(str => str.ToLower().Contains("on")));
        }



        [Test]
        public void StateDoorOpen_DoorClose_LightTurnsOffAndOutputIsCalled()
        {
            //Arrange
            doorSut.Open();
            output.ClearReceivedCalls();

            //Act
            doorSut.Close();


            //Assert
            output.Received(1).OutputLine(Arg.Is<string>(str => str.ToLower().Contains("off")));
            
        }
    }
}
