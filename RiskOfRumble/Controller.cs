using System.Diagnostics;
using System.Reflection;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Text;
using System.Collections;
using System.IO.Pipes;
using System.Linq;

namespace Vibrating
{
    class Controller
    {
        class TimeVibration
        {
            internal TimeVibration(float vibrating)
            {
                intensity = vibrating;
            }
            internal float intensity;
        }

        internal IEnumerator enumerator;
        readonly Process _processServer;
        readonly StreamWriter _streamWriter;
        readonly StreamReader _streamReader;
        readonly StreamReader _streamError;
        readonly List<TimeVibration> _list = new List<TimeVibration>();
        public Controller()
        {
            _processServer = new Process
            {
                StartInfo =
                {
                    FileName = Assembly.GetExecutingAssembly().Location,
                    UseShellExecute = false,
                    CreateNoWindow = false,
                    Arguments = Process.GetCurrentProcess().Id.ToString(),
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            _processServer.Start();
            _streamWriter = _processServer.StandardInput;
            _streamReader = _processServer.StandardOutput;
            _streamError = _processServer.StandardError;
            Application.quitting += Application_quitting;
        }

        public IEnumerator Read()
        {
            var output = _streamReader.ReadLineAsync();
            var error = _streamError.ReadLineAsync();
            while (true)
            {
                while (!output.IsCompleted && !error.IsCompleted)
                {
                    yield return null;
                }

                if (output.IsCompleted)
                {
                    UnityEngine.Debug.Log(output.Result);
                    output = _streamReader.ReadLineAsync();
                }

                if (error.IsCompleted)
                {
                    UnityEngine.Debug.LogError(error.Result);
                    error = _streamError.ReadLineAsync();
                }
            }
        }

        private void Application_quitting()
        {
            _processServer.Kill();
            _streamWriter.Close();
        }

        public void Update(float deltaTime)
        {
            if (_list.Count == 0)
            {
                return;
            }

            foreach (var t in _list)
            {
                t.intensity = Mathf.Lerp(t.intensity, -0.01f, 0.5f * deltaTime);
            }

            _list.RemoveAll(p => p.intensity <= 0f);

            var sum = _list.Sum(item => item.intensity);

            SendVibrate(sum);
        }

        public void Vibrating(float vibrating)
        {
            _list.Add(new TimeVibration(vibrating));
            SendVibrate(vibrating);
        }

        public void SendVibrate(float vibrating)
        {
            _streamWriter.WriteLine(vibrating);
        }
    }
}
