# AWSXRay.Http.Extension
Capture HTTP request/response details and send them to AWS XRay

![Build](https://github.com/waxtell/AWSXRay.Http.Extension/workflows/Build/badge.svg)![Publish to nuget](https://github.com/waxtell/AWSXRay.Http.Extension/workflows/Publish%20to%20nuget/badge.svg?branch=master)

Startup.cs

```csharp
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHttpXRayTracing(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseXRay("SampleApp9000", Configuration);
            app.ActivateXRayHttpDiagnosticsLogging();
```

To include all hosts and all headers:

appsettings.json
```
  "XRayHttpDiagnosticLoggerOptions": {
    "CaptureHosts": [
      {
        "type": "include",
        "Expression": ".*",
        "IsRegEx": true,
        "IncludeRequestBody": false,
        "IncludeResponseBody": true,
        "Traced": false,
        "CaptureRequestHeaders": [
          {
            "type": "include",
            "Expression": ".*",
            "IsRegEx": true
          }
        ],
        "CaptureResponseHeaders": [
          {
            "type": "include",
            "Expression": ".*",
            "IsRegEx": true
          }
        ]
      }
    ]
  }
```

To include all hosts except google.com (you can exclude as many hosts as you want):

appsettings.json
```
  "XRayHttpDiagnosticLoggerOptions": {
    "CaptureHosts": [
      {
        "type": "include",
        "Expression": ".*",
        "IsRegEx": true,
        "IncludeRequestBody": false,
        "IncludeResponseBody": true,
        "Traced": false,
        "CaptureRequestHeaders": [
          {
            "type": "include",
            "Expression": ".*",
            "IsRegEx": true
          }
        ],
        "CaptureResponseHeaders": [
          {
            "type": "include",
            "Expression": ".*",
            "IsRegEx": true
          }
        ]
      },
      {
        "type": "exclude",
        "Expression": "google.com",
        "IsRegEx": false
      }
    ]
  }
```