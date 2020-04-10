using System.Diagnostics;
using System.Reflection;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

namespace Vibrating
{
    class Controller
    {
        class TimeVibration
        {
            internal TimeVibration(float vibrating, float deltaTime)
            {
                intensity = vibrating;
                time = deltaTime;
            }
            internal float time;
            internal float intensity;
        }

        TimeVibration timeVibration = null;
        Process processServer;
        StreamWriter streamWriter;
        StreamReader streamReader;
        List<TimeVibration> list = new List<TimeVibration>();
        public Controller()
        {
            processServer = new Process();
            processServer.StartInfo.FileName = Assembly.GetExecutingAssembly().Location;
            processServer.StartInfo.UseShellExecute = false;
            processServer.StartInfo.CreateNoWindow = false;
            processServer.StartInfo.Arguments = Process.GetCurrentProcess().Id.ToString();
            processServer.StartInfo.RedirectStandardInput = true;
            processServer.StartInfo.RedirectStandardOutput = true;
            processServer.Start();
            streamWriter = processServer.StandardInput;
            streamReader = processServer.StandardOutput;
            Application.quitting += Application_quitting;
        }

        private void Application_quitting()
        {
            processServer.Kill();
            streamWriter.Close();
        }

        public void Update(float deltaTime)
        {
            //while (streamReader. >= 0)
            //{
            //    UnityEngine.Debug.Log(streamReader.ReadLine());
            //}

            //for (int i = 0; i < list.Count; i++)
            //{
            //    var tmp = list[i];
            //    tmp.time -= deltaTime;
            //    list[i] = tmp;
            //}
            if (timeVibration == null)
            {
                return;
            }

            timeVibration.time -= deltaTime;
            if (timeVibration.time <= 0f)
            {
                timeVibration = null;
                SendVibrate(0f);
            }

            //list.RemoveAll(p => p.time <= 0f);

            //var max = 0f;
            //foreach (var item in list)
            //{
            //    max = max > item.intensity ? max : item.intensity;
            //}
        }

        public void Vibrating(float vibrating, float deltaTime)
        {
            timeVibration = new TimeVibration(vibrating, deltaTime);
            SendVibrate(vibrating);
        }

        public void SendVibrate(double vibrating)
        {
            streamWriter.WriteLine(vibrating);
        }
    }
}
