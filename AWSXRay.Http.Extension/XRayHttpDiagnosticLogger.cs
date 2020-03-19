using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon.XRay.Recorder.Core;
using AWSXRay.SqlClient.Extension;
using Microsoft.Extensions.DiagnosticAdapter;

namespace AWSXRay.Http.Extension
{
    internal static class AsyncHelper
    {
        private static readonly TaskFactory TaskFactory = new
            TaskFactory(CancellationToken.None,
                TaskCreationOptions.None,
                TaskContinuationOptions.None,
                TaskScheduler.Default);
        public static void RunSync(Func<Task> func)
        {
            AsyncHelper
                .TaskFactory
                .StartNew(func)
                .Unwrap()
                .GetAwaiter()
                .GetResult();
        }
        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return
                AsyncHelper
                    .TaskFactory
                    .StartNew(func)
                    .Unwrap()
                    .GetAwaiter()
                    .GetResult();
        }
    }

    public class XRayHttpDiagnosticLogger : IObserver<DiagnosticListener>
    {
        private readonly List<IDisposable> _subscriptions;
        private readonly XRayHttpDiagnosticLoggerOptions _options;

        public XRayHttpDiagnosticLogger(XRayHttpDiagnosticLoggerOptions options)
        {
            _options = options;

            _subscriptions = new List<IDisposable>
            {
                DiagnosticListener.AllListeners.Subscribe(this)
            };
        }

        void IObserver<DiagnosticListener>.OnNext(DiagnosticListener diagnosticListener)
        {
            if (diagnosticListener.Name == "HttpHandlerDiagnosticListener")
            {
                _subscriptions.Add(diagnosticListener.SubscribeWithAdapter(this));
            }
        }

        void IObserver<DiagnosticListener>.OnError(Exception error)
        {
        }

        void IObserver<DiagnosticListener>.OnCompleted()
        {
            foreach (var sub in _subscriptions)
            {
                sub.Dispose();
            }
        }

        [DiagnosticName("System.Net.Http.HttpRequestOut.Start")]
        public virtual void OnHttpRequestOutStart(System.Net.Http.HttpRequestMessage request)
        {
            LogRequest(request);
        }

        [DiagnosticName("System.Net.Http.HttpRequestOut.Stop")]
        public virtual void OnHttpRequestOutStop(System.Net.Http.HttpRequestMessage request, System.Net.Http.HttpResponseMessage response, TaskStatus requestTaskStatus)
        {
            LogResponse(response);
        }

        [DiagnosticName("System.Net.Http.Request")]
        public virtual void OnRequest(System.Net.Http.HttpRequestMessage request)
        {
            LogRequest(request);
        }

        [DiagnosticName("System.Net.Http.Response")]
        public virtual void OnResponse(System.Net.Http.HttpResponseMessage response)
        {
            LogResponse(response);
        }

        private void LogRequest(System.Net.Http.HttpRequestMessage request)
        {
            AWSXRayRecorder
                .Instance
                .With
                (
                    recorder =>
                    {
                        if (recorder.IsEntityPresent() && request != null)
                        {
                            recorder.BeginSubsegment(request.RequestUri.Host);
                            recorder.SetNamespace("remote");

                            recorder
                                .AddHttpInformation
                                (
                                    "request",
                                    new
                                    {
                                        method = request.Method.ToString(),
                                        url = request.RequestUri.ToString(),
                                        user_agent = request.Headers.UserAgent.ToString()
                                    }
                                );

                            if (_options.ShouldCaptureDetails(request.RequestUri.Host, out var include))
                            {
                                if (include.IncludeRequestBody)
                                {
                                    var stream = AsyncHelper.RunSync(() => request.Content.ReadAsStreamAsync());
                                    stream.Seek(0, SeekOrigin.Begin);
                                    var reader = new StreamReader(stream);
                                    var content = reader.ReadToEnd();

                                    recorder
                                        .AddMetadata
                                        (
                                            "request",
                                            new
                                            {
                                                body = content
                                            }
                                        );
                                }
                            }
                            //recorder.AddHttpInformation("traced", false);
                        }
                    }
                );
        }

        private void LogResponse(System.Net.Http.HttpResponseMessage response)
        {
            AWSXRayRecorder
                .Instance
                .With
                (
                    recorder =>
                    {
                        if (recorder.IsEntityPresent() && response != null)
                        {
                            recorder
                                .AddHttpInformation
                                (
                                    "response",
                                    new
                                    {
                                        status = (int) response.StatusCode
                                    }
                                );

                            if (_options.ShouldCaptureDetails(response.RequestMessage.RequestUri.Host, out var include))
                            {
                                if (include.IncludeRequestBody)
                                {
                                    var stream = AsyncHelper.RunSync(() => response.Content.ReadAsStreamAsync());
                                    stream.Seek(0, SeekOrigin.Begin);
                                    var reader = new StreamReader(stream);
                                    var content = reader.ReadToEnd();

                                    recorder
                                        .AddMetadata
                                        (
                                            "response",
                                            new
                                            {
                                                body = content
                                            }
                                        );
                                }
                            }
                        }

                        recorder.EndSubsegment();
                    }
                );
        }
    }
}
