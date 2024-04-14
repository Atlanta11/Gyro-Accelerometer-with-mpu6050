using UnityEngine;
using System.IO;
using System.Globalization;
using System.IO.Ports;

public class SerialTest : MonoBehaviour
{
    SerialPort stream = new SerialPort("COM10", 115200);
    public string strReceived;

    // Correctiehoeken voor de X-, Y- en Z-assen (in graden)
    public float xAxisCorrectionAngle = 0f;
    public float yAxisCorrectionAngle = 0f;
    public float zAxisCorrectionAngle = 0f;

    void Start()
    {
        stream.Open(); // Open the Serial Stream.
    }

    void Update()
    {
        strReceived = stream.ReadLine(); // Read the information

        string[] strData = strReceived.Split(',');
        if (strData.Length >= 4 && strData[0] != "" && strData[1] != "" && strData[2] != "" && strData[3] != "") // make sure data are ready
        {
            if (float.TryParse(strData[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float qw) &&
                float.TryParse(strData[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float qx) &&
                float.TryParse(strData[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float qy) &&
                float.TryParse(strData[3], NumberStyles.Float, CultureInfo.InvariantCulture, out float qz))
            {
                // Convert quaternion to Euler angles
                Vector3 euler = QuaternionToEuler(new Quaternion(qx, -qy, -qz, qw));

                // Apply corrections to Euler angles
                euler.x += xAxisCorrectionAngle;
                euler.y += yAxisCorrectionAngle;
                euler.z += zAxisCorrectionAngle;

                // Set the rotation of the GameObject using Euler angles
                transform.rotation = Quaternion.Euler(euler);
            }
            else
            {
                Debug.LogError("Failed to parse quaternion values from serial data: " + strReceived);
            }
        }
    }

    // Function to convert quaternion to Euler angles
    Vector3 QuaternionToEuler(Quaternion q)
    {
        return q.eulerAngles;
    }

    void OnDestroy()
    {
        if (stream != null && stream.IsOpen)
        {
            stream.Close();
        }
    }
}
