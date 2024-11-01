using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OefeningThermostaat
{
    public class Thermostat
    {
        private readonly ITemperatureSensor _temperatureSensor;
        private readonly IHeatingElement _heatingElement;

        private int _failures = 0;
        public int MaxFailures { get; set; }
        public double Setpoint { get; set; }
        public double Offset { get; set; }

        public bool InsafeMode
        {
            get
            {
                return _failures >= MaxFailures ? true : false;
            }
        }

        public Thermostat(ITemperatureSensor temperatureSensor, IHeatingElement heatingElement)
        {
            _temperatureSensor = temperatureSensor;
            _heatingElement = heatingElement;
        }

        public void Work()
        {
            try
            {
                double temperature = _temperatureSensor.GetTemperature();
                _failures = 0;

                if (temperature > Setpoint - Offset && temperature < Setpoint + Offset)
                {
                    //
                }
                else if (temperature < Setpoint - Offset)
                {
                    _heatingElement.Enable();
                }
                else if (temperature == Setpoint - Offset)
                {
                    //
                }
                else if (temperature > Setpoint + Offset)
                {
                    _heatingElement.Disable();
                }
                else if (temperature == Setpoint + Offset)
                {
                    //
                }
                else
                {
                    //
                }
            }
            catch
            {
                _failures++;
                if (_failures >= MaxFailures)
                {
                    _heatingElement.Disable();
                }
            }
        }
    }
}
