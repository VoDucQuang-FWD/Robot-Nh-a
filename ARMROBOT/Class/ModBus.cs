using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
namespace ModBus_RTU
{
    public class ModBus_RS485
    {
        private SerialPort _RS485 = new SerialPort();
         public string Modbus_status ;

        #region Constructor/Deconstructor
        /// <summary>
        /// 
        /// </summary>
        public ModBus_RS485() 
        {
            
        }
        ~ModBus_RS485() 
        {
        
        }

        #endregion
        #region Open/Close Serial Port
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_portName"></param>
        /// <param name="_baudRate"></param>
        /// <param name="_dataBits"></param>
        /// <param name="_parity"></param>
        /// <param name="_stopBits"></param>
        /// <returns></returns>
        public bool Opened(string _portName,int _baudRate,int _dataBits, Parity _parity, StopBits _stopBits) 
        {
            //Check status port
            try
            {
                if (!_RS485.IsOpen)
                {
                    _RS485.PortName = _portName;
                    _RS485.BaudRate = _baudRate;
                    _RS485.DataBits = _dataBits;
                    _RS485.Parity = _parity;
                    _RS485.StopBits = _stopBits;

                    _RS485.ReadTimeout = 1000;
                    _RS485.WriteTimeout = 1000;
                    try
                    {
                        _RS485.Open();
                    }
                    catch (Exception er)
                    {

                        Modbus_status = "Error Opening " + _portName + er.Message;
                        return false;
                    }
                    Modbus_status = _portName + " : Opened successfully";
                    return true;
                }
                else
                {
                    Modbus_status = _portName + " : Already Open";
                    return false;
                }
            }
            catch (Exception ex)
            {

                Modbus_status = "Serial Connect error :" + ex.Message;
                return false;
            }
            
        }

        public bool Closed() 
        {
            //Check port status before close
            if (_RS485.IsOpen) 
            {
                try
                {
                    _RS485.Close();
                }
                catch (Exception er)
                {
                    Modbus_status = "Error Closing" + _RS485.PortName + " :" + er.Message;
                    return false;
                }
                Modbus_status = _RS485.PortName + " : Closed successfully";
                return true;
            }
            else 
            {
                Modbus_status = _RS485.PortName + " : Is not Open";
                return false;
            }
        }

        #endregion
        #region Caculating CRC 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="CRC"></param>
        private void GetCRC(byte[] request,ref byte[] CRC) 
        {
            ushort Full_CRC = 0xFFFF;
            byte Hi_CRC = 0xFF;
            byte Lo_CRC = 0xFF;
            char CRC_LSB;
            for (int i = 0; i < (request.Length)-2; i++)
            {
                Full_CRC = (ushort)(Full_CRC ^ request[i]);
                for (int j = 0; j < 8; j++)
                {
                    CRC_LSB = (char)(Full_CRC & 0x0001);
                    Full_CRC = (ushort)((Full_CRC >> 1) & 0x7FFF);
                    if (CRC_LSB == 1) Full_CRC = (ushort)(Full_CRC ^ 0xA001);
                }
            }
            CRC[1] = Hi_CRC = (byte)((Full_CRC >> 8) & 0xFF);
            CRC[0] = Lo_CRC = (byte)(Full_CRC & 0xFF);

        }

        #endregion
        #region Dimension Message
        /// <summary>
        /// 
        /// </summary>
        /// <param name="addrress"></param>
        /// <param name="type"></param>
        /// <param name="start"></param>
        /// <param name="registers"></param>
        /// <param name="message"></param>
        private void Message(byte addrress,byte Function,ushort start,ushort registers,ref byte[]message) 
        {
            byte[] CRC = new byte[2];
            message[0] = addrress;
            message[1] = Function;
            message[2] = (byte)(start >> 8);
            message[3] = (byte)(start);
            message[4] = (byte)(registers >> 8);
            message[5] = (byte)(registers);
            GetCRC(message, ref CRC);
            message[message.Length - 2] = CRC[0];
            message[message.Length - 1] = CRC[1];
            
        }
        #endregion

        #region Check Slave Response
        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private bool CheckResponse(byte[]response) 
        {
            byte[] CRC = new byte[2];
            GetCRC(response, ref CRC);
            if (CRC[0] == response[response.Length - 2] && CRC[1] == response[response.Length - 1])
            {
                return true;
            }
            else return false;
        }
        #endregion

        #region Get Response

        private void Get_Response(ref byte[] response) 
        {
            for (int i = 0; i < response.Length; i++)
            {
                response[i] = (byte)_RS485.ReadByte();
            }
        }
        #endregion
        #region Function 16 - Write Multiple Registers
        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="start"></param>
        /// <param name="registers"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool SendFc16(byte address, ushort start, ushort registers, short[] values)
        {
            //Ensure port is open:
            if (_RS485.IsOpen)
            {
                //Clear in/out buffers:
                _RS485.DiscardOutBuffer();
                _RS485.DiscardInBuffer();
                //Message is 1 addr + 1 fcn + 2 start + 2 reg + 1 count + 2 * reg vals + 2 CRC
                byte[] message = new byte[9 + 2 * registers];
                //Function 16 response is fixed at 8 bytes
                byte[] response = new byte[8];

                //Add bytecount to message:
                message[6] = (byte)(registers * 2);
                //Put write values into message prior to sending:
                for (int i = 0; i < registers; i++)
                {
                    message[7 + 2 * i] = (byte)(values[i] >> 8);
                    message[8 + 2 * i] = (byte)(values[i]);
                }
                //Build outgoing message:
                Message(address, (byte)16, start, registers, ref message);

                //Send Modbus message to Serial Port:
                try
                {
                    _RS485.Write(message, 0, message.Length);
                    Get_Response(ref response);
                }
                catch (Exception err)
                {
                    Modbus_status = "Error in write event: " + err.Message;
                    return false;
                }
                //Evaluate message:
                if (CheckResponse(response))
                {
                    Modbus_status = "Write successful";
                    return true;
                }
                else
                {
                    Modbus_status = "CRC error";
                    return false;
                }
            }
            else
            {
                Modbus_status = "Serial port not open";
                return false;
            }
        }
        #endregion

        #region Function 3 - Read Registers
        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="start"></param>
        /// <param name="registers"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool SendFc3(byte address, ushort start, ushort registers, ref short[] values)
        {
           
            if (_RS485.IsOpen)
            {
                //Clear in/out buffers:
                _RS485.DiscardOutBuffer();
                _RS485.DiscardInBuffer();
                //Function 3 request is always 8 bytes:
                byte[] message = new byte[8];
                //Function 3 response buffer:
                byte[] response = new byte[5 + 2 * registers];
                //Build outgoing modbus message:
                Message(address, (byte)03, start, registers, ref message);
                //Send modbus message to Serial Port:
                try
                {
                    _RS485.Write(message, 0, message.Length);
                    Get_Response(ref response);
                }
                catch (Exception err)
                {
                    Modbus_status = "Error in read event: " + err.Message;
                    return false;
                }
                //Evaluate message:
                if (CheckResponse(response))
                {
                    //Return requested register values:
                    for (int i = 0; i < (response.Length - 5) / 2; i++)
                    {
                        values[i] = response[2 * i + 3];
                        values[i] <<= 8;
                        values[i] += response[2 * i + 4];
                    }
                    Modbus_status = "Read successful";
                    return true;
                }
                else
                {
                    Modbus_status = "CRC error";
                    return false;
                }
            }
            else
            {
                Modbus_status = "Serial port not open";
                return false;
            }

        }
        #endregion
        #region Function 15
        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="start"></param>
        /// <param name="registers"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool SendFc15(byte address, ushort start, ushort registers, short[] values)
        {
            //Ensure port is open:
            if (_RS485.IsOpen)
            {
                //Clear in/out buffers:
                _RS485.DiscardOutBuffer();
                _RS485.DiscardInBuffer();
                ushort reg = Convert.ToUInt16((double)registers / 8 + (double)0.5);
               
                byte[] message = new byte[9 + reg];
              
                byte[] response = new byte[8];

                //Add bytecount to message:
                message[6] = (byte)(reg);
                //Put write values into message prior to sending:
                for (int i = 0; i < reg; i++)
                {

                    message[7 + i] = (byte)(values[i]);
                }
                //Build outgoing message:
                Message(address, (byte)15, start, registers, ref message);

                //Send Modbus message to Serial Port:
                try
                {
                    _RS485.Write(message, 0, message.Length);
                    Get_Response(ref response);
                }
                catch (Exception err)
                {
                    Modbus_status = "Error in write event: " + err.Message;
                    return false;
                }
                //Evaluate message:
                if (CheckResponse(response))
                {
                    Modbus_status = "Write successful";
                    return true;
                }
                else
                {
                    Modbus_status = "CRC error";
                    return false;
                }
            }
            else
            {
                Modbus_status = "Serial port not open";
                return false;
            }
        }
        #endregion
        #region Function 2
        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="start"></param>
        /// <param name="registers"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool SendFc02(byte address, ushort start, ushort registers, ref short[] values)
        {
            if (_RS485.IsOpen)
            {
                _RS485.DiscardInBuffer();
                _RS485.DiscardOutBuffer();
                byte[] message = new byte[8];
                ushort reg = Convert.ToUInt16((double)registers / 8 + (double)0.5);
                byte[] response = new byte[5 + reg];
                Message(address, (byte)02, start, registers, ref message);
                try
                {
                    _RS485.Write(message, 0, message.Length);
                    Get_Response(ref response);
                }
                catch (Exception ex)
                {
                    Modbus_status = "Error in read(01) event: " + ex.Message;
                    return false; throw;
                }
                if (CheckResponse(response))
                {
                    //Return requested register values:
                    for (int i = 0; i < (response.Length - 5); i++)
                    {
                        values[i] = response[i + 3];

                    }
                    Modbus_status = "Read successful";
                    return true;
                }
                else
                {
                    Modbus_status = "CRC error";
                    return false;
                }
            }
            else
            {
                Modbus_status = "Serial port not open";
                return false;
            }
        }

        #endregion
        #region Funtion 1
        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="start"></param>
        /// <param name="registers"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool SendFc01(byte address, ushort start, ushort registers, ref short[] values) 
        {
            if (_RS485.IsOpen)
            {
                _RS485.DiscardInBuffer();
                _RS485.DiscardOutBuffer();
                byte[] message = new byte[8];
                ushort reg = Convert.ToUInt16((double)registers/8 + (double)0.5);
                byte[] response = new byte[5 + reg];
                Message(address, (byte)01, start, registers, ref message);
                try
                {
                    _RS485.Write(message, 0, message.Length);
                    Get_Response(ref response);
                }
                catch (Exception ex)
                {
                    Modbus_status = "Error in read(01) event: " + ex.Message;
                    return false; throw;
                }
                if (CheckResponse(response))
                {
                    //Return requested register values:
                    for (int i = 0; i < (response.Length - 5); i++)
                    {
                        values[i] = response[i + 3];
                       
                    }
                    Modbus_status = "Read successful";
                    return true;
                }
                else
                {
                    Modbus_status = "CRC error";
                    return false;
                }
            }
            else 
            {
                Modbus_status = "Serial port not open";
                return false;
            }
        }
        #endregion
        #region Function 4
        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="start"></param>
        /// <param name="registers"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool SendFc04(byte address, ushort start, ushort registers, ref short[] values)
        {

            if (_RS485.IsOpen)
            {
                //Clear in/out buffers:
                _RS485.DiscardOutBuffer();
                _RS485.DiscardInBuffer();
                //Function 3 request is always 8 bytes:
                byte[] message = new byte[8];
                //Function 3 response buffer:
                byte[] response = new byte[5 + 2 * registers];
                //Build outgoing modbus message:
                Message(address, (byte)04, start, registers, ref message);
                //Send modbus message to Serial Port:
                try
                {
                    _RS485.Write(message, 0, message.Length);
                    Get_Response(ref response);
                }
                catch (Exception err)
                {
                    Modbus_status = "Error in read(04) event: " + err.Message;
                    return false;
                }
                //Evaluate message:
                if (CheckResponse(response))
                {
                    //Return requested register values:
                    for (int i = 0; i < (response.Length - 5) / 2; i++)
                    {
                        values[i] = response[2 * i + 3];
                        values[i] <<= 8;
                        values[i] += response[2 * i + 4];
                    }
                    Modbus_status = "Read successful";
                    return true;
                }
                else
                {
                    Modbus_status = "CRC error";
                    return false;
                }
            }
            else
            {
                Modbus_status = "Serial port not open";
                return false;
            }

        }
        #endregion
    }
}
