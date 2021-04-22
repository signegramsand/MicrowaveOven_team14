using System;
using System.Xml.Linq;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave_InteractionTest
{
    public class Tests
    {
        private Door sut;
        private UserInterface UI;

        private IButton powerButton;
        private IButton timeButton;
        private IButton startCancelButton;

        private IDisplay display;
        private ILight light;

        private ICookController cooker;

        [SetUp]
        public void Setup()
        {
            //Arrange
            powerButton = Substitute.For<IButton>();
            timeButton = Substitute.For<IButton>();
            startCancelButton = Substitute.For<IButton>();
            light = Substitute.For<ILight>();
            display = Substitute.For<IDisplay>();
            cooker = Substitute.For<ICookController>();



            sut = new Door();

            UI = new UserInterface(powerButton,timeButton,startCancelButton,sut,display,light,cooker);
            //State is ready by default

            }

        [Test]
        public void StateReady_DoorOpens_LightTurnsOn()
        {
            //Act
            sut.Open();
         
            //Assert
            light.Received(1).TurnOn(); 
        }


        public void StateDoorOpen_DoorCloses_LightTurnsOff()
        {
            //Arrange
            sut.Open();
            light.ClearReceivedCalls();

            //Act
            sut.Close();

            //Assert
            light.Received(1).TurnOff();
        }



    }
}