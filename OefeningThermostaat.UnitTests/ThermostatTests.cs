using Moq;
using FluentAssertions;

namespace OefeningThermostaat.UnitTests
{
    public class ThermostatTests
    {
        private readonly Mock<ITemperatureSensor> _temperatureSensor;
        private readonly Mock<IHeatingElement> _heatingElement;
        private readonly Thermostat _thermostat;

        public ThermostatTests()
        {
            _temperatureSensor = new Mock<ITemperatureSensor>();
            _heatingElement = new Mock<IHeatingElement>();
            _thermostat = new Thermostat(_temperatureSensor.Object, _heatingElement.Object)
            {
                Setpoint = 20,
                Offset = 2,
                MaxFailures = 3
            };
        }

        [Fact]
        public void WorkWhenTemperatureBetweenBoundariesDoNothing()
        {
            //arrange
            _temperatureSensor.Setup(t => t.GetTemperature()).Returns(21);

            //act
            _thermostat.Work();

            //assert
            _heatingElement.Verify(h => h.Enable(), Times.Never);
            _heatingElement.Verify(h => h.Disable(), Times.Never);
        }

        [Fact]
        public void WorkWhenTemperatureLessThanLowerBoundaryEnableHeatingElement()
        {
            //arrange
            _temperatureSensor.Setup(t => t.GetTemperature()).Returns(17);

            //act
            _thermostat.Work();

            //assert
            _heatingElement.Verify(h => h.Enable(), Times.Once);
            _heatingElement.Verify(h => h.Disable(), Times.Never);
        }


        [Fact]
        public void WorkWhenTemperatureEqualsLowerBoundaryDoNothing()
        {
            //arrange
            _temperatureSensor.Setup(t => t.GetTemperature()).Returns(18);

            //act
            _thermostat.Work();

            //assert
            _heatingElement.Verify(h => h.Enable(), Times.Never);
            _heatingElement.Verify(h => h.Disable(), Times.Never);
        }

        [Fact]
        public void WorkWhenTemperatureHigherThanUpperBoundaryDisableHeatingElement()
        {
            //arrange
            _temperatureSensor.Setup(t => t.GetTemperature()).Returns(24);

            //act
            _thermostat.Work();

            //assert
            _heatingElement.Verify(h => h.Enable(), Times.Never);
            _heatingElement.Verify(h => h.Disable(), Times.Once);
        }

        [Fact]
        public void WorkWhenTemperatureEqualsUpperBoundaryDoNothing()
        {
            //arrange
            _temperatureSensor.Setup(t => t.GetTemperature()).Returns(22);

            //act
            _thermostat.Work();

            //assert
            _heatingElement.Verify(h => h.Enable(), Times.Never);
            _heatingElement.Verify(h => h.Disable(), Times.Never);
        }

        [Fact]
        public void WorkWhenTemperatureFailsAndNotInsafeModeDoNothing()
        {
            _temperatureSensor.Setup(t => t.GetTemperature()).Returns(20);
            _thermostat.Work();

            _temperatureSensor.Setup(t => t.GetTemperature()).Throws<Exception>();
            _thermostat.Work();

            _thermostat.InsafeMode.Should().BeFalse();
            _heatingElement.Verify(h => h.Enable(), Times.Never);
            _heatingElement.Verify(h => h.Disable(), Times.Never);
        }

        [Fact]
        public void WorkWhenTemperatureFailsAndMaxFailuresInSafeMode()
        {
            _temperatureSensor.Setup(t => t.GetTemperature()).Throws<Exception>();

            for (int i = 0; i < _thermostat.MaxFailures; i++)
            {
                _thermostat.Work();
            }

            _thermostat.InsafeMode.Should().BeTrue();
            _heatingElement.Verify(h => h.Enable(), Times.Never);
            _heatingElement.Verify(h => h.Disable(), Times.Once);
        }

        [Fact]
        public void WorkWhenInSafeModeAndTemperatureSuccesReset()
        {
            _temperatureSensor.Setup(t => t.GetTemperature()).Throws<Exception>();

            for (int i = 0; i < _thermostat.MaxFailures; i++)
            {
                _thermostat.Work();
            }

            _temperatureSensor.Setup(t => t.GetTemperature()).Returns(20);
            _thermostat.Work();

            _thermostat.InsafeMode.Should().BeFalse();
        }
    }
}