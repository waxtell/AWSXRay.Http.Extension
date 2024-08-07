﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.XRay.Recorder.Core;
using AWSXRay.SqlClient.Extension;
using Microsoft.Extensions.DiagnosticAdapter;

namespace AWSXRay.Http.Extension
{
    public class XRayHttpDiagnosticLogger : IObserver<DiagnosticListener>
    {
        private readonly List<IDisposable> _subscriptions;
        private readonly XRayHttpDiagnosticLoggerOptions _options;

        public XRayHttpDiagnosticLogger(XRayHttpDiagnosticLoggerOptions options)
        {
            _options = options;

            _subscriptions = new List<IDisposable>();
            _subscriptions.Add(DiagnosticListener.AllListeners.Subscribe(this));
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

        [DiagnosticName("System.Net.Http.Exception")]
        public void OnHttpRequestOutException(Exception exception, System.Net.Http.HttpRequestMessage request)
        {
            AWSXRayRecorder
                .Instance
                .With
                (
                    recorder =>
                    {
                        if (recorder.IsEntityPresent() && exception != null && _options.ShouldCaptureHost(request.RequestUri.Host, out _))
                        {
                            recorder.AddException(exception);
                        }
                    }
                );
        }

        [DiagnosticName("System.Net.Http.HttpRequestOut.Start")]
        public void OnHttpRequestOutStart(System.Net.Http.HttpRequestMessage request)
        {
            LogRequest(request);
        }

        [DiagnosticName("System.Net.Http.HttpRequestOut.Stop")]
        public void OnHttpRequestOutStop(System.Net.Http.HttpRequestMessage _, System.Net.Http.HttpResponseMessage response, TaskStatus __)
        {
            LogResponse(response);
        }

        [DiagnosticName("System.Net.Http.Request")]
        public void OnRequest(System.Net.Http.HttpRequestMessage request)
        {
            LogRequest(request);
        }

        [DiagnosticName("System.Net.Http.Response")]
        public void OnResponse(System.Net.Http.HttpResponseMessage response)
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
                            if (_options.ShouldCaptureHost(request.RequestUri.Host, out var include))
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

                                if (include.IncludeRequestBody && request.Content != null)
                                {
                                    recorder
                                        .AddMetadata
                                        (
                                            "request",
                                            "body",
                                            request.Content.ToObject()
                                        );
                                }

                                var headersToInclude = request
                                                        .Headers
                                                        .Where(header => include.ShouldCaptureRequestHeader(header.Key))
                                                        .ToDictionary(x => x.Key, y => string.Join(",", y.Value));

                                if (headersToInclude.Any())
                                {
                                    recorder
                                        .AddMetadata
                                        (
                                            "request",
                                            "headers",
                                            headersToInclude
                                        );
                                }

                                if (include.Traced.HasValue)
                                {
                                    recorder.AddHttpInformation("traced", include.Traced.Value);
                                }
                            }
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
                            if (_options.ShouldCaptureHost(response.RequestMessage.RequestUri.Host, out var include))
                            {
                                recorder
                                    .AddHttpInformation
                                    (
                                        "response",
                                        new
                                        {
                                            status = (int)response.StatusCode,
                                            content_length = response.Content.Headers.ContentLength ?? 0
                                        }
                                    );

                                if (!string.IsNullOrEmpty(response.ReasonPhrase))
                                {
                                    recorder
                                        .AddMetadata
                                        (
                                            "response",
                                            "reason_phrase",
                                            response.ReasonPhrase
                                        );
                                }

                                if (include.IncludeResponseBody && response.Content != null)
                                {
                                    recorder
                                        .AddMetadata
                                        (
                                            "response",
                                            "body",
                                            response.Content.ToObject()
                                        );

                                    var headersToInclude = response
                                                            .Content
                                                            .Headers
                                                            .Where(header => include.ShouldCaptureResponseHeader(header.Key))
                                                            .ToDictionary(x => x.Key, y => string.Join(",", y.Value));

                                    if (headersToInclude.Any())
                                    {
                                        recorder
                                            .AddMetadata
                                            (
                                                "response",
                                                "headers",
                                                headersToInclude
                                            );
                                    }
                                }

                                if (response.StatusCode >= HttpStatusCode.InternalServerError)
                                {
                                    recorder.MarkFault();
                                }
                                else if (response.StatusCode >= HttpStatusCode.BadRequest)
                                {
                                    recorder.MarkError();
                                }

                                recorder.EndSubsegment();
                            }
                        }
                    }
                );
        }
    }
}
