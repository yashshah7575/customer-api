# blanket-api
API to server UI/Function

#Requirement - 1 : Build an API
- API is built using C#, EF Core and In-memory DB
- It can be accesed via swagger http://localhost:13517/swagger/index.html
- Endpoints:
    Customers:
        - GET Customers(All)
        - GET Customer By Id
        - POST Customer
        - PUT Customer
        - Delete Customer by Id
    System
        - Ping
        - Version

#Requirement - 2 : Integration and/or Acceptance Testing


#Requirement 3 : Provide Instrumentation of your API (Observability)
For this step I have enabled logger at repository layer. This is just sample. We can do it in whole application.

Due to time limitation, I can not complete application metrics tracking part. But, we can achive it using below steps
    1) Install OpenTelemetry nuget pakages
        OpenTelemetry.Extensions.Hosting
        OpenTelemetry.Instrumentation.AspNetCore
        OpenTelemetry.Instrumentation.Http
        OpenTelemetry.Instrumentation.EntityFrameworkCore
        OpenTelemetry.Exporter.Console
    2) We can unable it using of different metric in Startup. i.e. below code will enable tracking of Request latency
    and DB call timing
            builder.Services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService("CustomerAPI"))
            .WithMetrics(m =>
            {
                m.AddAspNetCoreInstrumentation();
                m.AddHttpClientInstrumentation();
                m.AddRuntimeInstrumentation();
                m.AddMeter("CustomerAPI.Metrics");
                m.AddConsoleExporter(); // For development
            })
            .WithTracing(t =>
            {
                t.AddAspNetCoreInstrumentation();
                t.AddHttpClientInstrumentation();
                t.AddEntityFrameworkCoreInstrumentation();
                t.AddConsoleExporter();
            });
    3) We can enable UI as well for such metrics using nuget pakcage AspNetCore.HealthChecks.UI


#Requirement - 4: Containerization
###Steps to build docker image:
1. Run docker desktop in background
2. Docker file exists here
3. Open "Customer.Api/Customer.Api/" folder in terminal
4. Run command "docker buildx build ."

###Steps to run image
Now you have image build, you can simply run "Docker Run"

#Requirement - 5: Kubernetes
    I am aware of Kubernetes concepts. But, I haven't worked on this. So skipping this section

#Requirement - 6: CI/CD
- Created sample terraform code to deploy code
- Steps in pipeline:
    1. Checkout code from github
    2. Build
    3. Run Unit Test
    4. Package and deploy : Dev environment
    5. Automation - Dev environment
    6. Package and deploy : STG environment
    7. Automation - STG environment
    8. Manual User Approval
    9. Package and deploy : UAT environment
    10. Automation - UAT environment
    11. Manual User Approval
    12. Package and deploy : PROD environment
    13. Automation - PROD environment

#Step 7: App Integration
